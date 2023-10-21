//Original from: https://forum.unity.com/threads/shortcut-key-for-lock-inspector.95815/#:~:text=You%20can%20do%20it%20like%20this.%20Code%20%28csharp%29%3A,%2F%2F%20Ctrl%20%2B%20L%20static%20void%20ToggleInspectorLock%20%28%29

using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;
using Assembly = System.Reflection.Assembly;
using Editor = UnityEditor.Editor;

using System.Collections.Generic;
using UnityEditor.Compilation;

namespace Game.Utilities
{
    internal class InspectorLockToggle
    {
        private static EditorWindow _mouseOverWindow;

        //IMPORTANT: %= ctrl, #= shift, &= alt, just regular key= _(key name)

        [MenuItem("Shortcuts/Select Inspector under mouse cursor (use hotkey) #&q")]
        private static void SelectLockableInspector()
        {
            if (EditorWindow.mouseOverWindow.GetType().Name == "InspectorWindow")
            {
                _mouseOverWindow = EditorWindow.mouseOverWindow;
                Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
                Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(type);
                int indexOf = findObjectsOfTypeAll.ToList().IndexOf(_mouseOverWindow);
                EditorPrefs.SetInt("LockableInspectorIndex", indexOf);
            }
        }

        [MenuItem("Shortcuts/Toggle Lock #l")]
        private static void ToggleInspectorLock()
        {
            if (_mouseOverWindow == null)
            {
                if (!EditorPrefs.HasKey("LockableInspectorIndex"))
                    EditorPrefs.SetInt("LockableInspectorIndex", 0);
                int i = EditorPrefs.GetInt("LockableInspectorIndex");

                Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
                Object[] findObjectsOfTypeAll = Resources.FindObjectsOfTypeAll(type);
                _mouseOverWindow = (EditorWindow)findObjectsOfTypeAll[i];
            }

            if (_mouseOverWindow != null && _mouseOverWindow.GetType().Name == "InspectorWindow")
            {
                Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.InspectorWindow");
                PropertyInfo propertyInfo = type.GetProperty("isLocked");
                bool value = (bool)propertyInfo.GetValue(_mouseOverWindow, null);
                propertyInfo.SetValue(_mouseOverWindow, !value, null);
                _mouseOverWindow.Repaint();
            }
        }

        [MenuItem("Shortcuts/Clear Console #&c")]
        private static void ClearConsole()
        {
            Type type = Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditorInternal.LogEntries");
            type.GetMethod("Clear").Invoke(null, null);
        }

        [MenuItem("Shortcuts/Select Children #e")]
        private static void SelectAllChildren()
        {
            List<GameObject> allChildren = new List<GameObject>();
            List<GameObject> allSelected = new List<GameObject>(Selection.gameObjects);
            Selection.objects = null;

            foreach (var selectedObject in allSelected)
            {
                //if they have children
                if (selectedObject.transform.childCount > 0)
                {
                    //add each children to the all children
                    foreach (Transform child in selectedObject.transform)
                    {
                        if (child.gameObject != selectedObject.gameObject)
                        {
                            allChildren.Add(child.gameObject);
                        }
                    }
                }
            }


            Selection.objects = allChildren.ToArray();
        }

        [MenuItem("Shortcuts/Recompile Scripts #r")]
        private static void Recompile()
        {
            CompilationPipeline.RequestScriptCompilation();
        }

    }
}

