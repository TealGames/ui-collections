using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "HealthTestProfileSO", menuName = "ScriptableObjects/Tests/Health Test Values")]
    public class HealthTestProfileSO : ScriptableObject
    {
        //START VALUES
        /*
        [Tooltip("The baseline health when testing begins")]
        [SerializeField] private int startHealth;
        public int StartHealth { get => startHealth; }
        */

        /*
        [Tooltip("The baseline max health when testing begins")]
        [SerializeField] private int startMaxHealth;
        public int StartMaxHealth { get => startMaxHealth; }
        */

        //TESTED VALUES
        [Tooltip("The amount of health lost from the current health when testing losing health. " +
            "Also the amount of health regained when testing gaining health")]
        [SerializeField] private int healthLost;
        public int HealthLost { get => healthLost; }

        [Tooltip("The amount of health increased from the current max health when testing max health increase")]
        [SerializeField] private int maxHealthIncrease;
        public int MaxHealthIncrease { get => maxHealthIncrease; }

        private void OnValidate()
        {
            if (healthLost < 0) healthLost = Mathf.Abs(healthLost);
            if (maxHealthIncrease < 0) maxHealthIncrease = Mathf.Abs(maxHealthIncrease);
        }
    }
}