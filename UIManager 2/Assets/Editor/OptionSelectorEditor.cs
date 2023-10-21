using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    [CustomEditor(typeof(OptionSelector))]
    internal class OptionSelectorEditor : Editor
    {
        private bool storeListFromEnum;

        private MonoScript enumScript;
        private int currentIndex = 0;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            OptionSelector optionSelector = (OptionSelector)target;

            storeListFromEnum = EditorGUILayout.Toggle("Find Options From Enum", storeListFromEnum);
            if (storeListFromEnum)
            {
                enumScript= EditorGUILayout.ObjectField("Enum Script", enumScript, typeof(MonoScript), allowSceneObjects: false) as MonoScript;
                if (enumScript != null)
                {
                    Dictionary<string, Type> enums = new Dictionary<string, Type>();
                    Type classType = enumScript.GetClass();
                    if (classType.IsEnum) enums.Add(classType.Name, classType);
                    else
                    {
                        foreach (var nestedType in classType.GetNestedTypes())
                        {
                            if (nestedType.IsEnum) enums.Add(nestedType.Name, nestedType);
                        }
                    }

                    if (enums.Count > 0)
                    {
                        currentIndex = EditorGUILayout.Popup(currentIndex, enums.Keys.ToArray());
                        List<string> selectedEnumValues= new List<string>();
                        foreach (var enumValue in Enum.GetValues(enums.GetDictionaryValueAtIndex<string, Type>(currentIndex))) 
                            selectedEnumValues.Add(enumValue.ToString());

                        optionSelector.AllOptionsProperty = selectedEnumValues;

                        GUIStyle style = new GUIStyle();
                        style.normal.textColor = Color.yellow;
                        EditorGUILayout.LabelField("*You can no longer edit \"AllOptions\"*", style);

                        //EditorGUI.BeginDisabledGroup(true);
                        //EditorGUILayout.PropertyField(serializedObject.FindProperty("AllOptions"), true);
                        //EditorGUI.EndDisabledGroup();

                        //serializedObject.ApplyModifiedProperties();
                    }
                    else
                    {
                        GUIStyle style = new GUIStyle();
                        style.normal.textColor = Color.red;
                        EditorGUILayout.LabelField("No Enums Were Found!", style);
                    }
                }
            }
        }
    }
}

