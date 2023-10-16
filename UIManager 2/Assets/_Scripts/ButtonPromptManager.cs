using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;
using static UnityEditor.MaterialProperty;

namespace Game.UI
{
    public class ButtonPromptManager : MonoBehaviour
    {
        public List<ButtonPrompt> buttonPrompts { get; private set; } = new List<ButtonPrompt>();

        // Start is called before the first frame update
        void Awake()
        {
            StoreButtonPrompts();
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void StoreButtonPrompts()
        {
            for (int i=0; i< transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<ButtonPrompt>(out ButtonPrompt prompt)) buttonPrompts.Add(prompt);
            }
        }

        public void EnableButtonPrompt(ButtonPromptType promptType, ButtonPromptPresetSO preset)
        {
            if (buttonPrompts.Count ==0)
            {
                UnityEngine.Debug.LogWarning($"Tried to enable a button prompt of type: {promptType}, but there are currently 0 children with parent {gameObject.name} that have a ButtonPrompt.cs script! " +
                    $"Add children with that script to the parent {gameObject.name} to be able to enable any button prompt from the type!");
                return;
            }
            FindPromptOfType(promptType).EnableButtonPromptMessage(preset);
        }

        public void DisableButtonPrompt(ButtonPromptType promptType)
        {
            if (buttonPrompts.Count == 0)
            {
                UnityEngine.Debug.LogWarning($"Tried to disable a button prompt of type: {promptType}, but there are currently 0 children with parent {gameObject.name} that have a ButtonPrompt.cs script! " +
                    $"Add children with that script to the parent {gameObject.name} to be able to enable any button prompt from the type!");
                return;
            }
            FindPromptOfType(promptType).DisableButtonPromptMessage();
        }

        private ButtonPrompt FindPromptOfType(ButtonPromptType type)
        {
            ButtonPrompt foundPrompt = null;
            foreach (var prompt in buttonPrompts)
            {
                if (prompt.PromptType == type)
                {
                    foundPrompt = prompt;
                    break;
                }
            }
            return foundPrompt;
        }
    }
}

