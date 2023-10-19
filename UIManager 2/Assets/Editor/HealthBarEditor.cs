using Codice.Client.BaseCommands;
using Game.Player;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

namespace Game.UI.EditorExtension
{
    [CustomEditor(typeof(HealthBar))]
    public class HealthBarEditor : ExtendedEditor
    {
        private static bool isReturnToDefaultTimerRunning = false;
        //private static int returnToDefaultSeconds = 4;

        //Testing Values
        private static HealthTestProfileSO profile;

        public override async void OnInspectorGUI()
        {
            DrawDefaultInspector();
            HealthBar healthBar = (HealthBar)target;

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

            if (GUILayout.Button("Test Health Lost"))
            {
                if (!CanExecuteTest(healthBar)) return;

                healthBar.UpdateHealthUI(healthBar.CurrentHealth - profile.HealthLost);
            }

            if (GUILayout.Button("Test Health Gained"))
            {
                if (!CanExecuteTest(healthBar)) return;

                healthBar.UpdateHealthUI(healthBar.CurrentHealth- profile.HealthLost);
                if (healthBar.ValueChangeFactor!=-1) await Task.Delay((int)(profile.HealthLost * healthBar.ValueChangeFactor * 1000) + 500);
                healthBar.UpdateHealthUI(healthBar.CurrentHealth + profile.HealthLost);
            }

            if (GUILayout.Button("Test Max Health Increased"))
            {
                if (!CanExecuteTest(healthBar)) return;

                int difference = Mathf.Abs(healthBar.CurrentMaxHealth - healthBar.CurrentHealth);
                healthBar.UpdateHealthUI(healthBar.CurrentMaxHealth);
                if (healthBar.ValueChangeFactor != -1) await Task.Delay((int)(difference * healthBar.ValueChangeFactor * 1000) + 500);
                healthBar.UpdateHealthUI(healthBar.CurrentMaxHealth+ profile.MaxHealthIncrease);
            }
        }

        private bool CanExecuteTest(HealthBar healthBar)
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
            if (healthBar.CurrentHealth<=0 || healthBar.CurrentMaxHealth<=0)
            {
                UnityEngine.Debug.Log($"Tried to execute health bar test, but either the current health({healthBar.CurrentHealth}) or the max health({healthBar.CurrentMaxHealth}) is not an appropriate value! " +
                    $"Both must be >0. This could be due to the fact that UpdateHealthUI() and UpdateMaxHealthUI() have not been called before the test occurred.");
                return false;
            }
            if (profile==null)
            {
                UnityEngine.Debug.Log("The health test profile SO is null! Assign one before executing tests!");
                return false;
            }
            if(profile.HealthLost==0 || profile.MaxHealthIncrease==0)
            {
                UnityEngine.Debug.LogWarning($"Tried to execute test, but at least 1 value in the test profile is 0! " +
                    $"HealthLost:{profile.HealthLost}; MaxHealthIncrease{profile.MaxHealthIncrease}");
                return false;
            }
            return true;
        }
    }
}
