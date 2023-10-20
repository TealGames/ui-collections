using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// A preset for a global text object
    /// </summary>
    [CreateAssetMenu(fileName = "TextPresetSO", menuName = "ScriptableObjects/Global Text Preset")]
    public class TextPresetSO : ScriptableObject
    {
        [field: SerializeField] public string TextTypeName { get; private set; }
        [field: SerializeField] public float EnableTime { get; private set; }
        [field: SerializeField] public float DisableTime { get; private set; }

    }
}

