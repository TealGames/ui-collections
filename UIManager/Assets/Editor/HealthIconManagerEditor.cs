using Game.Player;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Game.UI
{
    [CustomEditor(typeof(HealthIconManager))]
    public class HealthIconManagerEditor : ExtendedEditor
    {
        private static int returnToDefaultSeconds = 4;
        private static bool isReturnToDefaultTimerRunning = false;
        private static HealthTestProfileSO profile;
        public override async void OnInspectorGUI()
        {
            DrawDefaultInspector();
            HealthIconManager healthManager = (HealthIconManager)target;

            EditorGUILayout.Space(20);
            DrawHeader("Tests", false);
            try
            {
                profile = EditorGUILayout.ObjectField("Test Profile", profile, typeof(HealthTestProfileSO), allowSceneObjects: false) as HealthTestProfileSO;
            }
            catch (ExitGUIException e)
            {
                return;
            }
            
            DrawPlainLabel("All test health values will be used from the profile");
            DrawInspectorWarning("Note: Internal health and displayed UI health may get out of sync when testing");

            if (GUILayout.Button("Test All Health Gained"))
            {
                if (!CanExecuteTest()) return;

                foreach (var icon in healthManager.HealthIcons) icon.GainHealth();
                await ReturnToOldHealth();
            }

            if (GUILayout.Button("Test All Health Lost"))
            {
                if (!CanExecuteTest()) return;

                foreach (var icon in healthManager.HealthIcons) icon.LoseHealth();
                await ReturnToOldHealth();
            }

            if (GUILayout.Button("Test Max Health Gained"))
            {
                if (!CanExecuteTest()) return;
            
                foreach (var icon in healthManager.HealthIcons)
                {
                    icon.gameObject.SetActive(false);
                    icon.gameObject.SetActive(true);
                }
                await ReturnToOldHealth();
            }

            async Task ReturnToOldHealth()
            {
                isReturnToDefaultTimerRunning = true;
                await Task.Delay(returnToDefaultSeconds * 1000);
                healthManager.UpdateHealthUI(PlayerCharacter.Instance.CurrentHealth);
                isReturnToDefaultTimerRunning = false;
            }
        }

        private bool CanExecuteTest()
        {
            if (!Application.isPlaying)
            {
                UnityEngine.Debug.LogWarning("Testing all health icons only works during play mode!");
                return false;
            }
            if (isReturnToDefaultTimerRunning)
            {
                UnityEngine.Debug.LogWarning("Wait for the current test to be done!");
                return false;
            }
            return true;
        }  
    }
}

