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
    public sealed class InputManager : MonoBehaviour
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

        private void Start()
        {
            UnityEngine.Debug.Log($"Current mouse: {Mouse.current.description.manufacturer}");
            foreach (var device in InputSystem.devices) UnityEngine.Debug.Log($"Connected device: {device.name}: {device.description.product}");
            foreach (var binding in InputAsset["Move"].bindings) UnityEngine.Debug.Log($"The binding on MOVE is {binding}");
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


        #region Binding Methods
        /// <summary>
        /// Will get a List of <see cref="InputBinding"/> from the <see cref="InputAction"/> <paramref name="name"/> from map <paramref name="mapName"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="allCaps"></param>
        /// <param name="mapName"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Will get a List of <see cref="InputBinding"/> from the <see cref="InputAction"/> <paramref name="name"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="allCaps"></param>
        /// <param name="getAllComposites"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Will get the first composite or <see cref="InputBinding"/>, no matter what control scheme it corresponds to, as a single string
        /// </summary>
        /// <param name="name"></param>
        /// <param name="allCaps"></param>
        /// <param name="separateCompositesWithSpace"></param>
        /// <returns></returns>
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

        /// <summary>
        ///  Will get the first composite or <see cref="InputBinding"/>, no matter what control scheme it corresponds to, as an <see cref="InputBinding"/>[]
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Will get the <see cref="InputDeviceType"/> that corresponds to the <see cref="InputBinding"/>
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        public InputDeviceType GetInputDeviceFromBinding(InputBinding binding)
        {
            foreach (var device in Enum.GetValues(typeof(InputDeviceType)))
            {
                if (binding.path.Contains(device.ToString())) return (InputDeviceType)device;
            }
            UnityEngine.Debug.LogError($"Could not find an input device from the binding: {binding.name}");
            return default;
        }

        /// <summary>
        /// Will get a List of <see cref="InputBinding"/> of the current connected device from <see cref="InputAction"/> <paramref name="action"/>. 
        /// It will checkall the types of devices and if it has a current device. For example, if a keyboard is connected it checks if <see cref="Keyboard.current"/> != null.
        /// If there is a composite, it will return all the <see cref="InputBinding"/>, otherwise it just returns the one found. 
        /// </summary>
        /// <remarks>If 0 <see cref="InputBinding"/> are found or an error occurs, null is returned.</remarks>
        /// <param name="action"></param>
        /// <returns></returns>
        public List<InputBinding> GetBindingsForConnectedDeviceFromAction(InputAction action)
        {
            int index = 0;
            InputDeviceType? targetDevice = GetConnectedDeviceType();
            if (targetDevice == null)
            {
                UnityEngine.Debug.LogError($"Tried to call {typeof(InputManager)}.GetBindingsForConnectedDeviceFromAction(), but there are no currently connected devices that are accepted!");
                return null;
            }

            List<InputBinding> inputBindings = new List<InputBinding>();
            foreach (var binding in action.bindings)
            {
                //If it is not a compsite and it matches the correct connected device, we check further. 
                //Note: Since we can get either keyboard or mouse, either is accepted. This may not work for all games
                if (!binding.isComposite && (GetInputDeviceFromBinding(binding) == targetDevice ||
                    targetDevice == InputDeviceType.Keyboard && GetInputDeviceFromBinding(binding) == InputDeviceType.Mouse))
                {
                    //If the binding is by itself, we just return it
                    if (!binding.isPartOfComposite)
                    {
                        inputBindings.Add(binding);
                        return inputBindings;
                    }
                    else
                    {
                        //Otherwise, we continue from this binding and look until we either find another composite or finish with all bindings.
                        //By then, we should have gathered all the bindings in the composite and then get the icons for each binding  
                        for (int i = index; i < action.bindings.Count; i++)
                        {
                            if (action.bindings[i].isComposite) break;
                            else if (action.bindings[i].isPartOfComposite) inputBindings.Add(action.bindings[i]);
                        }
                        return inputBindings;
                    }
                }
                index++;
            }
            UnityEngine.Debug.LogError($"Tried to find the {typeof(InputBinding)}s from {typeof(InputAction)} {action.name} for the current connected device type: {GetConnectedDeviceType()} but there aren't any! " +
                $"Make sure that the connected device is correct and that there are {typeof(InputBinding)}s for it! ");
            return null;
        }
        #endregion



        #region Action Methods
        public void AddStartedAction(string actionName, Action<InputAction.CallbackContext> action) => InputAsset[actionName].started += action;
        public void RemoveStartedAction(string actionName, Action<InputAction.CallbackContext> action) => InputAsset[actionName].started -= action;

        public void AddPerformedAction(string actionName, Action<InputAction.CallbackContext> action) => InputAsset[actionName].performed += action;
        public void RemovePerformedAction(string actionName, Action<InputAction.CallbackContext> action) => InputAsset[actionName].performed -= action;

        public void AddCanceledAction(string actionName, Action<InputAction.CallbackContext> action) => InputAsset[actionName].canceled += action;
        public void RemoveCanceledAction(string actionName, Action<InputAction.CallbackContext> action) => InputAsset[actionName].canceled -= action;

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
        #endregion



        #region Prompt Icon Methods
        public List<Sprite> GetIconsFromAction(InputAction action)
        {
            InputDeviceType? targetDevice = GetConnectedDeviceType();
            if(targetDevice==null)
            {
                UnityEngine.Debug.LogError("Tried to execute GetIconFromAction(), but there are no currently connected devices that are accepted!");
                return null;
            }

            List<InputBinding> inputBindings= GetBindingsForConnectedDeviceFromAction(action);
            if(inputBindings.Count==0)
            {
                UnityEngine.Debug.LogError($"Tried to execute GetIconFromAction(), but no {typeof(InputBinding)}s are found for the current connected device type: {targetDevice}");
                return null;
            }

            List<Sprite> sprites = new List<Sprite>();
            foreach (var binding in inputBindings) sprites.Add(GetIconFromBinding(binding));
            return sprites; 
        }

        public Sprite? GetIconFromBinding(InputBinding binding)
        {
            //based on the device, we get the SO with that device data
            InputDeviceType bindingDevice= GetInputDeviceFromBinding(binding);
            GamepadType gamepadType= GetCurrentGamepadType();
            DeviceInputIconsSO buttonSpriteData = null;

            if (deviceButtonSprites.Length > 0)
            {
                foreach (var device in deviceButtonSprites)
                {
                    if (device.DeviceType == bindingDevice && device.GamepadType== gamepadType)
                    {
                        buttonSpriteData = device;
                        break;
                    }
                }
            }

            //dont allow to continue if that device has no sprites
            if (buttonSpriteData == null)
            {
                UnityEngine.Debug.LogError($"Tried to find {typeof(DeviceInputIconsSO)} with {typeof(InputDeviceType)}: {bindingDevice} and {typeof(GamepadType)} {gamepadType} in {typeof(InputManager)} " +
                    $"for button sprites but it was not found!");
                return null;
            }

            //dont allow to continue if the actual data has no pairs
            if (buttonSpriteData.GetIconPairs().Count==0)
            {
                UnityEngine.Debug.LogError($"Was looking for the button sprites but " +
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

        public GamepadType GetCurrentGamepadType()
        {
            if (Gamepad.current == null || Gamepad.current.description.empty) return GamepadType.None;

            string productName = Gamepad.current.description.product.ToLower();

            if (productName.Contains("xbox")) return GamepadType.Xbox;
            else if (productName.Contains("dualsense") || productName.Contains("dualshock")) return GamepadType.PlayStation;
            else if (productName.Contains("switch")) return GamepadType.PlayStation;
            else if (productName.Contains("luna")) return GamepadType.AmazonLuna;
            else if (productName.Contains("stadia")) return GamepadType.GoogleStadia;
            else if (productName.Contains("wii")) return GamepadType.WiiU;
            else if (productName.Contains("steam")) return GamepadType.SteamDeck;
            else return GamepadType.None;
        }

        public InputDeviceType? GetConnectedDeviceType()
        {
            if (Gamepad.current != null) return InputDeviceType.Gamepad;
            else if (Keyboard.current != null) return InputDeviceType.Keyboard;
            else if (Mouse.current != null) return InputDeviceType.Keyboard;
            else return null;
        }

        
        #endregion
    }
}


