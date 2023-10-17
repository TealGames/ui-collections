using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    [System.Serializable]
    public class InputSaveData
    {
        [field: SerializeField] public string ActionName { get; private set; }
        [field: SerializeField] public string Path { get; private set; }
        [field: SerializeField] public string OverridePath { get; private set; }

        public InputSaveData(string actionName, string path, string overridePath)
        {
            this.ActionName = actionName;
            this.Path = path;
            this.OverridePath = overridePath;
        }
    }
}

