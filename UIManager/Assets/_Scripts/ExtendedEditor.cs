using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExtendedEditor : Editor
{
    protected void GenerateTooltip(string text)
    {
        var propRect = GUILayoutUtility.GetLastRect();
        GUI.Label(propRect, new GUIContent("", text));
    }

    protected string DrawTextField(string labelText, string textAreaVariable)
    {
        string newText = "";
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(labelText, GUILayout.Width(138));
        newText = EditorGUILayout.TextField(textAreaVariable);
        EditorGUILayout.EndHorizontal();

        return newText;
    }

    protected void DrawHeader(string headerLabel)
    {
        EditorGUILayout.Space(20);
        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white; 
        EditorGUILayout.LabelField(headerLabel,headerStyle);
        EditorGUILayout.Space(5);
    }

    protected void DrawInspectorError(string errorMessage)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        EditorGUILayout.LabelField(errorMessage, style);
    }
}
