using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Game.UI;

namespace Game.Input
{
    /// <summary>
    /// Stores the <see cref="InputBinding"/> that corresponds to each binding button prompt icon in order to get the icon from the <see cref="InputBinding"/>
    /// </summary>
    [CreateAssetMenu(fileName = "DeviceInputIconsSO", menuName = "ScriptableObjects/Device Input Icons")]
    [System.Serializable]
    public class DeviceInputIconsSO : ScriptableObject
    {
        [System.Serializable]
        public class IconSection
        {
            [Tooltip("Name is for your convience and is only for organization when having a large quantity of icons")]
            [field: SerializeField] public string SectionName { get; private set; }
            [field: SerializeField] public List<BindingIconPair> SpritePairsSections { get; private set; } = new List<BindingIconPair>();
        }

        [System.Serializable]
        public class BindingIconPair
        {
            [Tooltip("The binding that corresponds to the icon")][field: SerializeField] public InputAction InputBinding { get; set; }
            [Tooltip("The icon that corresponds to the binding")][field: SerializeField] public Sprite Sprite { get; private set; }

            public BindingIconPair(InputAction inputBinding, Sprite whiteSprite, Sprite blackSprite)
            {
                this.InputBinding = inputBinding;
                this.Sprite = blackSprite;
            }

            private void OnValidate()
            {
                if (InputBinding.bindings.Count == 0) InputBinding.AddBinding();
            }
        }

        [field: SerializeField] public InputDeviceType DeviceType { get; private set; }
        [field: SerializeField] public GamepadType GamepadType { get; private set; }
        [field: SerializeField] public BindingIconColor IconColor { get; private set; }


        [Tooltip("You can fit all of the pairs in 1 section, but for devices with many sprite pairs, you can separate them into sections, " +
            "since all sections will be added into 1 when getting data. The name does not matter and is just for helpful reference.")]
        [SerializeField] private List<IconSection> spritePairSections = new List<IconSection>();

        /// <summary>
        /// String is the path name of the binding and sprite is the corresponding sprite
        /// </summary>
        private Dictionary<string, Sprite> iconPairs = new Dictionary<string, Sprite>();

        public event Action OnSpritesLoaded;

        private void OnValidate()
        {
            if (DeviceType != InputDeviceType.Gamepad) GamepadType = GamepadType.None;
            if (DeviceType == InputDeviceType.Gamepad && GamepadType == GamepadType.None) GamepadType = GamepadType.Xbox;
            AddAllPairsToDictionary();
        }

        public void AddAllPairsToDictionary()
        {
            if (spritePairSections != null && spritePairSections.Count > 0)
            {
                foreach (var section in spritePairSections)
                {
                    foreach (var pair in section.SpritePairsSections)
                    {
                        string currentActionPath = pair.InputBinding.bindings[0].path;
                        if (!iconPairs.ContainsKey(currentActionPath)) AddPairToDictionary(currentActionPath, pair.Sprite);
                    }
                }
            }
            OnSpritesLoaded?.Invoke();
        }
        public void AddPairToDictionary(string binding, Sprite sprite) => iconPairs.Add(binding, sprite);

        public Dictionary<string, Sprite> GetIconPairs() => iconPairs;
    }
}


