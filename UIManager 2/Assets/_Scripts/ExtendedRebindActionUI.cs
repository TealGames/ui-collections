using Game.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;
using UnityEngine.UI;
using RebindActionUI = UnityEngine.InputSystem.Samples.RebindUI.RebindActionUI;

namespace Game.Input
{
    /// <summary>
    /// Extends the basic uses of the RebindActionUI in order to improve accessibility
    /// </summary>
    public class ExtendedRebindActionUI : MonoBehaviour
    {
        [System.Serializable]
        public enum RebindDisplay
        {
            Text,
            Icon,
        }
        [field: SerializeField] public RebindActionUI RebindActionUI { get; private set; }
        [SerializeField] private RebindDisplay displayType;
        [Tooltip("If true, the global display type (in SettingsMenu.cs) will be ignored and will not set this display type")]
        [SerializeField] private bool overrideGlobalDisplayType;

        [Tooltip("If true, will update the binding so that it matches the connected device")][SerializeField] private bool setBindingFromConnectedDevice;
        [Tooltip("If you choose to set the action of the RebindActionUI as a composite member, then specify the index from the composite using 0 as first composite member, 1 as the second, etc...")]
        [Range(0, 3)][SerializeField] private int compositeIndex;

        [Tooltip("The image that displays the binding icon")][SerializeField] private Image bindingIconImage;
        [Tooltip("The text that displays the binding in text form")][SerializeField] private Text bindingText;

        /// <summary>
        /// The <see cref="InputAction"/> that correpsponds to this <see cref="RebindActionUI"/>
        /// </summary>
        public InputAction Action { get; private set; }

        /// <summary>
        /// The <see cref="InputBinding"/> that correpsponds to this <see cref="RebindActionUI"/>
        /// </summary>
        public InputBinding Binding { get; private set; }

        /// <summary>
        /// The icon that corresponds to the <see cref="InputBinding"/> of this <see cref="RebindActionUI"/>
        /// </summary>
        public Sprite ActionIcon { get; private set; }

        public event Action OnRebindStart;
        public event Action OnRebindEnd;
        public event Action OnRebindUIUpdated;

        // Start is called before the first frame update
        void Start()
        {
            UpdateInputInfo();
            OnRebindUIUpdated += UpdateInputInfo;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnValidate()
        {
            if (bindingIconImage!= null && bindingIconImage.sprite!=null && displayType==RebindDisplay.Icon)
            {
                bindingIconImage.sprite = null;
                UnityEngine.Debug.LogWarning($"A binding icon sprite for image {bindingIconImage.gameObject.name} will be automatically set in PlayMode!");
            }
        }

        public void RebindStart(RebindActionUI action, InputActionRebindingExtensions.RebindingOperation operation)
        {
            OnRebindStart?.Invoke();
        }

        public void RebindEnd(RebindActionUI action, InputActionRebindingExtensions.RebindingOperation operation)
        {
            OnRebindEnd?.Invoke();
        }

        public void UpdateRebindUI(RebindActionUI action, string displayString, string deviceLayoutName, string controlPath)
        {
            OnRebindUIUpdated?.Invoke();
        }

        private void UpdateInputInfo()
        {
            Action = RebindActionUI.actionReference.action;
            Binding = RebindActionUI.binding;

            if (setBindingFromConnectedDevice)
            {
                InputBinding targetBinding;
                List<InputBinding> usableBindings = InputManager.Instance.GetBindingsForConnectedDeviceFromAction(Action);
                if (compositeIndex!=-1 && usableBindings.Count>0)
                {
                    targetBinding = usableBindings[compositeIndex];
                    RebindActionUI.binding = targetBinding;
                    UnityEngine.Debug.Log($"Reset the {typeof(InputBinding)} on {typeof(RebindActionUI)} named {RebindActionUI.gameObject.name} " +
                        $"because the {typeof(InputBinding)} did not match the connected device's {typeof(InputBinding)}!");
                }
            }


            if (displayType == RebindDisplay.Icon && bindingIconImage != null)
            {
                ActionIcon = InputManager.Instance.GetIconFromBinding(Binding);
                bindingIconImage.sprite = ActionIcon;

                bindingIconImage.enabled = true;
                bindingText.enabled = false;
            }
            else if (displayType== RebindDisplay.Text && bindingText != null)
            {
                bindingIconImage.enabled = false;
                bindingText.enabled = true;
            }
        }

        public void SetDisplayType(RebindDisplay displayType)
        {
            if (overrideGlobalDisplayType) return;
            this.displayType = displayType;
        }
    }

}
