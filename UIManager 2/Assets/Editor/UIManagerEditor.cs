using System.Collections;
using System.Collections.Generic;
using UnityEditor.Compilation;
using UnityEditor;
using UnityEngine;
using TMPro;

namespace Game.UI
{
    [CustomEditor(typeof(UIManager))]
    internal class GameManagerEditor : Editor
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
                TextMeshProUGUI[] text = GameObject.FindObjectsOfType<TextMeshProUGUI>(true);
                foreach (var textObj in text) textObj.font = uiManager.OverrideFontAsset;
            }

            if (GUILayout.Button("Override Scene Tooltip Settings"))
            {
                if (uiManager.OverrideTooltipSettings==null)
                {
                    UnityEngine.Debug.LogError("Tried to override all scene tooltip component tooltip settings, but the TooltipSetting is NULL!");
                    return;
                }
                Tooltip[] tooltips= GameObject.FindObjectsOfType<Tooltip>(true);
                foreach (var tooltipObj in tooltips) tooltipObj.TooltipSettings = uiManager.OverrideTooltipSettings;
            }
        }

    }
}


