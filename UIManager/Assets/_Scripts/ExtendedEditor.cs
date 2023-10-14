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

    protected void DrawHeader(string headerLabel, bool bold, float topMargin= 20, float lowerMargin= 5)
    {
        EditorGUILayout.Space(topMargin);
        GUIStyle headerStyle = new GUIStyle();
        if (bold) headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;
        EditorGUILayout.LabelField(headerLabel,headerStyle);
        EditorGUILayout.Space(lowerMargin);
    }

    /// <summary>
    /// Will draw a text label with the error message in a red color
    /// </summary>
    /// <param name="errorMessage"></param>
    protected void DrawInspectorError(string errorMessage)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.red;
        EditorGUILayout.LabelField(errorMessage, style);
    }

    /// <summary>
    /// Will draw a text lavel with the warning message in a yellow color
    /// </summary>
    /// <param name="warningMessage"></param>
    protected void DrawInspectorWarning(string warningMessage)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.yellow;
        EditorGUILayout.LabelField(warningMessage, style);
    }

    /// <summary>
    /// Will draw a text lavel with the message in a plain white color
    /// </summary>
    /// <param name="message"></param>
    protected void DrawPlainLabel(string message)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.gray;
        EditorGUILayout.LabelField(message, style);
    }

    /// <summary>
    /// Will draw a text lavel with the message in the color choosen
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    protected void DrawLabel(string message, Color color)
    {
        GUIStyle style = new GUIStyle();
        style.normal.textColor = color;
        EditorGUILayout.LabelField(message, style);
    }
}
