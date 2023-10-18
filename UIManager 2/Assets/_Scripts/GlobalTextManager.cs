using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Manages the global text gameObjects in order to allow for more consistent text across the game and allowing for more customizability while maintaining universal settings
    /// </summary>
    [ExecuteAlways]
    public class GlobalTextManager : MonoBehaviour
    {
        [System.Serializable]
        public class GlobalTextData
        {
            [field: SerializeField] public TextMeshProUGUI TextObject { get; private set; }
            [field: SerializeField] public TextPresetSO UniquePreset { get; private set; }
        }
        [Tooltip("The text object with unique data pairs. The UniquePreset must be unique to the text so it can be used as identification when finding the text")]
        [SerializeField] private GlobalTextData[] globalTextData;

        [Header("Options")]
        [Tooltip("If true, the text object name will be the unique preset's name plus the suffix")]
        [SerializeField] private bool matchTextObjectToPresetName;
        [Tooltip("If matchTextObjectToPresetName is true, the text object name will be the unique preset's name plus this suffix")]
        [SerializeField] private string textObjectNameSuffix = "Text";

        // Start is called before the first frame update
        void Start()
        {
            if (globalTextData != null && Application.isPlaying)
            {
                foreach (var data in globalTextData) 
                    data.TextObject.color = new Color(data.TextObject.color.r, data.TextObject.color.g, data.TextObject.color.b, 0f);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (globalTextData.Length > 0 && !Application.isPlaying)
            {
                foreach (var data in globalTextData)
                {
                    if (matchTextObjectToPresetName && textObjectNameSuffix != "") 
                        data.TextObject.gameObject.name = $"{data.UniquePreset.TextTypeName} {textObjectNameSuffix}";
                }
            }
        }

        private void OnValidate()
        {
            if (globalTextData.Length>0 && !Application.isPlaying)
            {
                foreach (var data in globalTextData)
                {
                    if(data.UniquePreset!=null)
                    {
                        int matchingPresets = globalTextData.Where(dataSearch => dataSearch.UniquePreset == data.UniquePreset).ToList().Count();
                        if (matchingPresets > 1)
                        {
                            UnityEngine.Debug.LogError($"Global text preset: {data.UniquePreset.TextTypeName} has {matchingPresets} other global text data that have the same preset! " +
                                $"Presets are used to identify the text, so each global text has to have a unique preset!");
                            break;
                        }
                    }
                }
            }
        }

        public void EnableText(string text, TextPresetSO textPreset)
        {
            GlobalTextData data = GetDataFromPreset(textPreset);
            if (data!=null)
            {
                data.TextObject.text = text;
                UIManager.Instance.LerpTextAlpha(data.TextObject, data.UniquePreset.EnableTime, 1f);
            }
        }

        public void DisableText(TextPresetSO textPreset)
        {
            GlobalTextData data = GetDataFromPreset(textPreset);
            if (data!=null) UIManager.Instance.LerpTextAlpha(data.TextObject, data.UniquePreset.DisableTime, 0f);
        }

        private GlobalTextData GetDataFromPreset(TextPresetSO textPreset)
        {
            GlobalTextData data = Array.Find(globalTextData, data => data.UniquePreset == textPreset);
            if (data == null) 
                UnityEngine.Debug.LogError($"There is no text preset in {gameObject.name} that matches the preset with TextTypeName {textPreset.TextTypeName}!");
            return data;
        }
    }
}
