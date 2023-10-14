using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public class TabContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tabTitle;
        [SerializeField] private ExtendedButton resetDefaultsButton;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetTitle(string title) => tabTitle.text = title;
        public string GetTitle() => tabTitle.text;

        /// <summary>
        /// Returns true if there are other UI elements implemented in the container. 
        /// Checks if there are more UI elements besides the reset button and tab title
        /// </summary>
        /// <returns></returns>
        public bool HasContents() => transform.childCount > 2;
        public void SetResetDefaultAction(UnityAction action) => resetDefaultsButton.AddOnClickAction(action);
    }
}

