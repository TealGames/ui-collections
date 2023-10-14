using Game.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.UI
{
    public class PauseMenu : BaseUI
    {
        [Header("Pause Menu")]
        [SerializeField] private InputActionReference enableAction;
        [SerializeField] private InputActionReference disableAction;

        // Start is called before the first frame update
        void Start()
        {
            InputManager.Instance.InputAsset[enableAction.name].performed += EnableUIContext;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void EnableUIContext(InputAction.CallbackContext context) => EnableUI();

        public override void EnableUI()
        {
            base.EnableUI();
            InputManager.Instance.InputAsset[enableAction.name].performed -= EnableUIContext;
            InputManager.Instance.InputAsset[disableAction.name].performed += DisableUIContext;
        }


        private void DisableUIContext(InputAction.CallbackContext context) => DisableUI();
        public override void DisableUI()
        {
            base.DisableUI();
            InputManager.Instance.InputAsset[disableAction.name].performed -= DisableUIContext;
            InputManager.Instance.InputAsset[enableAction.name].performed += EnableUIContext;
        }

    }
}

