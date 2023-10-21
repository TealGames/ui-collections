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
        [SerializeField] private GameObject container;
        [SerializeField] private TextMeshProUGUI messageText1;
        [SerializeField] private TextMeshProUGUI messageText2;
        [SerializeField] private Image[] images;

        [Header("Options")]
        [Tooltip("Preset set on start")][SerializeField] private ButtonPromptPresetSO startPreset;
        [field: SerializeField] public ButtonPromptType PromptType { get; private set; }
        //[SerializeField] private float textHorizontalOffsetFromIcon;


        [Header("Target Pressed")]
        [SerializeField] private bool disableOnTargetButtonPressed = false;
        [SerializeField] private UnityEvent OnTargetButtonPressed;

        private bool isEnabled = false;

        private void Start()
        {
            //set all images to have the sprite null so that when we switch alpha, only the null ones are set
            foreach (var image in images) image.sprite = null;

            DisableButtonPromptMessage();
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
                    image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
                    image.sprite = null;
                    image.gameObject.SetActive(false);
                }

                //we can have a composite, so we make sure we go through each path
                for (int i = 0; i < bindingPaths.Length; i++)
                {
                    Sprite sprite = InputManager.Instance.GetIconFromBinding(bindingPaths[i]);
                    if (sprite != null)
                    {
                        images[i].sprite = sprite;
                        images[i].gameObject.SetActive(true);
                    }
                    else UnityEngine.Debug.LogError($"Tried to assign button message sprite for binding path:{bindingPaths[i].name} but it was not found!");
                }
            }

            //get the last image, find the max horizontal position and then set it with the offset
            //RectTransform lastImageTransform = images[bindingPaths.Length - 1].GetComponent<RectTransform>();
            //float lastPosition = lastImageTransform.localPosition.x + lastImageTransform.sizeDelta.x / 2;

            //if (messageText2 != null) messageText2.GetComponent<RectTransform>().localPosition = new Vector2(lastPosition + textHorizontalOffsetFromIcon,
            //   messageText2.GetComponent<RectTransform>().localPosition.y);


            container.SetActive(true);
            //SetImageAndTextAlpha(1f, enableImageAndTextTime);
            container.SetActive(true);

            InputManager.Instance.InputAsset[preset.InputAction.name].performed += TargetActionTriggered;
        }

        /// <summary>
        /// Enables this ButtonPrompt.cs's default/start preset. If this does not exist or is null, it will throw an error
        /// </summary>
        public void EnableButtonPromptMessage()
        {
            if (startPreset==null)
            {
                UnityEngine.Debug.LogError($"Tried to call EnableButtonPromptMessage() in {gameObject.name} with no ButtonPromptPresetSO argument, but this instance does not have a startPreset set up!");
                return;
            }
            EnableButtonPromptMessage(startPreset);
        }

        /*
        private void SetImageAndTextAlpha(float newAlpha, float time, bool disableGameObject = false)
        {
            //we then begin gradually increasing the text and image alpha
            if (messageText1 != null) StartCoroutine(HUD.Instance.LerpTextAlpha(messageText1, time, newAlpha));
            if (messageText2 != null) StartCoroutine(HUD.Instance.LerpTextAlpha(messageText2, time, newAlpha));
            foreach (var image in images) if (image.sprite != null) StartCoroutine(HUD.Instance.LerpImageAlpha(image, time, newAlpha));

            if (disableGameObject)
            {
                IEnumerator DisableAfterLerpEnd()
                {
                    //UnityEngine.Debug.Log("Container timer for disabling has begun!");
                    yield return new WaitForSecondsRealtime(time);
                    container.SetActive(false);
                }
                StartCoroutine(DisableAfterLerpEnd());
            }
        }
        */

        public void DisableButtonPromptMessage()
        {
            isEnabled = false;
            //UnityEngine.Debug.Log("Disabling called for sprite message!");
            //if (gameObject.activeInHierarchy) SetImageAndTextAlpha(0f, disableImageAndTextTime, disableGameObject: true);
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


