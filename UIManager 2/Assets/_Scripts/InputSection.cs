using Game.Input;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Samples.RebindUI;

namespace Game.UI
{
    /// <summary>
    /// A section that holds <see cref="ExtendedRebindActionUI"/> that can be used to group them into categories
    /// </summary>
    public class InputSection : MonoBehaviour
    {
        private List<ExtendedRebindActionUI> extendedActions = new List<ExtendedRebindActionUI>();
        [SerializeField] private bool throwWarningIfRebindActionsNotFound;

        // Start is called before the first frame update
        void Start()
        {
            FindRebindableActions();
            foreach (var action in extendedActions)
            {
                action.OnRebindStart += () => UIManager.Instance.GetComponentInChildren<SettingsMenu>(true).SetRebindOverlayStatus(true);
                action.OnRebindEnd += () => UIManager.Instance.GetComponentInChildren<SettingsMenu>(true).SetRebindOverlayStatus(false);
            }
        }

        // Update is called once per frame 
        void Update()
        {

        }

        private void FindRebindableActions()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).TryGetComponent<ExtendedRebindActionUI>(out ExtendedRebindActionUI rebindAction))
                    extendedActions.Add(rebindAction);
            }

            if (throwWarningIfRebindActionsNotFound && extendedActions.Count == 0)
            {
                UnityEngine.Debug.LogWarning($"Tried to find RebindActionUI.cs (rebindable input actions) in {gameObject.name}'s children, but 0 were found!");
                return;
            }
        }

        public List<RebindActionUI> GetRebindActions()
        {
            List<RebindActionUI> rebindActions= new List<RebindActionUI>();
            foreach (var extendedAction in extendedActions) rebindActions.Add(extendedAction.RebindActionUI);
            return rebindActions;
        }

        public List<ExtendedRebindActionUI> GetExtendedRebindActons() => extendedActions;

        public void ResetSectionBindingsToDefault()
        {
            foreach (var action in extendedActions) action.RebindActionUI.ResetToDefault();
        }

        public void SetBindingDisplayType(ExtendedRebindActionUI.RebindDisplay displayType)
        {
            FindRebindableActions();
            foreach (var action in extendedActions) action.SetDisplayType(displayType);
        }
    }

}
