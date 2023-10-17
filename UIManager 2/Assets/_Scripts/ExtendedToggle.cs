using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class ExtendedToggle : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;
        public bool ToggleValue { get => toggle.isOn; }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetToggleValue(bool value) => toggle.isOn = value;
    }
}
