# if UNITY_EDITOR

using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
    public class MemberSelectionManager : MonoBehaviour
    {

        // Start is called before the first frame update
        void OnEnable()
        {
            DeleteUnsedDirectories();
            //SetInstanceID();
        }

        // Update is called once per frame
        void Update()
        {
            DeleteUnsedDirectories();
            //SetInstanceID();
        }

        private void OnValidate()
        {
            DeleteUnsedDirectories();
            //SetInstanceID();
        }

        private void DeleteUnsedDirectories()
        {
            UnityEngine.Debug.Log("Delete unsed directories");
            List<MemberSelectionSO> memberSelections = new List<MemberSelectionSO>();
            List<string> directoriesForDeletion= new List<string>();

            string directoriesLocation = (HelperFunctions.GetPathFromPathType(GamePathType.Game) + Path.DirectorySeparatorChar + MemberSelectionSO.DATA_UNITY_PATH.Replace("Asset/", "")).FormatAsSystemPath(true);
            foreach (string directory in Directory.GetDirectories(directoriesLocation))
            {
                string directoryName = directory.Split(Path.DirectorySeparatorChar).Last();
                string soDataFile = directory + $"{Path.DirectorySeparatorChar}{MemberSelectionSO.SO_DATA_FILE_NAME}";
                UnityEngine.Debug.Log($"SO data file path: {soDataFile}");
                if (HelperFunctions.TryLoadData(MemberSelectionSO.PATH_TYPE, soDataFile.FormatAsSystemPath(), out string data))
                {
                    UnityEngine.Debug.Log($"Data found {data}");
                    string[] dataSplit = data.Split(HelperFunctions.DATA_SEPARATOR);
                    string assetRelativepath = "";
                    foreach (var separatedData in dataSplit)
                    {
                        if (HelperFunctions.IsStringAPath(separatedData))
                        {
                            assetRelativepath = separatedData;
                            break;
                        }
                    }

                    if (assetRelativepath == "")
                    {
                        UnityEngine.Debug.Log($"{MemberSelectionSO.DATA_UNITY_PATH}/{directoryName}");
                        //FileUtil.DeleteFileOrDirectory($"{MemberSelectionSO.DATA_UNITY_PATH}/{directoryName}");
                        directoriesForDeletion.Add(directory);
                        continue;
                    }

                    UnityEngine.Debug.Log($"Data count found: {dataSplit.Length}");
                    UnityEngine.Debug.Log($"The path to delete asset is {assetRelativepath}");
                    MemberSelectionSO asset = AssetDatabase.LoadAssetAtPath<MemberSelectionSO>(assetRelativepath.FormatUnityPath());
                    if (asset == null)
                    {
                        UnityEngine.Debug.Log($"{MemberSelectionSO.DATA_UNITY_PATH}/{directoryName}");
                        //FileUtil.DeleteFileOrDirectory($"{MemberSelectionSO.DATA_UNITY_PATH}/{directoryName}");
                        directoriesForDeletion.Add(directory);
                        //File.Delete($"ScriptableObjects/MemberSelections/{directory}/{fileName}");
                    }
                    else
                    {
                        if (asset.InstanceID == null)
                        {
                            asset.SetInstanceID();
                            UnityEngine.Debug.Log($"Set instance id of {asset.name}");
                        }

                        //AssetDatabase.RenameAsset(assetRelativepath.FormatUnityPath(), instanceID);
                        if (directoryName != asset.InstanceID)
                        {
                            UnityEngine.Debug.Log($"Last folder: {directory.Split(Path.DirectorySeparatorChar).Last()}, asset instance ID: {asset.InstanceID}");
                            //Directory.Move(directory, directoriesLocation + Path.DirectorySeparatorChar + asset.InstanceID);

                            AssetDatabase.RenameAsset($"{MemberSelectionSO.DATA_UNITY_PATH}/{directoryName}", asset.InstanceID);
                        }

                    }
                }
            }
            AssetDatabase.Refresh();
            string[] assetPaths = Directory.GetFiles(directoriesLocation, "*.asset");
            foreach (string fileInfo in assetPaths) memberSelections.Add(AssetDatabase.LoadAssetAtPath<MemberSelectionSO>(fileInfo));

            foreach (var directory in directoriesForDeletion) Directory.Delete(directory, false);

            /*
            foreach (var dir in directories) UnityEngine.Debug.Log($"Found directory {dir}");
            foreach (var sel in memberSelections) UnityEngine.Debug.Log($"Found SO {sel}");

            if (directories.Length == 0 || memberSelections.Count == 0) return;

            
            List<string> directoriesForDeletion= new List<string>();
            foreach (var directory in directories)
            {
                bool hasSelection = false;
                foreach (var selection in memberSelections)
                {
                    if (directory.Contains(selection.GetInstanceID().ToString()))
                    {
                        hasSelection = true;
                        break;
                    }
                }
                if (!hasSelection)
                {
                    UnityEngine.Debug.Log($"Deleting directory {directory}");
                    directoriesForDeletion.Add(directory);
                }
            }
            //foreach (var directory in directoriesForDeletion) Directory.Delete(directory, true);
            */
        }

        public void SetInstanceID()
        {
            foreach (MemberSelectionSO asset in AssetDatabase.LoadAllAssetsAtPath(MemberSelectionSO.SO_UNITY_PATH))
            {
                if (asset.InstanceID == null) GetInstanceID();
            }
        }
    }
}
#endif
