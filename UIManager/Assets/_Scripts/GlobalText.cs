using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class GlobalText : MonoBehaviour
    {
        [System.Serializable]
        public class GlobalTextData
        {
            [field: SerializeField] public TextMeshProUGUI TextObject { get; private set; }
            [field: SerializeField] public TextPresetSO UniquePreset { get; private set; }
        }
        [Tooltip("The text object with unique data pairs. The UniquePreset must be unique to the text so it can be used as identification when finding the text")]
        [SerializeField] private GlobalTextData[] globalTextData;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnValidate()
        {
            foreach (var data in globalTextData)
            {
                int matchingPresets = globalTextData.Where(dataSearch => dataSearch.UniquePreset == data.UniquePreset).ToList().Count();
                if (matchingPresets > 0)
                {
                    UnityEngine.Debug.LogError($"Global text preset: {data.UniquePreset.TextTypeName} has {matchingPresets} other global text data that have the same preset! " +
                        $"Presets are used to identify the text, so each global text has to have a unique preset!");
                    break;
                }
            }


        }

        public void EnableText()
        {

        }
    }
}

