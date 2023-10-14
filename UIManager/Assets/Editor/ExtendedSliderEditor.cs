using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Game.UI.EditorExtension
{
    [CustomEditor(typeof(ExtendedSlider))]
    public class ExtendedSliderEditor : ExtendedEditor
    {
        private bool setMinAndMaxFromMember;
        private bool setDefaultValueFromMember;

        private MemberSelectionSO minMaxMemberData;
        private MemberSelectionSO defaultValueMemberData;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ExtendedSlider extendedSlider = (ExtendedSlider)target;

            DrawHeader("Values From Methods", true);
            setMinAndMaxFromMember = EditorGUILayout.Toggle("Set Min/Max From Member", setMinAndMaxFromMember);
            if (setMinAndMaxFromMember)
            {
                minMaxMemberData = EditorGUILayout.ObjectField("Min/Max Data", minMaxMemberData, typeof(MemberSelectionSO), allowSceneObjects: true) as MemberSelectionSO;
                if (minMaxMemberData != null)
                {
                    Vector2 minMaxData;
                    MemberInfo currentMemberData = minMaxMemberData.SelectedMemberInfo;
                    if (minMaxMemberData.MemberType!= UserSelectedType.Vector2)
                    {
                        DrawInspectorError("Min/Max Member Data must have VECTOR2 Type!");
                        return;
                    }

                    if (currentMemberData.HasFieldData(out Object fieldValue)) minMaxData = (Vector2)fieldValue;
                    else if (currentMemberData.HasPropertyData(out Object propertyValue)) minMaxData = (Vector2)propertyValue;
                    else minMaxData = (Vector2)currentMemberData.InvokeMethod();

                    extendedSlider.SetSliderMinAndMax(minMaxData);
                }
            }

            setDefaultValueFromMember = EditorGUILayout.Toggle("Set Default Value From Member", setDefaultValueFromMember);
            if (setDefaultValueFromMember)
            {
                defaultValueMemberData = EditorGUILayout.ObjectField("Default Value Data", minMaxMemberData, typeof(MemberSelectionSO), allowSceneObjects: true) as MemberSelectionSO;
                if (defaultValueMemberData != null)
                {
                    float defaultValue;
                    MemberInfo currentMemberData = defaultValueMemberData.SelectedMemberInfo;
                    if (minMaxMemberData.MemberType != UserSelectedType.Float || minMaxMemberData.MemberType != UserSelectedType.Int)
                    {
                        DrawInspectorError("Default Value Member Data must have FLOAT or INT type!");
                        return;
                    }

                    //We can have int/float, but floats can be turned into ints if needed, but not vice versa, we we caste all to floats
                    if (currentMemberData.HasFieldData(out Object fieldValue)) defaultValue = (float)fieldValue;
                    else if (currentMemberData.HasPropertyData(out Object propertyValue)) defaultValue = (float)propertyValue;
                    else defaultValue = (float)currentMemberData.InvokeMethod();

                    extendedSlider.SetSliderValue(defaultValue);
                }
            }
        }
    }
}

