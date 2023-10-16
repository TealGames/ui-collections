using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game
{
    [ExecuteInEditMode]
    public class MemberSelectionManager : MonoBehaviour
    {
        private readonly string memberSelectionsPath = "Assets/ScriptableObjects/MemberSelections";

        // Start is called before the first frame update
        void OnEnable()
        {
            DeleteUnsedDirectories();
        }

        // Update is called once per frame
        void Update()
        {
            DeleteUnsedDirectories();
        }

        private void OnValidate()
        {
            DeleteUnsedDirectories();
        }

        private void DeleteUnsedDirectories()
        {
            UnityEngine.Debug.Log("Delete unsed directories");
            string dataStoragePath = Application.persistentDataPath;

            string[] directories = Directory.GetDirectories(dataStoragePath);

            List<MemberSelectionSO> memberSelections = new List<MemberSelectionSO>();

            string[] assetPaths = Directory.GetFiles(memberSelectionsPath, "*.asset");
            foreach (string fileInfo in assetPaths) memberSelections.Add(AssetDatabase.LoadAssetAtPath<MemberSelectionSO>(fileInfo));


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
            foreach (var directory in directoriesForDeletion) Directory.Delete(directory, true);
        }
    }
}

