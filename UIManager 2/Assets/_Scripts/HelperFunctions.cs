using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Events;
using Game.UI;
using PlasticGui;
using System.IO;
using Codice.Client.Commands;
using System.Text;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using static Codice.CM.Common.Serialization.PacketFileReader;

namespace Game.Utilities
{
    /// <summary>
    /// Provides lots of useful methods that abstract more complicated tasks
    /// </summary>
    public static class HelperFunctions
    {
        //since cameras only have 1 instance and will not be destroyed, we get main camera from start
        private static Camera mainCamera;
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

        //this is an extension function meaning that it can be called without static class and since it changes gameObject's pos, that argument can be
        //omitted because is implied when calling it on a gameObject (ex. call gameObject.SetObjectPos(Vector2.Zero) )
        public static void SetObjectPos(this GameObject gameObject, Vector2 position) => gameObject.transform.position = position;

        //public static void InstantiateGameObject(this GameObject prefab, Vector2 position) => Instantiate(prefab, position, Quaternion.identity);

        public static string GenerateRandomID() => System.Guid.NewGuid().ToString();

        public static int GenerateRandomPrecentage() => UnityEngine.Random.Range(0, 100);

        public static void PlaySound(this AudioClip audioClip, AudioSource audioSource, float minPitch, float maxPitch, float volume)
        {
            audioSource.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
            audioSource.PlayOneShot(audioClip, volume);
        }

        public static Quaternion LookAt2D(Vector2 forward) => Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);

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
            //we make sure that the points are ordered, (min, max)
            if (range1.x > range1.y) range1 = new Vector2(range1.y, range1.x);
            if (range2.x > range2.y) range2 = new Vector2(range2.y, range2.x);

            //UnityEngine.Debug.Log($"Checked if vertical points {range1} and {range2} overlap: {!(range2.x > range1.y && range2.y> range1.y) || (range2.x<range1.x && range2.y<range1.x)}");
            //we check if range2 is either above range1 or below range1 and then reverse it (since we are checking if they overlap)
            return !((range2.x > range1.y && range2.y > range1.y) || (range2.x < range1.x && range2.y < range1.x));
        }



        //gets the world coordinates of a UI elment give its rect transform (UI transform) (1 use case: so that you can position non-UI objects
        //to match behind UI elements, like particle effects)
        public static Vector2 GetWorldPositionOfCanvasElement(RectTransform element)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(element, element.position, MainCamera, out var result);
            return result;
        }

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

        //Note: this currently may not work
        public static T GetKey<T, V>(Dictionary<T, V> dictionary, V dictionaryValue)
        {
            T Key = default;
            foreach (KeyValuePair<T, V> pair in dictionary)
            {
                if (EqualityComparer<V>.Default.Equals(pair.Value, dictionaryValue))
                {
                    Key = pair.Key;
                    break;
                }
            }
            return Key;
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


        //scales an object around an arbitrary point rather than a gameObject's pivot
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

        //gets all of the non-zero (None value) flags in a multi-select enum
        public static List<string> GetFlagEnumValues(Enum flagEnum)
        {
            List<string> list = new List<string>();
            foreach (var flag in flagEnum.ToString().Split(","))
            {
                //if the value contains a space, find the space and remove it
                string newValue = flag.ToString().Trim();
                if (newValue.Contains(" "))
                {
                    //UnityEngine.Debug.Log($"Flag contains a space! Old value: {newValue}");

                    for (int i = 0; i < newValue.Length; i++)
                    {
                        //UnityEngine.Debug.Log($"Checking character: {newValue[i]}");
                        if (newValue[i] == ' ') newValue.Remove(i, 1);
                        //UnityEngine.Debug.Log($"Flag contains a space! New value: {newValue}");
                    }
                }
                list.Add(newValue);
                //UnityEngine.Debug.Log($"Added flag: {newValue}");
            }
            return list;
        }

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
        /// Checks is a class has anu subscribers to an event. Source is the publisher or the class that holds/invokes the event.
        /// EventHandler is the actual event, where object is the class you are checking
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
            //UnityEngine.Debug.Log($"Called TryGetUnityEvent for event: {name} in type {type.Name} for instance: {typeInstance}");
            FieldInfo[] fields = typeInstance.GetType().GetFields();
            //UnityEngine.Debug.Log($"Type: {type} has {fields.Length} fields!");
            //foreach (var field in fields) UnityEngine.Debug.Log($"When trying to get unity event found field: {field.Name}");

            List<FieldInfo> unityEvents = fields.Where(f => f.FieldType == typeof(UnityEvent)).ToList();
            //foreach (var field in unityEvents) UnityEngine.Debug.Log($"When trying to get unity event found UnityEvent: {field.Name}");

            foundEvent = null;
            foreach (FieldInfo field in unityEvents)
            {
                if (field.Name == name)
                {
                    //UnityEngine.Debug.Log($"Found target UnityEvent from fields with name: {field.Name}");
                    foundEvent = (UnityEvent)field.GetValue(typeInstance);
                    break;
                }
            }

            return foundEvent != null;
        }


        public static List<MonoBehaviour> GetAllMonoBehaviorsWithInterface(Type type)
        {
            MonoBehaviour[] monoBehaviors = UnityEngine.Object.FindObjectsOfType<MonoBehaviour>();
            return monoBehaviors.Where(behavior => behavior.GetType().GetInterfaces().Contains(type)).ToList();
        }

        public static T[] GetInterfacesOfType<T>(bool includeInactive) =>
            UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive).Where(behavoir => behavoir.GetType().GetInterfaces().Contains(typeof(T))).Cast<T>().ToArray<T>();

        public static GameObject[] GetInterfacesAsGameObjectsOfType<T>(bool includeInactive) =>
            UnityEngine.Object.FindObjectsOfType<MonoBehaviour>(includeInactive).Where(behavoir => behavoir.GetType().GetInterfaces().Contains(typeof(T))).
            Select(behavior => behavior.gameObject).ToArray<GameObject>();

        //Gets all event system raycast results of current mouse or touch position.
        public static List<RaycastResult> GetEventSystemRaycastResults()
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = UnityEngine.Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }

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

        public static bool TryLoadFullData(GamePathType pathType, string path, out string data, bool logWarningIfNotFound = true)
        {
            path = path.FormatAsSystemPath();
            string relativePath = GetRelativePathFromPath(path);
            string fullPath = GetPathFromPathType(pathType) + Path.DirectorySeparatorChar + relativePath;
            data = "";
            if (!File.Exists(fullPath) && logWarningIfNotFound)
            {
                UnityEngine.Debug.LogWarning($"Tried to load data as string in path {path} but that file does not exist!");
                return false;
            }

            using (FileStream fileStream = File.Open(fullPath, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    data = reader.ReadToEnd();
                }
            }
            return true;
        }

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

        public static bool IsStringAPath(string testedString) => testedString.Contains(Path.DirectorySeparatorChar) || testedString.Contains(Path.AltDirectorySeparatorChar);

        public static void DeleteFile(GamePathType pathType, string filePath)
        {
            filePath = filePath.FormatAsSystemPath();
            string path = Path.Combine(GetPathFromPathType(pathType), GetRelativePathFromPath(filePath));
            if (!File.Exists(path))
            {
                UnityEngine.Debug.LogWarning($"Tried to delete file: {filePath} but it does not exist!");
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
                UnityEngine.Debug.Log($"Deleting directory in path {directoryPath}");
                Directory.Delete(path, true);
            }
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

        
       
    }
}







