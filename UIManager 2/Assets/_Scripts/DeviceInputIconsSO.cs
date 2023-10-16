using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Game.UI;

namespace Game.Input
{
    [CreateAssetMenu(fileName = "DeviceInputIconsSO", menuName = "ScriptableObjects/Device Input Prompts")]
    [System.Serializable]
    public class DeviceInputIconsSO : ScriptableObject
    {
        [System.Serializable]
        public class IconSection
        {
            [field: SerializeField] public string SectionName { get; private set; }
            [field: SerializeField] public List<BindingIconPair> SpritePairsSections { get; private set; } = new List<BindingIconPair>();
        }

        [System.Serializable]
        public class BindingIconPair
        {
            [field: SerializeField] public InputAction InputBinding { get; set; }
            //[field: SerializeField] public Sprite WhiteSprite { get; private set; }
            [field: SerializeField] public Sprite Sprite { get; private set; }

            public BindingIconPair(InputAction inputBinding, Sprite whiteSprite, Sprite blackSprite)
            {
                this.InputBinding = inputBinding;
                //this.WhiteSprite = whiteSprite;
                this.Sprite = blackSprite;
            }

            private void OnValidate()
            {
                if (InputBinding.bindings.Count == 0) InputBinding.AddBinding();
            }
        }

        [field: SerializeField] public InputDeviceType DeviceType { get; private set; }
        [field: SerializeField] public BindingIconColor IconColor { get; private set; }


        [Tooltip("You can fit all of the pairs in 1 section, but for devices with many sprite pairs, you can separate them into sections, " +
            "since all sections will be added into 1 when getting data. The name does not matter and is just for helpful reference.")]
        [SerializeField] private List<IconSection> spritePairSections = new List<IconSection>();

        /// <summary>
        /// String is the path name of the binding and sprite is the corresponding sprite
        /// </summary>
        //private Dictionary<string, Sprite> blackSpritePairs = new Dictionary<string, Sprite>();
        private Dictionary<string, Sprite> iconPairs = new Dictionary<string, Sprite>();

        /*
        /// <summary>
        /// String is the path name of the binding and sprite is the corresponding sprite
        /// </summary>
        private Dictionary<string, Sprite> whiteSpritePairs = new Dictionary<string, Sprite>();
        */

        public event Action OnSpritesLoaded;

        private void OnValidate()
        {
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
                        //if (!blackSpritePairs.ContainsKey(currentActionPath)) AddPairToDictionary(currentActionPath, pair.BlackSprite, BindingIconColor.Black);
                        //if (!whiteSpritePairs.ContainsKey(currentActionPath)) AddPairToDictionary(currentActionPath, pair.WhiteSprite, BindingIconColor.White);
                    }
                }
            }

            //UnityEngine.Debug.Log($"There are {whiteSpritePairs.Count} white pairs and {blackSpritePairs.Count} black pairs. Time: {DateTime.Now}");
            OnSpritesLoaded?.Invoke();
            //UnityEngine.Debug.Log($"Invoked sprites loaded in SO: {this.name} Time: {DateTime.Now}");
        }

        /*
        public void AddPairToDictionary(string binding, Sprite sprite, BindingIconColor color)
        {
            //UnityEngine.Debug.Log($"Added {binding} {sprite} {color} to button sprites");
            if (color == BindingIconColor.Black && !blackSpritePairs.ContainsKey(binding))
            {
                blackSpritePairs.Add(binding, sprite);
            }
            else if (color == BindingIconColor.White && !whiteSpritePairs.ContainsKey(binding))
            {
                whiteSpritePairs.Add(binding, sprite);
            }
        }
        */
        public void AddPairToDictionary(string binding, Sprite sprite)
        {
            //UnityEngine.Debug.Log($"Added {binding} {sprite} {color} to button sprites");
            /*
            if (color == BindingIconColor.Black && !iconPairs.ContainsKey(binding))
            {
                iconPairs.Add(binding, sprite);
            }
            else if (color == BindingIconColor.White && !whiteSpritePairs.ContainsKey(binding))
            {
                whiteSpritePairs.Add(binding, sprite);
            }
            */
            iconPairs.Add(binding, sprite);
        }

        //public Dictionary<string, Sprite> GetBlackPairs() => iconPairs;
        public Dictionary<string, Sprite> GetIconPairs() => iconPairs;
        //public Dictionary<string, Sprite> GetWhitePairs() => whiteSpritePairs;
    }
}


