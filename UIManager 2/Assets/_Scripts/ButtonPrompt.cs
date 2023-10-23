using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using Game.Input;

namespace Game.UI
{
    /// <summary>
    /// When you want to make a button prompt, this script will manage the prompt. If you want multiple (such as in different locations, etc.), 
    /// you can create multiple prefabs of the ButtonPrompt prefab.
    /// If you want one, and only one spot, create only one and update the preset when you need to
    /// </summary>
    public class ButtonPrompt : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The container that holds all the UI elements for a UI system. This can be beneficial to allow the parent class to still be running while the container is disabled")]
        [SerializeField] private GameObject container;
        [SerializeField] private TextMeshProUGUI messageText1;
        [SerializeField] private TextMeshProUGUI messageText2;
        [Tooltip("The Images that will display the prompt icons. " +
            "Usually this should be 1, 2 (positive/negative), or 4 (left/right/down/up composite)")]
        [SerializeField] private Image[] images;

        [Header("Options")]
        [Tooltip("Preset set on start")][SerializeField] private ButtonPromptPresetSO defaultPreset;
        [Tooltip("The unique location of the prompt on the screen used to identify which prompt the ButtonPromptManager should enable")]
        [SerializeField] private ButtonPromptType promptType;
        public ButtonPromptType PromptType { get => promptType; }

        [Header("Target Pressed")]
        [SerializeField] private bool disableOnTargetButtonPressed = false;
        [SerializeField] private UnityEvent OnTargetButtonPressed;

        private bool isEnabled = false;

        private void Start()
        {
            //set all images to have the sprite null so that when we switch alpha, only the null ones are set
            foreach (var image in images) image.sprite = null;

            container.SetActive(false);
        }

        private void OnDestroy()
        {
            container.SetActive(false);
            StopAllCoroutines();
        }

        private void OnDisable()
        {
            DisableButtonPromptMessage();
        }

        /// <summary>
        /// Enables the preset set by the argument
        /// </summary>
        /// <param name="preset"></param>
        public void EnableButtonPromptMessage(ButtonPromptPresetSO preset)
        {
            //UnityEngine.Debug.Log($"Enable button prompt called on {gameObject.name}");
            gameObject.SetActive(true);
            isEnabled = true;

            if (messageText1 != null) messageText1.text = preset.Message1;
            if (messageText2 != null) messageText2.text = preset.Message2;

            //we get the binding or bindings that correspond to this first action
            InputBinding[] bindingPaths = InputManager.Instance.GetFirstFullPathCompositeOrBindingFromActionName(preset.InputAction.name);
            UnityEngine.Debug.Log($"Binding paths found: {bindingPaths.Length}");
            if (bindingPaths.Length > images.Length)
            {
                UnityEngine.Debug.LogError($"Tried to fit binding paths: {bindingPaths.Length} in {images.Length} button prompt images!");
                return;
            }
            else
            {
                //set all images to not have alpha and make the sprite null so that when we switch alpha, only the null ones are set
                foreach (var image in images)
                {
                    image.sprite = null;
                    image.gameObject.SetActive(false);
                }

                List<Sprite> sprites = InputManager.Instance.GetIconsFromAction(preset.InputAction.action);
                if(sprites.Count==0)
                {
                    UnityEngine.Debug.LogError($"Tried to assign the sprites for {typeof(ButtonPrompt)} {gameObject.name} but the sprite list from {typeof(InputAction)} {preset.InputAction.action.name} is 0!");
                    return;
                }

                for (int i=0; i< sprites.Count; i++)
                {
                    images[i].sprite = sprites[i];
                    images[i].gameObject.SetActive(true);
                }

                /*
                //we can have a composite, so we make sure we go through each path
                for (int i = 0; i < bindingPaths.Length; i++)
                {
                    Sprite sprite = InputManager.Instance.GetIconFromBinding(bindingPaths[i]);
                    if (sprite != null)
                    {
                        
                    }
                    else UnityEngine.Debug.LogError($"Tried to assign button message sprite for binding path:{bindingPaths[i].name} but it was not found!");
                }
                */
            }

            container.SetActive(true);

            InputManager.Instance.InputAsset[preset.InputAction.name].performed += TargetActionTriggered;
        }

        /// <summary>
        /// Enables this <see cref="ButtonPrompt"/> default/start preset: <see cref="ButtonPrompt.defaultPreset"/>. If this does not exist or is null, it will throw an error
        /// </summary>
        public void EnableButtonPromptMessage()
        {
            if (defaultPreset==null)
            {
                UnityEngine.Debug.LogError($"Tried to call EnableButtonPromptMessage() in {gameObject.name} with no ButtonPromptPresetSO argument, but this instance does not have a startPreset set up!");
                return;
            }
            EnableButtonPromptMessage(defaultPreset);
        }

        public void DisableButtonPromptMessage()
        {
            isEnabled = false;
            container.gameObject.SetActive(false);
        }

        private void TargetActionTriggered(InputAction.CallbackContext context)
        {
            OnTargetButtonPressed?.Invoke();
            InputManager.Instance.InputAsset[context.action.name].performed -= TargetActionTriggered;
            if (disableOnTargetButtonPressed) DisableButtonPromptMessage();
        }
    }

}


