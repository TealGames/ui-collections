using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.UI.EditorExtension
{
    [CustomEditor(typeof(SettingsMenu))]
    internal class SettingsMenuEditor : ExtendedEditor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            SettingsMenu settingsMenu = (SettingsMenu)target;

            DrawHeader("Editor", true, topMargin: 10);
            if (GUILayout.Button("Delete Saved Settings Data"))
            {
                HelperFunctions.DeleteDirectory(SettingsMenu.PREFERENCES_PATH_TYPE, SettingsMenu.PREFERENCES_RELATIVE_PATH);
            }
            DrawLabel("Will delete the save file for the saved settings during runtime", Color.gray, isItalicized: true);
        }
    }
}

