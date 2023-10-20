using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using MemberInfo = Game.MemberInfo;

namespace Game
{
    /// <summary>
    /// Stores data about members from a class instance so that you use it somewhere else. 
    /// <br></br>This is most useful when you want to subscribe a method to a <see cref="UnityEngine.Events.UnityEvent"/>, which is on a gameObject that might not always be in the active scene. 
    /// <br></br>In this scenario, you can store that method's data in this SO and it will persistent that data when the gameObject with the<see cref="UnityEngine.Events.UnityEvent"/> appears, and the reference is kept, allowing the <see cref="UnityEngine.Events.UnityEvent"/> to work.
    /// </summary>
    [CreateAssetMenu(fileName = "MemberSelectionSO", menuName = "ScriptableObjects/Member Selection")]
    public class MemberSelectionSO : ScriptableObject
    {
        /// <summary>
        /// The member info and details of the choosen member. Get this property to access the value for fields/properties and invoke methods. 
        /// </summary>
        public MemberInfo SelectedMemberInfo
        {
            get
            {
                UnityEngine.Debug.Log("Attempted to get member info!");
                //if (selectedMemberInfo != null) return selectedMemberInfo;
                if (true)
                {
                    UnityEngine.Debug.Log("Member info null, trying to get it!");
                    HelperFunctions.TryLoadFullData(PATH_TYPE, MemberInfoFullPath, out string data);
                    if (string.IsNullOrEmpty(data))
                    {
                        UnityEngine.Debug.LogError($"The MemberInfo cannot be accessed because there is no data stored for {name}! " +
                            $"To check where data is stored on your device you can check Unity's API for 'Application.persistentDataPath'.");
                    }
                    List<string> separatedData = data.Split(separator).ToList();
                    //Here we save the (0) Assembly name, (1) Object ID, (2) the script type, (3) the member/attribute type, and (4) the name of the member

                    #region Error Checks
                    Assembly assembly = null;
                    try
                    {
                        assembly = Assembly.Load(separatedData[0]);
                    }
                    catch (Exception e) when (e is FileNotFoundException || e is FileLoadException || e is FileLoadException)
                    {
                        UnityEngine.Debug.LogError($"Tried to get the Member Info in MemberSelectionSO but when tying to get the stored assembly name {separatedData[0]}, something went wrong! Exception: {e}");
                        return null;
                    }
                    
                    ObjectID[] objectIDs= GameObject.FindObjectsOfType<ObjectID>(true);
                    if (objectIDs.Length==0)
                    {
                        UnityEngine.Debug.LogError($"Tried to search for ObjectIDs in MemberSelectionSO {name} to find member info data, but none exist in the current open scene!");
                        return null;
                    }
                    ObjectID foundID = objectIDs.Where(id => id.GetID() == separatedData[1]).FirstOrDefault();
                    if (foundID== null || foundID== default)
                    {
                        UnityEngine.Debug.LogError($"Tried to search for the GUID specified in data {separatedData[1]} but no ObjectID in the current open scene has that GUID!");
                        return null;
                    }

                    Type classType = assembly.GetType(separatedData[2], true);
                    if (!foundID.gameObject.TryGetComponent(classType, out Component classInstance))
                    {
                        UnityEngine.Debug.Log($"Tried to get MemberInfo in MemberSelectionSO, but ObjectID with GUID {separatedData[1]} does not have the script of type {classType}!");
                        return null;
                    }
                    #endregion

                    if (Enum.TryParse(separatedData[3], true, out AttributeRestrictionType memberType))
                    {
                        UnityEngine.Debug.Log("Enum checks");
                        switch (memberType)
                        {
                            case AttributeRestrictionType.Field:
                                FieldInfo fieldInfo = classType.GetField(separatedData[4]);
                                if (fieldInfo==null)
                                {
                                    UnityEngine.Debug.LogError($"Tried to get the MemberInfo of MemberSelectionSO with name {name}, " +
                                        $"but a field with the name {separatedData[4]} does not exist in gameObject {classInstance.gameObject.name} with script {classType}. Make sure it meets all criteria!");
                                    return null;
                                }
                                selectedMemberInfo = new MemberInfo(fieldInfo.Name, classInstance, fieldInfo);
                                break;
                            case AttributeRestrictionType.Property:
                                PropertyInfo propertyInfo = classType.GetProperty(separatedData[4]);
                                if (propertyInfo == null)
                                {
                                    UnityEngine.Debug.LogError($"Tried to get the MemberInfo of MemberSelectionSO with name {name}, " +
                                        $"but a property with the name {separatedData[4]} does not exist in gameObject {classInstance.gameObject.name} with script {classType}. Make sure it meets all criteria!");
                                    return null;
                                }
                                selectedMemberInfo = new MemberInfo(propertyInfo.Name, classInstance, propertyInfo);
                                break;
                            case AttributeRestrictionType.Method:
                                MethodInfo methodInfo = classType.GetMethod(separatedData[4]);
                                if (methodInfo == null)
                                {
                                    UnityEngine.Debug.LogError($"Tried to get the MemberInfo of MemberSelectionSO with name {name}, " +
                                        $"but a method with the name {separatedData[4]} does not exist in gameObject {classInstance.gameObject.name} with script {classType}. Make sure it meets all criteria!");
                                    return null;
                                }
                                selectedMemberInfo = new MemberInfo(methodInfo.Name, classInstance, methodInfo);
                                break;
                            default:
                                UnityEngine.Debug.LogError($"Converted {separatedData[3]} to enum type {memberType} but there is no corresponding actions for this value of the enum {typeof(AttributeRestrictionType)}!");
                                return null;
                        }
                        UnityEngine.Debug.Log($"Successfully got member info with name {selectedMemberInfo.Name}");
                        return selectedMemberInfo;
                    }
                    else
                    {
                        UnityEngine.Debug.LogError($"Tried to parse data {separatedData[3]} in MemberSelectionSO for MemberInfo, but there are no enum values in {typeof(AttributeRestrictionType)} that correspond to that string!");
                        return null;
                    }
                }
            }
            set { selectedMemberInfo = value; }
        }

