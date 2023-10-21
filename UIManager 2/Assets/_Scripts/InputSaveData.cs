using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Input
{
    /// <summary>
    /// Data containing info about the input binding and action used for saving Input Bindings and restoring data between sessions
    /// </summary>
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

