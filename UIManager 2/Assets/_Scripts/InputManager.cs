using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Reflection;
using System.Text;
using System.Linq;
using Unity.VisualScripting;
using System.IO;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using Game.UI;
using UnityEngine.Events;

namespace Game.Input
{
    /// <summary>
    /// Manages the game's audio. Only one ever exists in a scene. Access the Singleton Instance with <see cref="InputManager.Instance"/>
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        [field: SerializeField] public InputActionAsset InputAsset { get; private set; }
        private string[] allActions;
        private const string PLAYER_MAP_NAME = "Player";
        private const string UI_MAP_NAME = "UI";
        private InputActionMap playerMap;


        [SerializeField] private float inputTriggerCooldown;
        [SerializeField] private bool doInputCooldowns;
        [SerializeField] private InputActionReference[] cooldownActions;

        /// <summary>
        /// The Input Device type is the device (gamepad, keyboard, mouse, etc.) and the string is the path of that device
        /// </summary>
        private Dictionary<InputDeviceType, string> deviceBindingPaths = new Dictionary<InputDeviceType, string>();
        [SerializeField] private DeviceInputIconsSO[] deviceButtonSprites;

        [Header("Customization")]
        [SerializeField] private BindingIconColor bindingIconColor;

        //the event and its corresponding action name
        private Dictionary<Action, string> inputEvents = new Dictionary<Action, string>();

        public static InputManager Instance { get; private set; }

