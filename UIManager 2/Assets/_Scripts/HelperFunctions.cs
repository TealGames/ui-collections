using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using Game.UI;
using System.IO;
using System.Text;

namespace Game.Utilities
{
    /// <summary>
    /// Provides lots of useful methods that abstract more complicated tasks. 
    /// <br>If you are unsure what Extension Methods are, there is more info here: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods</br>
    /// </summary>
    public static class HelperFunctions
    {
        #region Properties and Constants
        private static Camera mainCamera;
        /// <summary>
        /// Since doining Camera.Main is expensive because it searches through every hierarchy gameObject, it will be cached once and you can get the reference here rather than re-searching all the gameObjects
        /// </summary>
        public static Camera MainCamera
        {
            get
            {
                //finding main camera is very expensive, so we only do it once from start
                if (mainCamera == null) mainCamera = Camera.main;
                return mainCamera;
            }
        }

        /// <summary>
        /// The string used to separate data in a file. Change the initialization value in HelperFunctions in order to universally change the separator
        /// </summary>
        public const string DATA_SEPARATOR = ";";
        #endregion



        #region Quality of Life Methods
        /// <summary>
        /// A shorthand for setting the position of a gameObject
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="position"></param>
        public static void SetObjectPos(this GameObject gameObject, Vector2 position) => gameObject.transform.position = position;

        public static string GenerateRandomID() => System.Guid.NewGuid().ToString();

        /// <summary>
        /// Will generate a random int from 0-100 inclusive
        /// </summary>
        /// <returns></returns>
        public static int GenerateRandomPrecentage() => UnityEngine.Random.Range(0, 101);

        public static void PlaySound(this AudioClip audioClip, AudioSource audioSource, float minPitch, float maxPitch, float volume)
        {
            audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(audioClip, volume);
        }

        public static Quaternion LookAt2D(Vector2 forward) => Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);

        /// <summary>
        /// Will deselect a <see cref="Button"/> by disabling is interactability and reenabling it
        /// </summary>
        /// <param name="button"></param>
        public static void Deselect(this Button button)
        {
            button.interactable = false;
            button.interactable = true;
        }

        /// <summary>
        /// Checks if 2 points overlap vertically. Range1 and Range2 must both be ordered (min,max), but if they aren't the function will change them. 
        /// Note: these are not points of (x,y) but (min y, max y)
        /// </summary>
        /// <param name="range1"></param>
        /// <param name="range2"></param>
        /// <returns></returns>
        public static bool DoVerticalPointsOverlap(Vector2 range1, Vector2 range2)
        {
            //We make sure that the points are ordered, (min, max)
            if (range1.x > range1.y) range1 = new Vector2(range1.y, range1.x);
            if (range2.x > range2.y) range2 = new Vector2(range2.y, range2.x);

            //We check if range2 is either above range1 or below range1 and then reverse it (since we are checking if they overlap)
            return !((range2.x > range1.y && range2.y > range1.y) || (range2.x < range1.x && range2.y < range1.x));
        }



