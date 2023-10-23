using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// The base class that an openable UI should inherit from to have basic functionalities such as opening and closing, which can be overriden
    /// </summary>
    public class BaseUI : MonoBehaviour, IOpenableUI
    {
        [Header("BaseUI")]
        [Tooltip("The first UI element that inherits from Selectable that should be selected when this UI opens. " +
            "Leave empty if you do not want any selected.")][SerializeField] private Selectable firstSelected;

        
        [Tooltip("The container that holds all the UI elements for a UI system. This can be beneficial to allow the parent class to still be running while the container is disabled")]
        [field: SerializeField] public GameObject Container { get; set; }


        [Header("Activation")]
        [Tooltip("The delay between when EnableUI() is called and the time that it actually activates the container. " +
            "This can be useful for enable animations that require the UI to open after a certain time")][SerializeField] private float enableDelay;
        [field: SerializeField] public UnityEvent OnUIEnabled { get; set; }


        [Header("Deactivation")]
        [Tooltip("The delay between when DisableUI() is called and the time that it actually deactivates the container. " +
            "This can be useful for disable animations that require the UI to close after a certain time")]
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
            StartCoroutine(Delay());
            IEnumerator Delay()
            {
                yield return new WaitForSecondsRealtime(enableDelay);
                OnUIEnabledAction?.Invoke();
                OnUIEnabled?.Invoke();
                Container.SetActive(true);
                if (firstSelected!=null) SetFirstSelected(firstSelected);
            }
        }

        public virtual void DisableUI()
        {
            StartCoroutine(Delay()); 
            IEnumerator Delay()
            {
                yield return new WaitForSecondsRealtime(disableDelay);
                OnUIDisabledAction?.Invoke();
                OnUIDisabled?.Invoke();
                Container.SetActive(false);
            }
        }
    }
}


