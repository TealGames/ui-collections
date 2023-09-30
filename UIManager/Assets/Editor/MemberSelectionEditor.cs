using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using Object = UnityEngine.Object;

namespace Game.UI
{
    [CustomEditor(typeof(MemberSelectionSO))]
    public class MemberSelectionEditor : ExtendedEditor
    {
        private bool allowFields = false;
        private bool allowProperties = false;
        private bool allowMethods = false;

        private Type acceptedType = null;
        private Type convertedType = null;
        private string otherTypeName = "";
        private int currentAssemblyIndex = 0;
        Dictionary<string, MemberInfo> allowedMembers = new Dictionary<string, MemberInfo>();

        private MonoScript selectionScript;
        private string instanceGUID = "";
        //private MonoScript selectionScript;
        private int currentIndex = 0;

        private bool memberDataFilled = false;

        private const string allowedFieldsOption = "Fields";
        private const string allowedPropertiesOption = "Properties";
        private const string allowedMethodsOption = "Methods";

        private const string instanceGUIDTooltip= "The GUID of the object that has the selected Script. " +
            "You can easily get GUIDs by adding an ObjectID.cs component, click on 3 vertical dots on the component and select \"Generate guid for id\". " +
            "This is necessary so that when getting fields/properties/methods, it gets the values from the instance. Note: this means that when this value is used, that object with the script instance MUST be in the scene!";
        private const string targetScriptTooltip = "The Monoscript (script asset) that has the fields/properties/methods you want to use";
        private const string assemblyTooltip = "The Assembly that the type is in. You can check an assembly by locating that script, selecting it and checking the Assembly Information. " +
            "The assembly for that script will be under the Assembly Definition field. If not, create a new one by right clicking -> Create -> Assembly Definition";

        GUIStyle errorStyle = new GUIStyle();
        GUIStyle warningStyle= new GUIStyle();

        public override void OnInspectorGUI()
        {
            //serializedObject.Update();
            DrawDefaultInspector();
            MemberSelectionSO memberSelectionSO = (MemberSelectionSO)target;

            serializedObject.Update();

            errorStyle.normal.textColor = Color.red;
            warningStyle.normal.textColor = Color.yellow;

            this.allowFields = memberSelectionSO.AllowFields;
            this.allowProperties = memberSelectionSO.AllowProperties;
            this.allowMethods = memberSelectionSO.AllowMethods;

            if (memberSelectionSO.MemberType== UserSelectedType.Void)
            {
                memberSelectionSO.AllowFields = false;
                memberSelectionSO.AllowProperties = false;
                memberSelectionSO.AllowMethods = true;

                EditorGUILayout.LabelField("Note: Only methods are allowed for VOID types!", warningStyle);
            }

            if (memberSelectionSO.MemberType != UserSelectedType.Other) this.acceptedType = HelperFunctions.GetTypeFromUserSelectedType(memberSelectionSO.MemberType);
            else
            {
                Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) if (!assembly.GetName().Name.StartsWith("Unity") && !assembly.GetName().Name.StartsWith("System")) assemblies.Add(assembly.GetName().Name, assembly);

                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Type Assembly", GUILayout.Width(138));
                GenerateTooltip(assemblyTooltip);
                currentAssemblyIndex = EditorGUILayout.Popup(currentAssemblyIndex, assemblies.Keys.ToArray());
                GenerateTooltip(assemblyTooltip);
                EditorGUILayout.EndHorizontal();

                /*
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Type Name", GUILayout.Width(138));
                otherTypeName = EditorGUILayout.TextField(otherTypeName);
                EditorGUILayout.EndHorizontal();
                */
                otherTypeName= DrawTextField("Type Name", otherTypeName);

                try
                {
                    Assembly choosenAssembly = assemblies.GetDictionaryValueAtIndex(currentAssemblyIndex);
                    if (!string.IsNullOrEmpty(otherTypeName)) convertedType = choosenAssembly.GetType(otherTypeName, true, true);
                    else return;
                }
                catch (TypeLoadException e)
                {
                    UnityEngine.Debug.LogError($"Type {otherTypeName} does not exist in Assembly {Assembly.GetExecutingAssembly().FullName}! Make sure it is spelled correctly! Exception: {e}");
                    return;
                }

                acceptedType = convertedType;

                EditorGUI.indentLevel--;
            }


            EditorGUILayout.Space(20);
            selectionScript = EditorGUILayout.ObjectField("Target Script", selectionScript, typeof(MonoScript), allowSceneObjects: true) as MonoScript;
            GenerateTooltip(targetScriptTooltip);

            instanceGUID = DrawTextField("Target Instance GUID", instanceGUID);
            GenerateTooltip(instanceGUIDTooltip);