        private MemberInfo selectedMemberInfo = null;

        #region File Paths and Info
        /// <summary>
        /// The full path on your computer of the SO data so that it can persistent between sessions
        /// </summary>
        public string SOFileFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(instanceID))
                {
                    UnityEngine.Debug.LogError($"Tried to get {typeof(MemberSelectionSO)}'s file full path with SO name '{name}' but its instanceID is NULL or empty!");
                    return "";
                }
                else return $"{GeneralPath}/{instanceID}/{SO_DATA_FILE_NAME}";
            }
        }

        /// <summary>
        /// The full path on your computer of the member info data that will be used when getting the stored data during runtime
        /// </summary>
        public string MemberInfoFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(instanceID))
                {
                    UnityEngine.Debug.LogError($"Tried to get {typeof(MemberSelectionSO)}'s MemberInfo full path with SO name '{name}' but its instanceID is NULL or empty!");
                    return "";
                }
                else return $"{GeneralPath}/{instanceID}/{MEMBER_INFO_FILE_NAME}";
            }
        }
        /// <summary>
        /// The full path of the directory that contains the directories for each S0 on your computer
        /// </summary>
        public string GeneralPath
        {
            //We combine the paths, but since the path of the game will always point to the assets folder, we remove that from the data path so we don't have duplicates
            get => Path.Combine(HelperFunctions.GetPathFromPathType(PATH_TYPE), DATA_UNITY_PATH.Replace("Assets/", ""));
        }

        /// <summary>
        /// The path that the directories for each SO are found in within the Unity project
        /// </summary>
        public const string SO_UNITY_PATH = "Assets/ScriptableObjects/MemberSelections";
        public const string DATA_UNITY_PATH = SO_UNITY_PATH+"/Data";
        /// <summary>
        /// The name of the file that contains the user-entered data for the SO so its data can persist between Unity sessions
        /// </summary>
        public const string SO_DATA_FILE_NAME = "so-data";

        /// <summary>
        /// The name of the file that contains the member info which is retrieved when accessing this SO's data during runtime with the <see cref="SelectedMemberInfo"/> property
        /// </summary>
        public const string MEMBER_INFO_FILE_NAME = "member-info";

        public const GamePathType PATH_TYPE = GamePathType.Game;

        public readonly string separator = HelperFunctions.DATA_SEPARATOR;
        #endregion


        [Header("Data stored in 'Application.persistentDataPath'")]
        [Space(10)]
        //Visible Properties
        [ReadOnly] [Tooltip("The instance ID of this object. Some error messages may contain this value as well as the files that stores this data, so it is here for your convience. " +
            "If it is not visible, reload all scripts by pressing SHIFT+ R or going to Shortcuts -> Recompile Scripts")]
        [SerializeField] private string instanceID;
        public string InstanceID { get => instanceID; }
        [field: SerializeField] public AttributeRestrictionType AttributeType { get; set; } = AttributeRestrictionType.Method;
        [Tooltip("This is the type of a property/field or the return type of a method")][field: SerializeField] public UserSelectedType MemberType { get; private set; } = UserSelectedType.Void;

        public void OnDisable()
        {
            UnityEngine.Debug.Log("Disabled SO");
        }

        public void OnValidate()
        {
            //instanceID = GetInstanceID().ToString();
            SetInstanceID();
        }

        public void SetInstanceID()
        {
            if (string.IsNullOrEmpty(instanceID)) instanceID = GetInstanceID().ToString();
        }
    }
}