        // Start is called before the first frame update
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);

            
            if (deviceBindingPaths.Count == 0) SetInputDevicePaths();

            foreach (var deviceButtonSprite in deviceButtonSprites) deviceButtonSprite.AddAllPairsToDictionary();
            //UnityEngine.Debug.Log($"On start device white pairs:{deviceButtonSprites[0].GetWhitePairs().Count}, black pairs: {deviceButtonSprites[0].GetBlackPairs().Count}");
            foreach (var blackPair in deviceButtonSprites[0].GetIconPairs()) UnityEngine.Debug.Log($"Found black pair: {blackPair.Key}, {blackPair.Value}");

            //if the length is greater than 0, for each input on its trigger we disable the action and enable after a short time to prevent spamming
            if (doInputCooldowns && cooldownActions.Length > 0)
            {
                foreach (var action in cooldownActions) InputAsset[action.name].canceled += (InputAction.CallbackContext context) =>
                {
                    if (this != null && context.phase != InputActionPhase.Disabled)
                        StartCoroutine(InputCooldown(action.name));
                };
            }

            playerMap = InputAsset.FindActionMap(PLAYER_MAP_NAME, throwIfNotFound: true);
            foreach (var action in InputAsset) action.Enable();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void OnValidate()
        {
            allActions = GetAllActionsFromAsset().ToArray();
            //foreach (var field in typeof(GameInput).GetFields(BindingFlags.NonPublic | BindingFlags.Instance)) UnityEngine.Debug.Log($"Found field in {field.Name}");

            if (deviceBindingPaths.Count == 0) SetInputDevicePaths();
        }

        
        


        //BINDING METHODS
        public List<string> GetBindingsFromPlayerActionName(string name, bool allCaps, string mapName= PLAYER_MAP_NAME)
        {
            List<string> bindings = new List<string>();
            InputActionMap map = InputAsset.FindActionMap(mapName, throwIfNotFound: true);
            if (map == null) return null;

            foreach (InputControl binding in map.FindAction(name, throwIfNotFound: true).controls)
            {
                //get the path and only get the binding name
                string newBinding = binding.displayName;

                //based on if it should be all caps or not
                if (allCaps) bindings.Add(newBinding.ToUpper());
                else bindings.Add(newBinding);
            }
            return bindings;
        }

        public List<string> GetBindingsFromFullActionName(string name, bool allCaps, bool getAllComposites = true)
        {
            List<string> bindings = new List<string>();

            foreach (var binding in InputAsset[name].bindings)
            {
                //get the path and only get the binding name
                //string currentPath = binding.displayName;
                string newBinding = binding.ToDisplayString();

                //based on if it should be all caps or not
                if (allCaps) bindings.Add(newBinding.ToUpper());
                else bindings.Add(newBinding);
            }

            return bindings;
        }

        public string GetFirstCompositeOrBindingFromFullActionName(string name, bool allCaps, bool separateCompositesWithSpace = true)
        {
            string bindings = "";
            if (InputAsset[name].bindings.Count > 0)
            {
                //if we have a composite
                if (InputAsset[name].bindings[0].isComposite)
                {
                    //foreach composite value (not counting index 0 since that is the composite), we get the binding
                    for (int i = 1; i < InputAsset[name].bindings.Count; i++)
                    {
                        //if we encounter another composite, we return
                        if (InputAsset[name].bindings[i].isComposite) break;

                        string newBinding = "";

                        //if we separate them with spaces, we should not allow the first to have a start space
                        if (separateCompositesWithSpace && i != 1) newBinding = " ";

                        newBinding += InputAsset[name].bindings[i].ToDisplayString();

                        //based on if it should be all caps or not
                        if (allCaps) bindings += newBinding.ToUpper();
                        else bindings += newBinding;
                    }
                }
                else
                {
                    string newBinding = InputAsset[name].bindings[0].ToDisplayString();

                    //based on if it should be all caps or not
                    if (allCaps) bindings += newBinding.ToUpper();
                    else bindings += newBinding;
                }
            }
            return bindings;
        }

        public InputBinding[] GetFirstFullPathCompositeOrBindingFromActionName(string name)
        {
            List<InputBinding> bindings = new List<InputBinding>();
            if (InputAsset[name].bindings.Count > 0)
            {
                //if we have a composite
                if (InputAsset[name].bindings[0].isComposite)
                {
                    //foreach composite value (not counting index 0 since that is the composite), we get the binding
                    for (int i = 1; i < InputAsset[name].bindings.Count; i++)
                    {
                        //if we encounter another composite, we return
                        if (InputAsset[name].bindings[i].isComposite) break;

                        bindings.Add(InputAsset[name].bindings[i]);
                    }
                }
                else bindings.Add(InputAsset[name].bindings[0]);
            }
            return bindings.ToArray();
        }

        public InputDeviceType GetInputDeviceFromBinding(InputBinding binding)
        {
            foreach (var device in Enum.GetValues(typeof(InputDeviceType)))
            {
                if (binding.path.Contains(device.ToString())) return (InputDeviceType)device;
            }
            UnityEngine.Debug.LogError($"Could not find an input device from the binding: {binding.name}");
            return default;
        }



        //ACTION METHODS
        public void AddStartedAction(string actionName, Action<InputAction.CallbackContext> action) => InputAsset[actionName].started += action;
        public void AddPerformedAction(string actionName, Action<InputAction.CallbackContext> action) => InputAsset[actionName].performed += action;
        public void AddCanceledAction(string actionName, Action<InputAction.CallbackContext> action) => InputAsset[actionName].canceled += action;

        public void DisableInputActions(List<string> inputActionNames, string mapName = PLAYER_MAP_NAME)
        {
            foreach (var name in inputActionNames)
            {
                InputActionMap map = InputAsset.FindActionMap(mapName, throwIfNotFound: true);
                InputAction action = map.FindAction(name, throwIfNotFound: true);
                if (action != null) action.Disable();
            }
        }

        public void EnablePlayerInputActions(List<string> inputActionNames, string mapName = PLAYER_MAP_NAME)
        {
            foreach (var name in inputActionNames)
            {
                InputActionMap map = InputAsset.FindActionMap(mapName, throwIfNotFound: true);
                InputAction action = map.FindAction(name, throwIfNotFound: true);
                if (action != null) action.Enable();
            }
        }

        public void DisableAllInputActions() => InputAsset.Disable();

        public void EnableAllInputActions() => InputAsset.Enable();

        /// <summary>
        /// Gets the input action from the name. The name could be either in just the action name, or the id in the form of '{MAP NAME}/{ACTION NAME}'. Note: case does NOT matter
        /// </summary>
        /// <param name="name">The name could be either in just the action name, or the id in the form of '{MAP NAME}/{ACTION NAME}'. Note: case does NOT matter</param>
        /// <returns></returns>
        public InputAction GetActionFromName(string name) => InputAsset[name];

        public List<string> GetAllActionsFromAsset()
        {
            List<string> actions = new List<string>();
            string currentMap = "";
            foreach (var map in InputAsset.actionMaps)
            {
                currentMap = map.name;
                foreach (var action in map.actions) actions.Add($"{currentMap}/{action.name}");
            }
            return actions;
        }
        private IEnumerator InputCooldown(string name)
        {
            //UnityEngine.Debug.Log($"Started input cooldown on: {name}");

            InputAsset[name].Disable();
            yield return new WaitForSecondsRealtime(inputTriggerCooldown);
            InputAsset[name].Enable();

            //UnityEngine.Debug.Log($"Ended input cooldown on: {name}");
        }

        //PATH METHODS
        public string GetActionPathByName(string name) => PLAYER_MAP_NAME + "/" + name;

        private void SetInputDevicePaths()
        {
            foreach (var value in Enum.GetValues(typeof(InputDeviceType)))
            {
                StringBuilder builder = new StringBuilder(value.ToString());
                builder.Insert(0, "<");
                builder.Append(">/");
                deviceBindingPaths.Add((InputDeviceType)value, builder.ToString());
            }
        }

        public string GetPathFromDeviceType(InputDeviceType type) => deviceBindingPaths[type];


        //PROMPT ICON METHODS
        public Sprite? GetIconFromBinding(InputBinding binding)
        {
            //based on the device, we get the SO with that device data
            BindingIconColor color = this.bindingIconColor;
            InputDeviceType bindingDevice= GetInputDeviceFromBinding(binding);
            DeviceInputIconsSO buttonSpriteData = null;

            if (deviceButtonSprites.Length > 0)
            {
                foreach (var device in deviceButtonSprites)
                {
                    if (device.DeviceType == bindingDevice && device.IconColor== color)
                    {
                        buttonSpriteData = device;
                        break;
                    }
                }
            }

            //dont allow to continue if that device has no sprites
            if (buttonSpriteData == null)
            {
                UnityEngine.Debug.LogError($"Tried to find DeviceInputIconSO with device type: {bindingDevice} and color: {color} in GameInput for button sprites but it was not found!");
                return null;
            }

            //dont allow to continue if the color pairs in the SO is not filled up
            if (buttonSpriteData.GetIconPairs().Count==0)
            {
                UnityEngine.Debug.LogError($"Was looking for the color: {color} for button sprites but " +
                    $"{bindingDevice} button sprites has 0 of those pairs: {buttonSpriteData.GetIconPairs().Count}");
                return null;
            }
            Dictionary<string, Sprite> spritePairs = buttonSpriteData.GetIconPairs();

            //find the sprite by the binding path. If it is overriden with a new binding from rebinding in control settings, then we findthe override path
            Sprite sprite = null;
            if (binding.overridePath != null)
            {
                foreach (var pair in spritePairs)
                {
                    //UnityEngine.Debug.Log($"Checking sprite pair for GetSpriteFromBinding(): {pair.Key}, {pair.Value}");
                    if (pair.Key == binding.overridePath)
                    {
                        sprite = pair.Value;
                        break;
                    }
                }
            }
            else
            {
                foreach (var pair in spritePairs)
                {
                    if (pair.Key == binding.path)
                    {
                        //UnityEngine.Debug.Log($"Found the sprite with path: {binding.path}");
                        sprite = pair.Value;
                        break;
                    }
                }
            }

            //UnityEngine.Debug.Log($"Found button sprite: {sprite.name}");
            return sprite;
        }

        public DeviceInputIconsSO GetDeviceSpritesFromDeviceType(InputDeviceType device)
        {
            if (deviceButtonSprites.Length == 0) return null;
            return Array.Find(deviceButtonSprites, sprites => sprites.DeviceType == device);
        }
 

    }
}


