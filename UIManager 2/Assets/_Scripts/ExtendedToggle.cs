using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Extends the basic uses of the <see cref="Toggle"/> in order to allow more dynamic setups and customizability
    /// </summary>
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

        /// <summary>
        /// Will set the <see cref="Toggle"/>'s value to this. Note: if <paramref name="invokeEventOnAnyValue"/> is TRUE this behaves differently than just setting <see cref="Toggle.isOn"/> to the value. 
        /// If the default value is on, but you set it to on by default, it will NOT trigger the <see cref="UnityEngine.Events.UnityEvent"/> on the <see cref="Toggle"/> since it requires a value CHANGED. 
        /// Therefore, to ensure it is invoked every time, this will set the value without invoking the event, and then invoke the <see cref="UnityEngine.Events.UnityEvent"/> separately
        /// </summary>
        /// <param name="value"></param>
        public void SetToggleValue(bool value, bool invokeEventOnAnyValue= true)
        {
            toggle.SetIsOnWithoutNotify(value);
            toggle.onValueChanged?.Invoke(ToggleValue);
        }
    }
}
