using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Game.UI.EditorExtension
{
    //[CustomEditor(typeof(ExtendedButton))]
    public class ExtendedButtonEditor : ExtendedEditor
    {
        private const string assemblyTooltip = "The Assembly that the type is in. You can check an assembly by locating that script, selecting it and checking the Assembly Information. " +
            "The assembly for that script will be under the Assembly Definition field. If not, create a new one by right clicking -> Create -> Assembly Definition";

        private const string INSTANCE_KEY = "InstanceKey";
        private static int currentAssemblyIndex=-1;

        public class PersistentInfo
        {
            public int AssemblyIndex { get; set; } = -1;

            public PersistentInfo() { }
        
            public PersistentInfo(int assemblyIndex)
            {
                this.AssemblyIndex = assemblyIndex;
            }

            public string GetDataAsString()
            {
                return $"{AssemblyIndex}/";
            }
        }
        private static PersistentInfo persistentInfo = null;


        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            ExtendedButton extendedButton = (ExtendedButton)target;
            TryGetInfo();

            DrawHeader("Persistent Subscribers", false);
            DrawPlainLabel("All test health values will be used from the profile");

            Dictionary<string, Assembly> assemblies = new Dictionary<string, Assembly>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) 
                if (!assembly.GetName().Name.StartsWith("Unity") && !assembly.GetName().Name.StartsWith("System")) 
                    assemblies.Add(assembly.GetName().Name, assembly);

            EditorGUILayout.LabelField("Assembly", GUILayout.Width(138));
            GenerateTooltip(assemblyTooltip);
            persistentInfo.AssemblyIndex = EditorGUILayout.Popup(persistentInfo.AssemblyIndex, assemblies.Keys.ToArray());
            GenerateTooltip(assemblyTooltip);

            SaveInfo();
        }

        public void TryGetInfo()
        {
            string infoAsString = EditorPrefs.HasKey(target.GetInstanceID().ToString()) ? EditorPrefs.GetString(target.GetInstanceID().ToString()) : "";
            if (infoAsString == "")
            {
                UnityEngine.Debug.Log("Could not find instance id, new info created!");
                persistentInfo = new PersistentInfo();
                return;
            }
            List<string> data = infoAsString.Split("/").ToList();
            persistentInfo = new PersistentInfo(Convert.ToInt32(data[0]));
        }

        public void SaveInfo()
        {
            if (EditorPrefs.HasKey(target.GetInstanceID().ToString())) EditorPrefs.DeleteKey(target.GetInstanceID().ToString());
            EditorPrefs.SetString(target.GetInstanceID().ToString(), persistentInfo.GetDataAsString());
        }
    }
}

