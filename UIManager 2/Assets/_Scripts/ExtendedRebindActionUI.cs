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

        [SerializeField] private Image bindingIconImage;
        [SerializeField] private Text bindingText;
        public InputAction Action { get; private set; }
        public InputBinding Binding { get; private set; }
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
