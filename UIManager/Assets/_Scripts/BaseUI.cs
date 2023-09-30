using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class BaseUI : MonoBehaviour, IOpenableUI
    {
        [Header("BaseUI")]
        [SerializeField] private Selectable firstSelected;

        
        [field: SerializeField] public GameObject Container { get; set; }


        [Header("Activation")]
        [SerializeField] private float firstSelectedSetDelay;
        [field: SerializeField] public UnityEvent OnUIEnabled { get; set; }


        [Header("Deactivation")]
        [SerializeField] private float disableDelay;
        [field: SerializeField] public UnityEvent OnUIDisabled { get; set; }

        public Action OnUIEnabledAction { get; set; }
        public Action OnUIDisabledAction { get; set; }


        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetFirstSelected(Selectable firstSelected) => firstSelected.Select();

        public virtual void EnableUI()
        {
            OnUIEnabledAction?.Invoke();
            OnUIEnabled?.Invoke();
            Container.SetActive(true);

            if (firstSelected != null)
            {
                StartCoroutine(FirstSelectedDelay());
                IEnumerator FirstSelectedDelay()
                {
                    yield return new WaitForSecondsRealtime(firstSelectedSetDelay);
                    SetFirstSelected(firstSelected);
                }
            }
        }

        public virtual void DisableUI()
        {
            OnUIDisabledAction?.Invoke();
            OnUIDisabled?.Invoke();
            Container.SetActive(false);
        }
    }
}


