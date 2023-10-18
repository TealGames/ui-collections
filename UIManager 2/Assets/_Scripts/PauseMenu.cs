using Game.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UI
{
    public class PauseMenu : BaseUI, IUIBlurUser
    {
        [Header("Pause Menu")]
        [SerializeField] private InputActionReference enableAction;
        [SerializeField] private InputActionReference disableAction;

        public event Action OnEnableBlur;
        public event Action OnDisableBlur;

        // Start is called before the first frame update
        void Start()
        {
            if (enableAction!=null) InputManager.Instance.InputAsset[enableAction.name].performed += EnableUIContext;
            Container.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void EnableUIContext(InputAction.CallbackContext context) => EnableUI();

        public override void EnableUI()
        {
            base.EnableUI();
            OnEnableBlur?.Invoke();
            if (enableAction != null) InputManager.Instance.InputAsset[enableAction.name].performed -= EnableUIContext;
            if (disableAction != null) InputManager.Instance.InputAsset[disableAction.name].performed += DisableUIContext;
        }


        private void DisableUIContext(InputAction.CallbackContext context) => DisableUI();
        public override void DisableUI()
        {
            base.DisableUI();
            OnDisableBlur?.Invoke();
            if (disableAction != null) InputManager.Instance.InputAsset[disableAction.name].performed -= DisableUIContext;
            if (enableAction != null) InputManager.Instance.InputAsset[enableAction.name].performed += EnableUIContext;
        }

    }
}

