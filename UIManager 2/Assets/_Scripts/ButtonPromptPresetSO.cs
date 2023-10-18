using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UI
{
    /// <summary>
    /// Used to create <see cref="ButtonPrompt"/> message instances. Each instance is a unique prompt that can be set with the <see cref="ButtonPrompt"/> script
    /// </summary>
    [CreateAssetMenu(fileName = "ButtonPromptPresetSO", menuName = "ScriptableObjects/Button Prompt Preset")]
    public class ButtonPromptPresetSO : ScriptableObject
    {
        [Tooltip("The first message, before the binding icons of the prompt. Leave blank if there is no text here")]
        [TextArea(2, 3)][SerializeField] private string message1 = "";
        public string Message1 { get => message1; }

        [field: SerializeField] public InputActionReference InputAction { get; private set; }

        [Tooltip("The second message, after the binding icons of the prompt. Leave blank if there is no text here")]
        [TextArea(2, 3)][SerializeField] private string message2 = "";
        public string Message2 { get => message2; } 
    }
}
