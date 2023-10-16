using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using System;
using System.Reflection;
using static Codice.Client.BaseCommands.BranchExplorer.ExplorerData.BrExTreeBuilder.BrExFilter;

namespace Game.Utilities.Editor
{
    public static class EditorHelperFunctions
    {
        private static string assemblyName = "UICollection";

        /// <summary>
        /// Normally when instantiating it creates a clone, but this can instantiate it as a prefab instance instead
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject InstantiateAsPrefab(GameObject prefab, Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null)
        {
            GameObject gameObject = InstantiateAsPrefab(prefab, parent: parent);

            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;
            gameObject.transform.localScale = scale;

            return gameObject;
        }

        /// <summary>
        /// Normally when instantiating it creates a clone, but this can instantiate it as a prefab instance instead
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static GameObject InstantiateAsPrefab(GameObject prefab, Transform parent = null)
        {
            GameObject gameObject;
            if (parent != null) gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            else gameObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);

            return gameObject;
        }

        public static Sprite GetSpriteAtPath(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        public static List<Assembly> GetAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.GetName().Name.StartsWith("Unity") && !assembly.GetName().Name.StartsWith("System")) assemblies.Add(assembly);
            }
            return assemblies;
        }

        public static string GetDefaultAssemblyName() => assemblyName;

        public static MonoScript FindMonoScriptByName(string name)
        {
            MonoScript foundScript = null;
            string[] guids = AssetDatabase.FindAssets("t:MonoScript");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                System.Type type = monoScript.GetClass();
                if (monoScript.name == name)
                {
                    foundScript = monoScript;
                    UnityEngine.Debug.Log($"Found monoscript {monoScript.name} when searching for it!");
                    break;
                }
            }
            return foundScript;
        }

        public static MonoScript FindMonoScriptByClassName(string className)
        {
            MonoScript foundScript = null;
            string[] guids = AssetDatabase.FindAssets("t:MonoScript");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript monoScript = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                System.Type type = monoScript.GetClass();
                if (monoScript.GetClass().Name == className)
                {
                    foundScript = monoScript;
                    UnityEngine.Debug.Log($"Found monoscript ith class name {className} when searching for it!");
                    break;
                }
            }
            return foundScript;
        }
    }
}
