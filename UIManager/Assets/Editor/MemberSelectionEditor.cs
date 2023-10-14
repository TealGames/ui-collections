using Game.Utilities;
using Game.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Game.UI.EditorExtension
{
    [CustomEditor(typeof(MemberSelectionSO))]
    public class MemberSelectionEditor : ExtendedEditor
    {
        public class PersistentInfo
        {
            public string AssemblyName { get; set; } = EditorHelperFunctions.GetDefaultAssemblyName();
            public int AssemblyIndex { get; set; } = -1;
            public Type Type { get; set; } = null;
            public MonoScript MonoScript { get; set; } = null;
            public string InstanceGUID { get; set; } = "";
            public int ChoosenMemberIndex { get; set;} = -1;

            public PersistentInfo() { }

            public PersistentInfo(string assemblyName, int assemblyIndex, Type type, MonoScript monoScript, string instanceGUID, int memberIndex)
            {
                this.AssemblyName = assemblyName;
                this.AssemblyIndex = assemblyIndex;
                this.Type = type;
                this.MonoScript = monoScript;
                this.InstanceGUID = instanceGUID;
                this.ChoosenMemberIndex = memberIndex;
            }

            public PersistentInfo(string assemblyName, int assemblyIndex, Type type, string instanceGUID, int memberIndex)
            {
                this.AssemblyName = assemblyName;
                this.AssemblyIndex = assemblyIndex;
                this.Type = type;
                this.InstanceGUID = instanceGUID;
                this.ChoosenMemberIndex = memberIndex;
            }

            public PersistentInfo(string assemblyName, int assemblyIndex, MonoScript monoScript, string instanceGUID, int memberIndex)
            {
                this.AssemblyName = assemblyName;
                this.AssemblyIndex = assemblyIndex;
                this.MonoScript = monoScript;
                this.InstanceGUID = instanceGUID;
                this.ChoosenMemberIndex = memberIndex;
            }

            /// <summary>
            /// Use this constructor if the Type and MonoScript are null
            /// </summary>
            /// <param name="assemblyName"></param>
            /// <param name="assemblyIndex"></param>
            /// <param name="instanceGUID"></param>
            /// <param name="memberIndex"></param>
            public PersistentInfo(string assemblyName, int assemblyIndex, string instanceGUID, int memberIndex)
            {
                this.AssemblyName = assemblyName;
                this.AssemblyIndex = assemblyIndex;
                this.InstanceGUID = instanceGUID;
                this.ChoosenMemberIndex = memberIndex;
            }

            public bool HasData() => AssemblyName != "" && AssemblyIndex != -1 && Type != null && MonoScript != null && InstanceGUID != "" && ChoosenMemberIndex != -1;

            public string GetDataAsString()
            {
                return $"{AssemblyName}/{AssemblyIndex}/{((Type!=null)? Type.Name : "null")}/{((MonoScript!=null)? MonoScript.name : "null")}/{InstanceGUID}/{ChoosenMemberIndex}";
            }
        }
        private static PersistentInfo persistentInfo = null;


        private bool allowFields = false;
        private bool allowProperties = false;
        private bool allowMethods = false;

        //private Type acceptedType = null;
        //private Type convertedType = null;
        //private string otherTypeName = "";
        //private int currentAssemblyIndex = 0;
        //Dictionary<string, MemberInfo> allowedMembers = new Dictionary<string, MemberInfo>();

        //private MonoScript selectionScript;
        //private string instanceGUID = "";
        //private int currentIndex = 0;

        private bool memberDataFilled = false;

        //MEMBER SELECTION PATH NAMES
        private const string ALLOWED_FIELD_OPTION = "Fields";
        private const string ALLOWED_PROPERTIES_OPTION = "Properties";
        private const string ALLOWED_METHODS_OPTION = "Methods";

        //TOOLTIPS
        private const string INSTANCE_GUID_TOOLTIP= "The GUID of the object that has the selected Script. " +
            "You can easily get GUIDs by adding an ObjectID.cs component, click on 3 vertical dots on the component and select \"Generate guid for id\". " +
            "This is necessary so that when getting fields/properties/methods, it gets the values from the instance. Note: this means that when this value is used, that object with the script instance MUST be in the scene!";
        private const string TARGET_SCRIPT_TOOLTIP = "The Monoscript (script asset) that has the fields/properties/methods you want to use";
        private const string ASSEMBLY_TOOLTIP = "The Assembly that the type is in. You can check an assembly by locating that script, selecting it and checking the Assembly Information. " +
            "The assembly for that script will be under the Assembly Definition field. If not, create a new one by right clicking -> Create -> Assembly Definition";
        private const string customTypeTooltip = "The custom type as a string. Make sure it is spelled correctly (types are case sensitive!)";

        private readonly string separator = HelperFunctions.DATA_SEPARATOR;


        public override void OnInspectorGUI()
        {
            //serializedObject.Update();
            DrawDefaultInspector();
            MemberSelectionSO memberSelectionSO = (MemberSelectionSO)target;
            serializedObject.Update();
            TryGetInfo();
            UnityEngine.Debug.Log("Editor has been redrawn!");

            this.allowFields = (memberSelectionSO.AttributeType & AttributeRestrictionType.Field) != 0;
            this.allowProperties = (memberSelectionSO.AttributeType & AttributeRestrictionType.Property) != 0;
            this.allowMethods = (memberSelectionSO.AttributeType & AttributeRestrictionType.Method) != 0;

            if (!allowFields && !allowProperties && !allowMethods)
            {
                UnityEngine.Debug.LogError("There are no attribute types choosen! Methods are automatically set!");
                memberSelectionSO.AttributeType = AttributeRestrictionType.Method;
                this.allowFields = false;
                this.allowProperties = false;
                this.allowMethods = true;
            }

            if (memberSelectionSO.MemberType== UserSelectedType.Void)
            {
                memberSelectionSO.AttributeType = AttributeRestrictionType.Method;
                DrawInspectorWarning("Note: Only methods are allowed for VOID types!");
            }

            if (memberSelectionSO.MemberType != UserSelectedType.Other) persistentInfo.Type = HelperFunctions.GetTypeFromUserSelectedType(memberSelectionSO.MemberType);
            else
            {
                Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) if (!assembly.GetName().Name.StartsWith("Unity") && !assembly.GetName().Name.StartsWith("System")) assemblies.Add(assembly.GetName().Name, assembly);

                EditorGUI.indentLevel++;
                GUIStyle style= new GUIStyle(EditorStyles.boldLabel);
                //EditorGUILayout.LabelField("Type Info", GUILayout.Width(138));
                DrawHeader("Type Info", true, 5, 0);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Type Assembly", GUILayout.Width(138));
                GenerateTooltip(ASSEMBLY_TOOLTIP);
                persistentInfo.AssemblyIndex = EditorGUILayout.Popup(persistentInfo.AssemblyIndex, assemblies.Keys.ToArray());
                SaveInfo();
                GenerateTooltip(ASSEMBLY_TOOLTIP);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Type Name", GUILayout.Width(138));
                string otherTypeName = "";
                otherTypeName = EditorGUILayout.TextField(otherTypeName);
                if (otherTypeName.Length > 0 && otherTypeName.Contains(" ")) otherTypeName.Replace(" ", "");
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel--;

                Type convertedType = null;
                Assembly choosenAssembly = assemblies.GetDictionaryValueAtIndex(persistentInfo.AssemblyIndex);
                try
                {
                    Assembly.Load(choosenAssembly.GetName());
                    foreach (var referencedAssembly in choosenAssembly.GetReferencedAssemblies()) Assembly.Load(referencedAssembly);
                    if (!string.IsNullOrEmpty(otherTypeName)) convertedType = choosenAssembly.GetType(otherTypeName, true, false);
                    else return;
                }
                catch (TypeLoadException e)
                {
                    UnityEngine.Debug.LogError($"Type {otherTypeName} does not exist in Assembly {choosenAssembly.GetName().Name}! Make sure it is spelled correctly! Exception: {e}");
                    return;
                }

                persistentInfo.Type = convertedType;
                SaveInfo();
            }


            EditorGUILayout.Space(20);
            persistentInfo.MonoScript = EditorGUILayout.ObjectField("Target Script", persistentInfo.MonoScript, typeof(MonoScript), allowSceneObjects: true) as MonoScript;
            SaveInfo();
            //MonoScript selectionScript = MonoScript.FromMonoBehaviour(persistentInfo.MonoScript);
            GenerateTooltip(TARGET_SCRIPT_TOOLTIP);

            persistentInfo.InstanceGUID = DrawTextField("Target Instance GUID", persistentInfo.InstanceGUID);
            SaveInfo();
            GenerateTooltip(INSTANCE_GUID_TOOLTIP);

            if (persistentInfo.MonoScript != null && !string.IsNullOrEmpty(persistentInfo.InstanceGUID))
            {
                //if we have already filled it out, we don't want it to get deleted, so we have this guard here
                //if (memberDataFilled) return;

                ObjectID foundInstance = null;
                ObjectID[] objectIDs = FindObjectsOfType<ObjectID>(true);
                if (objectIDs.Length==0)
                {
                    UnityEngine.Debug.LogError("There are no gameObjects in the current open scene that have an ObjectID script! " +
                        "Instances of scripts can only be found via a unique object GUID which can only be assigned if an object has the class 'ObjectID'");
                    return;
                }

                foundInstance= objectIDs.Where(id => id.GetID() == persistentInfo.InstanceGUID)?.FirstOrDefault();
                if (foundInstance==default)
                {
                    DrawInspectorError($"The current scene does not have any GameObject with ObjectID.cs that has GUID: {persistentInfo.InstanceGUID}! " +
                        $"Make sure you copy and paste correctly and that the current open scene contains that instance!");
                    return;
                }

                Type classType = persistentInfo.MonoScript.GetClass();
                if (!foundInstance.gameObject.TryGetComponent(classType, out Component foundClassInstance))
                {
                    UnityEngine.Debug.LogError($"The script instance Object GUID: {persistentInfo.InstanceGUID} does not have a script with type: {classType}!");
                    DrawInspectorError($"That object does not have a script of type: {classType}!");
                    return;
                }


                //Type classType = selectionScript.GetType();
                //allowedMembers.Clear();
                Dictionary<string, MemberInfo> allowedMembers = new Dictionary<string, MemberInfo>();

                if (!allowFields && !allowProperties && !allowMethods)
                {
                    DrawInspectorError("No Allowed Member Type is Selected! Please set allowFields, allowProperties, and/or allowMethods to true!");
                    return;
                }

                UnityEngine.Debug.Log($"User selected type is: {persistentInfo.Type}");
                if (allowFields)
                {
                    foreach (var field in classType.GetFields())
                    {
                        //If the member is publicly gettable, if there is a type restriction- it meets it, and is not currently added, we add to allowed members
                        if (field.IsPublic &&
                            field.FieldType == persistentInfo.Type && !allowedMembers.ContainsKey($"{ALLOWED_FIELD_OPTION}/{field.Name}"))
                            allowedMembers.Add($"{ALLOWED_FIELD_OPTION}/{field.Name}", new MemberInfo(field.Name, foundClassInstance, field));
                    }
                }
                if (allowProperties)
                {
                    foreach (var property in classType.GetProperties())
                    {
                        //If the member is publicly gettable, if there is a type restriction- it meets it, and is not currently added, we add to allowed members
                        if (property.IsPubliclyGettable() && property.CanRead && property.PropertyType == persistentInfo.Type
                            && !allowedMembers.ContainsKey($"{ALLOWED_PROPERTIES_OPTION}/{property.Name}"))
                            allowedMembers.Add($"{ALLOWED_PROPERTIES_OPTION}/{property.Name}", new MemberInfo(property.Name, foundClassInstance, property));
                    }
                }
                if (allowMethods)
                {
                    foreach (var method in classType.GetMethods())
                    {
                        //If the member is publicly gettable, if there is a type restriction- it meets it, and is not currently added, we add to allowed members
                        //We make sure that we do not get any of the automatic get and set methods for properties
                        if (method.IsPublic && !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_") && method.GetParameters().Length == 0 && 
                            method.ReturnType == persistentInfo.Type && !allowedMembers.ContainsKey($"{ALLOWED_METHODS_OPTION}/{method.Name}()"))
                            allowedMembers.Add($"{ALLOWED_METHODS_OPTION}/{method.Name}()", new MemberInfo(method.Name, foundClassInstance, method));
                    }
                }

                if (allowedMembers.Count > 0)
                {
                    persistentInfo.ChoosenMemberIndex = EditorGUILayout.Popup(persistentInfo.ChoosenMemberIndex, allowedMembers.Keys.ToArray());
                    //memberSelectionSO.SelectedMemberInfo = allowedMembers.GetDictionaryValueAtIndex(persistentInfo.ChoosenMemberIndex);

                    //Here we save the Assembly name, Object ID, the script type, the member type and the name of the member
                    string saveString = $"{persistentInfo.AssemblyName}/{persistentInfo.InstanceGUID}/{persistentInfo.MonoScript.GetClass()}/{memberSelectionSO.AttributeType}/{allowedMembers.GetDictionaryValueAtIndex(persistentInfo.ChoosenMemberIndex).Name}";
                    HelperFunctions.SaveDataInFile(target.GetInstanceID().ToString() + separator + MemberSelectionSO.MEMBER_INFO_FILE_NAME, saveString);

                    memberDataFilled = true;
                    SaveInfo();
                }
                else
                {
                    DrawInspectorError($"No Public " + (allowFields ? $"{ALLOWED_FIELD_OPTION}" : "") + (allowProperties ? $" {ALLOWED_PROPERTIES_OPTION}" : "") + (allowMethods ? $" {ALLOWED_METHODS_OPTION}" : "") + 
                          $" with type {persistentInfo.Type.Name.ToUpper()} in {persistentInfo.MonoScript.name}!");
                }
            }

            //EditorUtility.SetDirty(target);
            //serializedObject.ApplyModifiedProperties();
            //Undo.RecordObject(target, target.name);
        }

        public void TryGetInfo()
        {
            string infoAsString = EditorPrefs.HasKey(target.GetInstanceID().ToString()) ? EditorPrefs.GetString(target.GetInstanceID().ToString()) : "";

            if (infoAsString == "")
            {
                //If the data does not exist in preferences, but is stored in a file (meaning we reopened the editor after closing so the prefs was cleared, but we still have the data)
                //Then, we can just get it here from the file
                string loadedData = HelperFunctions.LoadDataFromFile(target.GetInstanceID().ToString()+separator+ MemberSelectionSO.SO_FILE_NAME);
                if (!loadedData.Equals(""))
                {
                    UnityEngine.Debug.Log("Info not saved in EditorPrefs, found data in file instead! (can occur on loading data which is set after relaunching the editor)");
                    infoAsString = loadedData;
                }

                //If we have not found the data in EditorPrefs nor in files, we create a new set of info
                else
                {
                    UnityEngine.Debug.Log("Could not find instance id, new info created!");
                    persistentInfo = new PersistentInfo();
                    return;
                }
            }
            List<string> data = infoAsString.Split(separator).ToList();
            //Reference: (0) assemblyName, (1) assemblyIndex, (2) type, (3) monoScript, (4) instanceGUID, (5) memberIndex

            List<Assembly> assemblies = EditorHelperFunctions.GetAssemblies();
            Assembly foundAssembly = null;
            MonoScript foundScript = null;

            //If both the type and the monoscript are not found, we create the object without them
            if (data[2].ToLower().Equals("null") && data[3].ToLower().Equals("null"))
            {
                persistentInfo = new PersistentInfo(data[0], Convert.ToInt32(data[1]), data[4], Convert.ToInt32(data[5]));
                return;
            }

            //Since the type is null, we get the monoscript
            else if (data[2].ToLower().Equals("null"))
            {
                foundScript = EditorHelperFunctions.FindMonoScriptByName(data[3]);
                persistentInfo = new PersistentInfo(data[0], Convert.ToInt32(data[1]), foundScript, data[4], Convert.ToInt32(data[5]));
                return;
            }
            
            //Since the monoscript is null, we get the type
            else if (data[3].ToLower().Equals("null"))
            {
                UnityEngine.Debug.Log($"Trying to get assembly name {data[0]}");
                foundAssembly = assemblies.Where(assembly => assembly.GetName().Name == data[0]).First();
                persistentInfo = new PersistentInfo(data[0], Convert.ToInt32(data[1]), foundAssembly.GetType(data[2]), data[4], Convert.ToInt32(data[5]));
                return;
            }

            foundAssembly = assemblies.Where(assembly => assembly.GetName().Name == data[0]).First();
            foundScript = EditorHelperFunctions.FindMonoScriptByName(data[3]);
            persistentInfo = new PersistentInfo(data[0], Convert.ToInt32(data[1]), foundAssembly.GetType(data[2]), foundScript, data[4], Convert.ToInt32(data[5]));
        }

        public void SaveInfo()
        {
            if (EditorPrefs.HasKey(target.GetInstanceID().ToString())) EditorPrefs.DeleteKey(target.GetInstanceID().ToString());
            EditorPrefs.SetString(target.GetInstanceID().ToString(), persistentInfo.GetDataAsString());
            HelperFunctions.SaveDataInFile(target.GetInstanceID().ToString()+separator +MemberSelectionSO.SO_FILE_NAME, persistentInfo.GetDataAsString());
        }
    }
}