            if (selectionScript != null || !string.IsNullOrEmpty(instanceGUID))
            {
                EditorUtility.SetDirty(memberSelectionSO);
                //serializedObject.ApplyModifiedProperties();
                //serializedObject.Update();
            }
            if (selectionScript != null && !string.IsNullOrEmpty(instanceGUID))
            {
                //if we have already filled it out, we don't want it to get deleted, so we have this guard here
                //if (memberDataFilled) return;

                EditorUtility.SetDirty(memberSelectionSO);
                serializedObject.ApplyModifiedProperties();
                serializedObject.Update();

                ObjectID foundInstance = null;
                foundInstance= FindObjectsOfType<ObjectID>(true).Where(id => id.GetID() == instanceGUID).First();
                if (foundInstance==null)
                {
                    EditorGUILayout.LabelField($"The current scene does not have any GameObject with ObjectID.cs that has GUID: {instanceGUID}! " +
                        $"Make sure you copy and paste correctly and that the current open scene contains that instance!", errorStyle);
                    return;
                }

                Type classType = selectionScript.GetClass();
                if (!foundInstance.gameObject.TryGetComponent(classType, out Component foundClassInstance))
                {
                    EditorGUILayout.LabelField($"The script instance Object GUID: {instanceGUID} does not have a script with type: {classType}!",errorStyle);
                    return;
                }


                //Type classType = selectionScript.GetType();
                allowedMembers.Clear();

                if (!allowFields && !allowProperties && !allowMethods)
                {
                    EditorGUILayout.LabelField("No Allowed Member Type is Selected! Please set allowFields, allowProperties, and/or allowMethods to true!", errorStyle);
                    return;
                }

                UnityEngine.Debug.Log($"User selected type is: {acceptedType}");
                if (allowFields)
                {
                    foreach (var field in classType.GetFields())
                    {
                        //If the member is publicly gettable, if there is a type restriction- it meets it, and is not currently added, we add to allowed members
                        if (field.IsPublic &&
                            ((memberSelectionSO.MemberType != UserSelectedType.None && field.FieldType == acceptedType) || memberSelectionSO.MemberType == UserSelectedType.None) &&
                            !allowedMembers.ContainsKey($"{allowedFieldsOption}/{field.Name}"))
                            allowedMembers.Add($"{allowedFieldsOption}/{field.Name}", new MemberInfo(field.Name, foundClassInstance, field));
                    }
                }
                if (allowProperties)
                {
                    foreach (var property in classType.GetProperties())
                    {
                        //If the member is publicly gettable, if there is a type restriction- it meets it, and is not currently added, we add to allowed members
                        if (property.IsPubliclyGettable() && property.CanRead &&
                            ((memberSelectionSO.MemberType != UserSelectedType.None && property.PropertyType == acceptedType) || memberSelectionSO.MemberType == UserSelectedType.None) &&
                            !allowedMembers.ContainsKey($"{allowedPropertiesOption}/{property.Name}"))
                            allowedMembers.Add($"{allowedPropertiesOption}/{property.Name}", new MemberInfo(property.Name, foundClassInstance, property));
                    }
                }
                if (allowMethods)
                {
                    foreach (var method in classType.GetMethods())
                    {
                        //If the member is publicly gettable, if there is a type restriction- it meets it, and is not currently added, we add to allowed members
                        //We make sure that we do not get any of the automatic get and set methods for properties
                        if (method.IsPublic && !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_") && method.GetParameters().Length == 0 && 
                            ((memberSelectionSO.MemberType != UserSelectedType.None && method.ReturnType == acceptedType) || memberSelectionSO.MemberType == UserSelectedType.None) &&
                            !allowedMembers.ContainsKey($"{allowedMethodsOption}/{method.Name}()"))
                            allowedMembers.Add($"{allowedMethodsOption}/{method.Name}()", new MemberInfo(method.Name, foundClassInstance, method));
                    }
                }



                if (allowedMembers.Count > 0)
                {
                    currentIndex = EditorGUILayout.Popup(currentIndex, allowedMembers.Keys.ToArray());
                    memberSelectionSO.SelectedMemberInfo = allowedMembers.GetDictionaryValueAtIndex(currentIndex);

                    memberDataFilled = true;
                }
                else
                {
                    EditorGUILayout.LabelField($"No Public " + (allowFields ? $"{allowedFieldsOption}" : "") + (allowProperties ? $" {allowedPropertiesOption}" : "") + (allowMethods ? $" {allowedMethodsOption}" : "") +
                        $" with type {acceptedType.Name.ToUpper()} in {selectionScript.name}!", errorStyle);
                }
            }

            EditorUtility.SetDirty(target);
            serializedObject.ApplyModifiedProperties();
            Undo.RecordObject(target, target.name);
        }
    }
}
