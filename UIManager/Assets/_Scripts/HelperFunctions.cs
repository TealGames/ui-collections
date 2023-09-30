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

namespace Game.Utilities
{
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

        //this is an extension function meaning that it can be called without static class and since it changes gameObject's pos, that argument can be
        //omitted because is implied when calling it on a gameObject (ex. call gameObject.SetObjectPos(Vector2.Zero) )
        public static void SetObjectPos(this GameObject gameObject, Vector2 position) => gameObject.transform.position = position;

        //public static void InstantiateGameObject(this GameObject prefab, Vector2 position) => Instantiate(prefab, position, Quaternion.identity);

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
            eventData.position = Input.mousePosition;
            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}







