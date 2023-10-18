using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UI
{
    [System.Serializable]
    public class InputSaveData
    {
        [field: SerializeField] public string ActionName { get; private set; }
        [field: SerializeField] public InputBinding InputBinding { get; private set; }

        public InputSaveData(string actionName, InputBinding binding)
        {
            this.ActionName = actionName;
            this.InputBinding = binding;
        }
    }
}

