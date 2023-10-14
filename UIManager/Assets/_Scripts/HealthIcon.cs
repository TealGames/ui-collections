using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class HealthIcon : MonoBehaviour
    {
        [Tooltip("When this health icon is destroyed/gone, this event occurs")][SerializeField] private UnityEvent OnThisHealthLost;
        [Tooltip("When this health icon is regained, this event occurs")][SerializeField] private UnityEvent OnThisHealthGained;

        [Tooltip("When this gameObject is enabled.By default, this occurs when the new max health incldues this icon")]
        [SerializeField] private UnityEvent OnEnabled;
        [Tooltip("When this gameObject is disabled")][SerializeField] private UnityEvent OnDisabled;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnEnable()
        {
            OnEnabled?.Invoke();
        }

        private void OnDisable()
        {
            OnDisabled?.Invoke();
        }

        public void LoseHealth()
        {
            OnThisHealthLost?.Invoke();
        }

        public void GainHealth()
        {
            OnThisHealthGained?.Invoke();
        }
    }
}

