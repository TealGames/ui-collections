using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class OptionSelector : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI currentOptionText;
        [SerializeField] private Selectable nextOption;
        [SerializeField] private Selectable previousOption;


        [field: SerializeField] public List<string> AllOptions { get; private set; }

#if UNITY_EDITOR
        public List<string> AllOptionsProperty
        {
            get => AllOptions;
            set
            {
                AllOptions = value;
            }
        }
#endif

        private List<string> options = new List<string>();
        private int currentOptionIndex = -1;

        [Tooltip("Called on Start(). If you want to set a default option, this is where you can do it")] 
        [SerializeField] private UnityEvent OnSetup;
        [Tooltip("Called when next option is set. Event is invoked with the name of the new option set. " +
            "This cane be useful for calling functions that change the visuals of the selector")]
        [SerializeField] private UnityEvent<string> OnNextOption;
        [Tooltip("Called when previous option is set. Event is invoked with the name of the new option set. " +
            "This cane be useful for calling functions that change the visuals of the selector")]
        [SerializeField] private UnityEvent<string> OnPreviousOption;
        [Tooltip("Called when any option is set. Event is invoked with the name of the new option set. " +
            "Any function that actually does the changing for the new data should most likely be listening here")] 
        [SerializeField] private UnityEvent<string> OnAnyOptionSet;
        

        // Start is called before the first frame update
        void OnEnable()
        {
            if (this.options.Count == 0 && AllOptions.Count>0) SetAllOptions(AllOptions);
            OnSetup?.Invoke();
        }

        // Update is called once per frame
        void Update()
        {
            UnityEngine.Debug.Log($"There are {options.Count} options internally");
        }

        public void SetAllOptions(List<string> options)
        {
            UnityEngine.Debug.Log($"Set all options called on {gameObject.name} with {options.Count} options");
            //foreach (var option in options) UnityEngine.Debug.Log($"Setting option on {gameObject.name} to {option}");

            this.options.Clear();
            foreach (var option in options)
            {
                string optionFormatted = "";
                
                for (int i=0; i<option.Length; i++)
                {
                    if (char.IsUpper(option[i]) && i != 0 && char.IsLower(option[i - 1])) optionFormatted += " ";
                    optionFormatted+= option[i];    
                }
                optionFormatted = optionFormatted.Replace("_", " ");

                this.options.Add(optionFormatted);
                UnityEngine.Debug.Log($"Setting option on {gameObject.name} to {optionFormatted}");
            }

            //We also update all the inspector visible options so we can see the ones that are actually used during runtime in the inspector
            AllOptions.Clear();
            //AllOptions = this.options;
            foreach (var option in this.options)
            {
                UnityEngine.Debug.Log($"Added {option} to AllOptions");
                AllOptions.Add(option);
            }

            //If we have more than 1 choice, it means we can rotate through options
            if (this.options.Count > 1)
            {
                nextOption.gameObject.SetActive(true);
                previousOption.gameObject.SetActive(true);
            }
            else
            {
                nextOption.gameObject.SetActive(false);
                previousOption.gameObject.SetActive(false);
            }

            UnityEngine.Debug.Log($"After all options set, total options: {this.options.Count}");
        }

        public void SetCurrentOption(string newCurrentOption)
        {
            if (options.Count <= 0)
            {
                UnityEngine.Debug.LogError($"Tried to set the Current Option of {gameObject.name} OptionSelector.cs with {newCurrentOption}, but it there are no possible choices set yet!");
                return;
            }

            int index = -1;
            for (int i = 0; i < options.Count; i++) if (options[i].Equals(newCurrentOption)) index = i;

            if (index == -1)
            {
                UnityEngine.Debug.LogError($"Tried to find option: {newCurrentOption} in all options of {gameObject.name} OptionSelector.cs, but it was not found! Make sure it is spelled correctly!");
                return;
            }

            OnAnyOptionSet?.Invoke(newCurrentOption);   
            currentOptionIndex = index;
            currentOptionText.text = options[index].ToString();
        }

        public void SetNextOption()
        {
            if (options.Count <= 1)
            {
                UnityEngine.Debug.LogError($"Tried to set the next option of {gameObject.name} OptionSelector.cs, but there are only {options.Count} options!");
                return;
            }

            currentOptionIndex++;
            if (currentOptionIndex == options.Count) currentOptionIndex = 0;

            OnNextOption?.Invoke(options[currentOptionIndex].ToString());
            SetCurrentOption(options[currentOptionIndex].ToString());
        }

        public void SetPreviousOption()
        {
            if (options.Count <= 1)
            {
                UnityEngine.Debug.LogError($"Tried to set the previous option of {gameObject.name} OptionSelector.cs, but there are only {options.Count} options!");
                return;
            }

            currentOptionIndex--;
            if (currentOptionIndex <0) currentOptionIndex = options.Count - 1;

            OnPreviousOption?.Invoke(options[currentOptionIndex].ToString());
            SetCurrentOption(options[currentOptionIndex].ToString());
        }
    }
}

