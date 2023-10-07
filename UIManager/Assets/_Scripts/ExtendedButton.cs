using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class ExtendedButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// Will add an action to this button. Note: this will NOT show up in the inspector because it is anonymous
        /// </summary>
        /// <param name="action"></param>
        public void SetOnClickAction(UnityAction action) => button.onClick.AddListener(action);
    }

}