        /// <summary>
        /// Gets the world coordinates of a UI elment given its <see cref="RectTransform"/>. 
        /// An example of a use case is if you have to position non-UI objects to match behind UI elements, like particle effects.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, MainCamera, out var result);
            return result;
        }

        /// <summary>
        /// Will set <see cref="RectTransform.anchorMin"/> and <see cref="RectTransform.anchorMax"/> of <paramref name="transform"/> based on <paramref name="preset"/>. 
        /// You can check out what each preset does by selecting a <see cref="RectTransform"/> and changing the preset in the top left corner 
        /// <br>(Notice the values that are set for <see cref="RectTransform.anchorMin"/> and <see cref="RectTransform.anchorMax"/>).</br>
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="preset"></param>
        public static void SetAnchorPreset(this RectTransform transform, AnchorPresets preset)
        {
            switch(preset)
            {
                case AnchorPresets.TopLeft:
                    SetAnchors(transform, new Vector2(0, 1), new Vector2(0, 1));
                    return;
                case AnchorPresets.TopCenter:
                    SetAnchors(transform, new Vector2(0.5f, 1), new Vector2(0.5f, 1));
                    return;
                case AnchorPresets.TopRight:
                    SetAnchors(transform, new Vector2(1, 1), new Vector2(1, 1));
                    return;
                case AnchorPresets.TopStretch:
                    SetAnchors(transform, new Vector2(0, 1), new Vector2(1, 1));
                    return;

                case AnchorPresets.MiddleLeft:
                    SetAnchors(transform, new Vector2(0, 0.5f), new Vector2(0, 0.5f));
                    return;
                case AnchorPresets.MiddleCenter:
                    SetAnchors(transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
                    return;
                case AnchorPresets.MiddleRight:
                    SetAnchors(transform, new Vector2(1, 0.5f), new Vector2(1, 0.5f));
                    return;
                case AnchorPresets.MiddleStretch:
                    SetAnchors(transform, new Vector2(0, 0.5f), new Vector2(1, 0.5f));
                    return;

                case AnchorPresets.BottomLeft:
                    SetAnchors(transform, new Vector2(0, 0), new Vector2(0, 0));
                    return;
                case AnchorPresets.BottomCenter:
                    SetAnchors(transform, new Vector2(0.5f, 0), new Vector2(0.5f, 0));
                    return;
                case AnchorPresets.BottomRight:
                    SetAnchors(transform, new Vector2(1, 0), new Vector2(1, 0));
                    return;
                case AnchorPresets.BottomStretch:
                    SetAnchors(transform, new Vector2(0, 0), new Vector2(1, 0));
                    return;

                case AnchorPresets.StretchLeft:
                    SetAnchors(transform, new Vector2(0, 0), new Vector2(0, 1));
                    return;
                case AnchorPresets.StretchCenter:
                    SetAnchors(transform, new Vector2(0.5f, 0), new Vector2(0.5f, 1));
                    return;
                case AnchorPresets.StretchRight:
                    SetAnchors(transform, new Vector2(1, 0), new Vector2(1, 1));
                    return;
                case AnchorPresets.StretchStretch:
                    SetAnchors(transform, new Vector2(0, 0), new Vector2(1, 1));
                    return;

                default:
                    UnityEngine.Debug.LogWarning($"Tried to update {transform}'s anchor preset, but preset {preset} does not have a corresponding anchor setting!");
                    return;
            }
        }

        /// <summary>
        /// Will set <see cref="RectTransform.anchorMin"/> and <see cref="RectTransform.anchorMax"/> of <paramref name="transform"/>
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static void SetAnchors(this RectTransform transform, Vector2 min, Vector2 max)
        {
            Vector3 position= transform.localPosition;

            transform.anchorMin = min;
            transform.anchorMax = max;

            //We make sure that we do not change the position of the rect transform when we set the anchors
            transform.localPosition = position;
        }


        public static void DestroyChildren(this Transform parentTransform)
        {
            foreach (Transform child in parentTransform) UnityEngine.Object.Destroy(child.gameObject);
        }

        public static V GetDictionaryValueAtIndex<T, V>(this Dictionary<T, V> dictionary, int targetIndex)
        {
            int currentIndex = 0;
            V foundValue = default;
            foreach (var pair in dictionary)
            {
                if(currentIndex==targetIndex)
                {
                    foundValue= pair.Value;
                    break;
                }
                else currentIndex++; 
            }
            return foundValue;
        }

        public static T[] AddToArray<T>(T[] originalArray, T newItem)
        {
            T[] newArray = new T[originalArray.Length + 1];
            for (int i = 0; i < originalArray.Length; i++)
            {
                newArray[i] = originalArray[i];
            }
            newArray[originalArray.Length] = newItem;
            return newArray;
        }

        /// <summary>
        /// Will copy the component values from <paramref name="originalComponent"/> to <paramref name="newComponent"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="originalComponent"></param>
        /// <param name="newComponent"></param>
        public static void CopyComponentValues<T>(T originalComponent, T newComponent) where T : Component
        {
            var json = JsonUtility.ToJson(originalComponent);
            JsonUtility.FromJsonOverwrite(json, newComponent);
        }

        public static T? GetFirstComponentInChildrenOfType<T>(this GameObject gameObject, bool includeParent) where T : Component
        {
            //UnityEngine.Debug.Log($"Called GetFirstComponentInChildrenOfType() and parent has: {gameObject.transform.childCount} children");

            if (includeParent && gameObject.TryGetComponent<T>(out T component)) return component;

            if (gameObject.transform.childCount == 0) return null;
            else
            {

                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    if (gameObject.transform.GetChild(i).TryGetComponent<T>(out T targetComponent)) return targetComponent;
                }

                //if we get to this point, we have went through all children, meaning that it was not found
                return null;
            }
        }

        /// <summary>
        /// Will convert the enum values of enum type <paramref name="enumType"/> to a <see cref="string"/> List
        /// </summary>
        /// <param name="enumType"></param>
        /// <returns></returns>
        public static List<string> GetListFromEnum(Type enumType)
        {
            //UnityEngine.Debug.Log($"Called GetListFromEnum with argument: {enumType} type: {enumType.GetType()}");
            List<string> list = new List<string>();
            if(!enumType.IsEnum)
            {
                UnityEngine.Debug.LogError($"Tried to get a list of string values from enum: {enumType}, but it does not exist as an enum!");
                return list;
            }
            foreach (var enumValue in Enum.GetValues(enumType)) list.Add(enumValue.ToString());
            return list;
        }

        /// <summary>
        /// Will get the <see cref="Type"/> from the <see cref="UserSelectedType"/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTypeFromUserSelectedType(UserSelectedType type)
        {
            switch (type)
            {
                case UserSelectedType.Void:
                    return typeof(void);
                case UserSelectedType.Float:
                    return typeof(float);
                case UserSelectedType.Int:
                    return typeof(int);
                case UserSelectedType.String:
                    return typeof(string);
                case UserSelectedType.Boolean:
                    return typeof(bool);
                case UserSelectedType.Vector2:
                    return typeof(Vector2);
                case UserSelectedType.Vector3:
                    return typeof(Vector3);
                default:
                    UnityEngine.Debug.LogWarning($"Tried to find User Selected Type:{type}, but it is not defined as a returnable type in GetTypeFromUserSelectedType() in HelperFunctions");
                    return null;
            }
        }


        /// <summary>
        /// Scales an object around an arbitrary point rather than a gameObject's pivot
        /// </summary>
        /// <param name="target"></param>
        /// <param name="pivot"></param>
        /// <param name="newScale"></param>
        public static void ScaleAround(GameObject target, Vector3 pivot, Vector3 newScale)
        {
            //pivot
            var pivotDelta = target.transform.localPosition - pivot;
            Vector3 scaleFactor = new Vector3(newScale.x / target.transform.localScale.x, newScale.y / target.transform.localScale.y, newScale.z / target.transform.localScale.z);
            pivotDelta.Scale(scaleFactor);

            target.transform.localPosition = pivot + pivotDelta;

            //scale
            target.transform.localScale = newScale;
        }
        
        /// <summary>
        /// Gets all of the non-zero (None value) flags in a multi-select (<see cref="System.FlagsAttribute"/>) enum
        /// </summary>
        /// <param name="flagEnum"></param>
        /// <returns></returns>
        public static List<string> GetFlagEnumValues(Enum flagEnum)
        {
            List<string> list = new List<string>();
            foreach (var flag in flagEnum.ToString().Split(","))
            {
                //if the value contains a space, find the space and remove it
                string newValue = flag.ToString().Trim();
                if (newValue.Contains(" "))
                {
                    for (int i = 0; i < newValue.Length; i++)
                    {
                        if (newValue[i] == ' ') newValue.Remove(i, 1);
                    }
                }
                list.Add(newValue);
            }
            return list;
        }
        #endregion
        


        #region Reflection Methods

        public static List<string> GetAllPublicVoidMethods(Type type)
        {
            List<string> methods = new List<string>();
            foreach (var method in type.GetMethods())
            {
                if (method.ReturnType == typeof(void) && method.GetParameters().Length == 0) methods.Add(method.Name);
            }
            return methods;
        }

        public static List<MethodInfo> GetAllPublicVoidMethodInfo(Type type)
        {
            List<MethodInfo> methods = new List<MethodInfo>();
            foreach (var method in type.GetMethods())
            {
                if (method.ReturnType == typeof(void) && method.GetParameters().Length == 0) methods.Add(method);
            }
            return methods;
        }

        public static void CallPublicVoidMethodByName(string name, Type typeForMethods, object objectToInvokeMethodOn)
        {
            foreach (var method in typeForMethods.GetMethods())
            {
                if (method.ReturnType == typeof(void) && method.GetParameters().Length == 0 && method.Name == name) method.Invoke(objectToInvokeMethodOn, null);
            }
        }

        public static List<string> GetAllPublicMethodsWithReturnType(Type type, Type returnType)
        {
            List<string> methods = new List<string>();
            foreach (var method in type.GetMethods())
            {
                if (method.ReturnType == returnType && method.GetParameters().Length == 0) methods.Add(method.Name);
            }
            return methods;
        }

        public static List<string> GetAllPublicMethodsFromMultipleScriptsWithReturnType(Type[] types, Type returnType)
        {
            List<string> methods = new List<string>();
            foreach (var script in types)
            {
                foreach (var method in script.GetMethods())
                {
                    if (method.ReturnType == returnType && method.GetParameters().Length == 0) methods.Add(method.Name);
                }
            }
            return methods;
        }

        public static MethodInfo GetMethodInfoByMethodName(string name, Type typeForMethods) => typeForMethods.GetMethod(name);


        public static List<string> GetAllPublicActionEventsFromType(Type type)
        {
            List<EventInfo> events = new List<EventInfo>();
            foreach (var declaredEvent in type.GetEvents())
            {
                //UnityEngine.Debug.Log($"Checking declared public event: {declaredEvent.Name}");
                events.Add(declaredEvent);
            }

            //Func<EventInfo, bool> testUnityEvents = (EventInfo info) => !info.EventHandlerType.IsGenericType && (info.EventHandlerType == typeof(Action) || info.EventHandlerType == typeof(UnityAction));
            Func<EventInfo, bool> normalTest = (EventInfo info) => !info.EventHandlerType.IsGenericType && info.EventHandlerType == typeof(Action);

            //List<string> eventNames = events.Where(includeUnityEvents? testUnityEvents : normalTest).Select(eventCheck => eventCheck.Name).ToList();
            List<string> eventNames = events.Where(normalTest).Select(eventCheck => eventCheck.Name).ToList();
            return eventNames;
        }

        public static EventInfo GetEventInfoByEventName(string name, Type typeForEvent) => typeForEvent.GetEvent(name);

        /// <summary>
        /// Checks is a class has any subscribers to an event. <paramref name="source"/>"/> is the publisher or the class that holds/invokes the event.
        /// <paramref name="eventHandler"/> is the actual event, where <paramref name="target"/> is the class you are checking
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="eventHandler"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool IsSubscribed<T>(object source, EventHandler<T> eventHandler, object target)
        {
            var invocationList = eventHandler.GetInvocationList();

            foreach (var del in invocationList)
                if (del.Target == target) return true;
            return false;
        }

        public static bool TryGetUnityEventByName(string name, Type type, object typeInstance, out UnityEvent foundEvent)
        {
            FieldInfo[] fields = typeInstance.GetType().GetFields();
            List<FieldInfo> unityEvents = fields.Where(f => f.FieldType == typeof(UnityEvent)).ToList();

            foundEvent = null;
            foreach (FieldInfo field in unityEvents)
            {
                if (field.Name == name)
                {
                    foundEvent = (UnityEvent)field.GetValue(typeInstance);
                    break;
                }
            }

            return foundEvent != null;
        }

        /// <summary>
        /// Gets all the <see cref="MonoBehaviour"/> that implement the interface of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<MonoBehaviour> GetAllMonoBehaviorsWithInterfaceType<T>()
        {
            if (!typeof(T).IsInterface)
            {
                UnityEngine.Debug.LogError($"Tried to call GetAllMonoBehaviorsWithInterfaceType<T>() but the generic type {typeof(T)} is not an interface!");
                return null;
            }
            MonoBehaviour[] monoBehaviors = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
            return monoBehaviors.Where(behavior => behavior.GetType().GetInterfaces().Contains(typeof(T))).ToList();
        }

        /// <summary>
        /// Gets a <typeparamref name="T"/> array of all the Monobheaviours that implement the interface <typeparamref name="T"/>"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static T[] GetInterfacesOfType<T>(bool includeInactive)
        {
            if (!typeof(T).IsInterface)
            {
                UnityEngine.Debug.LogError($"Tried to call GetInterfacesOfType<T>() but the generic type {typeof(T)} is not an interface!");
                return null;
            }
            return UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive).Where(behavoir => behavoir.GetType().GetInterfaces().Contains(typeof(T))).Cast<T>().ToArray<T>();
        }
            
        /// <summary>
        /// Gets an array of <see cref="GameObject"/> that have a <see cref="MonoBehaviour"/> that implements the interface of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        public static GameObject[] GetInterfacesAsGameObjectsOfType<T>(bool includeInactive)
        {
            if (!typeof(T).IsInterface)
            {
                UnityEngine.Debug.LogError($"Tried to call GetInterfacesAsGameObjectsOfType<T>() but the generic type {typeof(T)} is not an interface!");
                return null;
            }
            return UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive).Where(behavoir => behavoir.GetType().GetInterfaces().Contains(typeof(T))).
            Select(behavior => behavior.gameObject).ToArray<GameObject>();
        }
            

        /// <summary>
        /// Will get all event system raycast results of the current mouse or touch position.
        /// </summary>
        /// <returns></returns>
        public static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = UnityEngine.Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
        #endregion



        #region File Management Methods

        /// <summary>
        /// Will save data in the file. If you save this data for the first time, it will create a new file. 
        /// Otherwise, it will delete that file and create a new one with the file data, basically overriding the old data.
        /// If <paramref name="createDirectoriesIfDontExist"/> is true, it will create the directories that don't exist.
        /// </summary>
        /// <remarks>Note: the <paramref name="path"/> can either be relative or absolute since it will be formatted correctly either way</remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="saveObject"></param>
        /// <param name="pathType"></param>
        /// <param name="path"></param>
        /// <param name="createDirectoriesIfDontExist"></param>
        public static void SaveDataInFile<T>(T saveObject, GamePathType pathType, string path, bool createDirectoriesIfDontExist = true)
        {
            UnityEngine.Debug.Log($"Path is: {path}");
            path = path.FormatAsSystemPath();
            string relativePath = GetRelativePathFromPath(path);
            string fullPath = GetPathFromPathType(pathType) + Path.DirectorySeparatorChar + relativePath;
            UnityEngine.Debug.Log($"Full path is {fullPath}, relative path is {relativePath}, ");

            if (File.Exists(fullPath)) File.Delete(fullPath);

            //create the directory if we need to
            if (createDirectoriesIfDontExist)
            {
                List<string> directories = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).ToList();
                directories.Remove(directories.Last());

                string currentPath = GetPathFromPathType(pathType);
                foreach (var directory in directories)
                {
                    currentPath += $"{Path.DirectorySeparatorChar}{directory}";
                    UnityEngine.Debug.Log($"Found directory to create {directory} at path {currentPath}");
                    if (!File.Exists(currentPath) && !Directory.Exists(currentPath)) Directory.CreateDirectory(currentPath);
                }
            }

            using (FileStream fileStream = File.Create(fullPath))
            {
                using (StreamWriter writer = new StreamWriter(fileStream))
                {
                    if (typeof(T) != typeof(string)) writer.Write(JsonUtility.ToJson(saveObject));
                    else writer.Write(saveObject);
                }
            }
        }

        public static bool TryLoadFullData(GamePathType pathType, string path, out string data, bool logWarningIfNotFound = true) => TryLoadData(pathType, path, out data, logWarningIfNotFound);
        /// <summary>
        /// Will try and load the data from the file. If it is found, it returns true and the out parameter is the type <typeparamref name="T"/>. 
        /// If it is not found, it returns false and the out parameter is the default of <typeparamref name="T"/>.
        /// </summary>
        /// <remarks>Note: the T type must NOT inherit from <see cref="UnityEngine.Object"/>(ScriptableObjects, Monobehaviour, etc.). Otherwise use <see cref="HelperFunctions.TryLoadDataOverwrite(GamePathType, string, object, bool)"/></remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="pathType"></param>
        /// <param name="path"></param>
        /// <param name="loadObject"></param>
        /// <param name="logWarningIfNotFound"></param>
        /// <returns></returns>
        public static bool TryLoadData<T>(GamePathType pathType, string path, out T loadObject, bool logWarningIfNotFound = true)
        {
            path = path.FormatAsSystemPath();
            string relativePath = GetRelativePathFromPath(path);
            string fullPath = GetPathFromPathType(pathType) + Path.DirectorySeparatorChar + relativePath;
            UnityEngine.Debug.Log($"system path: {path}, relative path: {relativePath}, full path: {fullPath}");

            loadObject = default(T);
            if (!File.Exists(fullPath) && logWarningIfNotFound)
            {
                UnityEngine.Debug.LogWarning($"Tried to load data of type {typeof(T)} in path {path} but that file does not exist!");
                return false;
            }

            if (typeof(T).GetType().IsSubclassOf(typeof(UnityEngine.Object)))
            {
                UnityEngine.Debug.Log($"Tried to save data, but the object type {typeof(T)} inherit from {typeof(UnityEngine.Object)} since JSON cannot serialize these types!");
                return false;
            }

            using (FileStream fileStream = File.Open(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    UnityEngine.Debug.Log($"Trying to parse data of type {typeof(T)}");
                    if (typeof(T) != typeof(string)) loadObject = JsonUtility.FromJson<T>(reader.ReadToEnd());
                    else loadObject= (T)(object)reader.ReadToEnd();
                }
            }
            return true;
        }


        /// <summary>
        /// Will try and load the data from the file. If it is found, it returns true and <paramref name="overwriteObject"/> is overriden with the data. 
        /// If it is not found, it returns false and <paramref name="overwriteObject"/> is not overriden
        /// </summary>
        /// <remarks>Note: the T type MUST inherit from <see cref="UnityEngine.Object"/>(ScriptableObjects, Monobehaviour, etc.). Otherwise use <see cref="HelperFunctions.TryLoadData{T}(GamePathType, string, out T, bool)"/></remarks>
        /// <param name="pathType"></param>
        /// <param name="path"></param>
        /// <param name="overwriteObject"></param>
        /// <param name="logWarningIfNotFound"></param>
        /// <returns></returns>
        public static bool TryLoadDataOverwrite(GamePathType pathType, string path, object overwriteObject, bool logWarningIfNotFound = true)
        {
            path = path.FormatAsSystemPath();
            string relativePath = GetRelativePathFromPath(path);
            string fullPath = GetPathFromPathType(pathType) + Path.DirectorySeparatorChar + relativePath;

            if (!File.Exists(fullPath) && logWarningIfNotFound)
            {
                UnityEngine.Debug.LogWarning($"Tried to load data of type {overwriteObject.GetType()} in path {fullPath} but that file does not exist!");
                return false;
            }

            if (!overwriteObject.GetType().IsSubclassOf(typeof(UnityEngine.Object)))
            {
                UnityEngine.Debug.Log($"Tried to save data, but the object type {overwriteObject.GetType()} does not inherit from {typeof(UnityEngine.Object)} which is required for TryLoadDataOverwrite().");
                return false;
            }

            using (FileStream fileStream = File.Open(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    JsonUtility.FromJsonOverwrite(reader.ReadToEnd(), overwriteObject);
                }
            }
            return true;
        }

        /// <summary>
        /// Will get the corresponding path formatted as a system path using <see cref="HelperFunctions.FormatAsSystemPath(string, bool)"/> from <see cref="GamePathType"/>
        /// </summary>
        /// <param name="pathType"></param>
        /// <returns></returns>
        public static string GetPathFromPathType(GamePathType pathType)
        {
            switch (pathType)
            {
                case GamePathType.Game:
                    UnityEngine.Debug.Log($"Getting game path: {Application.dataPath}");
                    return Application.dataPath.FormatAsSystemPath();
                case GamePathType.StreamingAsset:
                    UnityEngine.Debug.Log($"Getting streaming asset path: {Application.streamingAssetsPath}");
                    return Application.streamingAssetsPath.FormatAsSystemPath();
                case GamePathType.PersistentSave:
                    UnityEngine.Debug.Log($"Getting persistent data path: {Application.persistentDataPath}");
                    return Application.persistentDataPath.FormatAsSystemPath();
                default:
                    UnityEngine.Debug.LogError($"Tried to get path from {typeof(GamePathType)} {pathType} but it does not have path corresponding to that type in GetFullPathFromPathType()");
                    return "";
            }
        }

        /// <summary>
        /// If a path contains any of the paths derived from <see cref="Game.GamePathType"/> that path segment is removed. 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetRelativePathFromPath(string fullPath)
        {
            string relativePath = fullPath;
            if (fullPath.Split("Assets").Length > 2) fullPath.Replace("Assets/", "");
            foreach (GamePathType pathType in Enum.GetValues(typeof(GamePathType)))
            {
                string startPath = GetPathFromPathType(pathType);
                UnityEngine.Debug.Log($"start path is: {startPath}");
                UnityEngine.Debug.Log($"Full path contains start path {fullPath.Contains(startPath)}, Full: {fullPath}    VS    start: {startPath}");
                if (fullPath.Contains(startPath))
                {
                    StringBuilder builder = new StringBuilder(fullPath);
                    builder= builder.Replace(startPath, "");
                    UnityEngine.Debug.Log($"Relative path from builder {builder.ToString()}");

                    //Since this path is relative to Unity, we set all directory separtors to be only "/"
                    relativePath= builder.ToString();
                    if (relativePath[0].Equals(Path.DirectorySeparatorChar) || relativePath[0].Equals(Path.AltDirectorySeparatorChar))
                    {
                        UnityEngine.Debug.Log($"Found ");
                        relativePath = relativePath.Substring(1);
                    }
                    break;
                }
            }
            UnityEngine.Debug.Log($"Relative path from full path: {fullPath}  -->  {relativePath}");
            return relativePath;
        }

        /// <summary>
        /// If the path contains the directory "Assets", it will format all directory separators and duplicate directory separators to be "/". 
        /// This should NOT be used when using System.IO functions or file functions from <see cref="HelperFunctions"/> since those also use System.IO. 
        /// This is most helpful when having a general file path that may need to be converted into a Unity one when using paths in Unity functions such as AssetDatabase
        /// </summary>
        /// <remarks>Note: if the string is not a path, nothing happens and the argument string is returned</remarks>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string FormatUnityPath(this string fullPath)
        {
            //Only if the string is a valid path and it contains the Unity path root, do we format it
            if (!IsStringAPath(fullPath) || !fullPath.Contains("Assets")) return fullPath;

            string otherPathSegment = fullPath.Substring(0, fullPath.IndexOf("Assets"));
            string unityPath= fullPath.Substring(fullPath.IndexOf("Assets"));
            unityPath.Replace("\\", "/").Replace("\\\\", "/").Replace("//", "/").Replace("\\/", "/").Replace("/\\", "/");
            return otherPathSegment + unityPath;
        }

        /// <summary>
        /// Formats the path to work with System.IO by replacing <see cref="Path.AltDirectorySeparatorChar"/> (which is "/" on Windows) to <see cref="Path.DirectorySeparatorChar"/> (which is "\" on Windows)
        /// and removes duplicated directory separator characters. This is useful if you are using Unity paths and file paths because this method will clean it up and make sure the path can be used with System.IO. 
        /// </summary>
        /// <remarks>Note: if the string is not a path, nothing happens and the argument string is returned</remarks>
        /// <param name="path"></param>
        /// <param name="removeRepeatedConsecutiveDirectories">If true will remove directories that have the same name, including capitalization, that are repeated right after each other.</param>
        /// <returns></returns>
        public static string FormatAsSystemPath(this string path, bool removeRepeatedConsecutiveDirectories= false)
        {
            if (!IsStringAPath(path)) return path;
            
            string formattedPath= path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).
                Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", Path.DirectorySeparatorChar.ToString()).
                Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", Path.DirectorySeparatorChar.ToString());
            if (removeRepeatedConsecutiveDirectories)
            {
                UnityEngine.Debug.Log($"Formatted path before split: {formattedPath}");
                List<string> directories = formattedPath.Split(Path.DirectorySeparatorChar).ToList();
                List<string> duplicateDirectories = new List<string>();
                foreach (var search in directories) UnityEngine.Debug.Log($"Searching directory: {search}");

                for (int i = 0; i< directories.Count; i++)
                {
                    if (i!=directories.Count-1 && directories[i].Equals(directories[Math.Clamp(i + 1, 0, directories.Count-1)]))
                        duplicateDirectories.Add(directories[i+1]);
                }
                foreach (string duplicateDirectory in duplicateDirectories) directories.Remove(duplicateDirectory);

                formattedPath = "";
                for (int i = 0;i< directories.Count; i++)
                {
                    if (i == 0) formattedPath += $"{directories[i]}";
                    else formattedPath += $"{Path.DirectorySeparatorChar}{directories[i]}";
                }
                UnityEngine.Debug.Log($"Path after duplciate check: {formattedPath}");
            }
            UnityEngine.Debug.Log($"Returned system path: {path}");
            return formattedPath;
        }

        /// <summary>
        /// Checks if a string is a path by checking if it contains <see cref="Path.DirectorySeparatorChar"/> or <see cref="Path.AltDirectorySeparatorChar"/>
        /// </summary>
        /// <param name="testedString"></param>
        /// <returns></returns>
        public static bool IsStringAPath(string testedString) => testedString.Contains(Path.DirectorySeparatorChar) || testedString.Contains(Path.AltDirectorySeparatorChar);

        public static void DeleteFile(GamePathType pathType, string filePath)
        {
            filePath = filePath.FormatAsSystemPath();
            string path = Path.Combine(GetPathFromPathType(pathType), GetRelativePathFromPath(filePath));
            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogWarning($"Tried to delete file {filePath} but it does not exist!");
                return;
            }
            File.Delete(path);
        }

        /// <summary>
        /// Deletes a directory (universal name for folder) in the path if it exists. If not, nothing happens
        /// </summary>
        /// <param name="directoryPath"></param>
        public static void DeleteDirectory(GamePathType pathType, string directoryPath)
        {
            directoryPath = directoryPath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            string path = Path.Combine(GetPathFromPathType(pathType), GetRelativePathFromPath(directoryPath));
            if (Directory.Exists(path))
            {
                UnityEngine.Debug.Log($"Deleted directory at {path}");
                Directory.Delete(path, true);
            }
            else UnityEngine.Debug.LogWarning($"Tried to delete directory at {path} but it does not exist!");
        }

        /// <summary>
        /// Appends the KeyValuePair to the END of the dictionary
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="pair"></param>
        /// <param name="createFileIfDoesNotExist"></param>
        public static void AddPairToDictionaryFile<K, V>(GamePathType pathType, string filePath, K key, V value, bool createFileIfDoesNotExist= true)
        {
            string path = Path.Combine(GetPathFromPathType(pathType), GetRelativePathFromPath(filePath));
            if (!File.Exists(path))
            {
                if (createFileIfDoesNotExist) 
                    SaveDataInFile(new Dictionary<K, V>() { { key, value } }, pathType, filePath);
                else
                {
                    UnityEngine.Debug.LogWarning($"Tried to add dictionary key/value pair of type: {typeof(K)}, {typeof(V)} to file: {filePath} but it does not exist!");
                    return;
                }
            }

            TryLoadData(pathType, filePath, out Dictionary<K,V> dictionary);
            dictionary.Add(key, value);
            SaveDataInFile(dictionary, pathType, filePath);
        }

        /// <summary>
        /// Updates the value of the key for the dictionary
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="filePath"></param>
        /// <param name=""></param>
        public static void UpdateValueInDictionaryFile<K, V>(GamePathType pathType, string filePath, K key, V newValue)
        {
            string path = Path.Combine(GetPathFromPathType(pathType), GetRelativePathFromPath(filePath));
            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogWarning($"Tried to udpate pair of type {typeof(K)} from file: {filePath} but it does not exist!");
                return;
            }
            TryLoadData(pathType, filePath, out Dictionary<K, V> dictionary);
            dictionary[key] = newValue;
            SaveDataInFile(dictionary, pathType, filePath);
        }

        /// <summary>
        /// Will remove the pair based on the <paramref name="key"/> from the directory located in the <paramref name="filePath"/>
        /// </summary>
        /// <typeparam name="K"></typeparam>
        /// <typeparam name="V"></typeparam>
        /// <param name="pathType"></param>
        /// <param name="filePath"></param>
        /// <param name="key"></param>
        public static void RemovePairFromDictionaryFile<K,V>(GamePathType pathType, string filePath, K key)
        {
            string path = Path.Combine(GetPathFromPathType(pathType), GetRelativePathFromPath(filePath));
            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogWarning($"Tried to remove pair of type {typeof(K)} from file: {filePath} but it does not exist!");
                return;
            }
            TryLoadData(pathType, filePath, out Dictionary<K, V> dictionary);
            dictionary.Remove(key);
            SaveDataInFile(dictionary, pathType, filePath);
        }
        #endregion
    }
}







