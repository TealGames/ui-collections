using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Triggers a <see cref="ButtonPrompt"/> when a <see cref="Collider"/> enters the <see cref="ButtonPromptTrigger3D"/> <see cref="Collider"/>
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ButtonPromptTrigger3D : Trigger3D
    {
        [Header("Prompt Trigger 2D")]
        [Tooltip("The preset data that will be triggered when the target object enters the collider")]
        [SerializeField] private ButtonPromptPresetSO triggeredPreset;
        [SerializeField] private ButtonPromptType promptType;

        private ButtonPromptManager promptManager = null;

        // Start is called before the first frame update
        void Start()
        {
            promptManager = UIManager.Instance.GetComponentInChildren<ButtonPromptManager>(true);
            if (promptManager == null) 
                UnityEngine.Debug.LogError($"{typeof(ButtonPromptTrigger3D)} named {gameObject.name} tried to find the {typeof(ButtonPromptManager)} in the {typeof(UIManager)} children, but it does not exist! " +
                    $"You can only use {typeof(ButtonPromptTrigger3D)} if a {typeof(ButtonPromptManager)} exists!");
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider collider)
        {
            if (promptManager == null) return;

            base.OnEnter(collider);
            promptManager.EnableButtonPrompt(promptType, triggeredPreset);
        }

        private void OnTriggerExit(Collider collider)
        {
            if (promptManager == null) return;
              
            base.OnExit(collider);
            promptManager.DisableButtonPrompt(promptType);
        }
    }
}
