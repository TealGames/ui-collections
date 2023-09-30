using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEditor;
using UnityEngine;
using TMPro;

namespace Game.UI
{
    [CustomEditor(typeof(UIManager))]
    public class GameManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            UIManager uiManager = (UIManager)target;

            if (GUILayout.Button("Replace Scene TMPro Text With Override"))
            {
                if (uiManager.OverrideFontAsset==null)
                {
                    UnityEngine.Debug.LogError("Tried to replace All Scene TMPro Text, but override font is NULL!");
                    return;
                }
                TextMeshProUGUI[] text = GameObject.FindObjectsOfType<TextMeshProUGUI>();
                foreach (var textObj in text) textObj.font = uiManager.OverrideFontAsset;
            }
        }

    }
}


