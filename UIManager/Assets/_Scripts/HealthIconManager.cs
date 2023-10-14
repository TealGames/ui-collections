using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class HealthIconManager : HealthUI
    {
        [Header("Health Icons UI")]
        [Tooltip("All the health icons for the player. It should also include any potential health icons for max health upgrades")]
        [SerializeField] private List<HealthIcon> healthIcons= new List<HealthIcon>();
        public List<HealthIcon> HealthIcons { get => healthIcons; }
        private int currentHealthIcons = 0;
        public int CurrentHealthIcons { get => currentHealthIcons; }

        // Start is called before the first frame update
        void Start()
        {
            /*
            if (healthIconParent != null)
            {
                for (int i=0; i< healthIconParent.transform.childCount; i++)
                {
                    if (healthIconParent.transform.GetChild(i).TryGetComponent<HealthIcon>(out HealthIcon healthIcon)) healthIcons.Add(healthIcon);
                    else
                    {
                        UnityEngine.Debug.LogError($"There was a gameObject with the name {gameObject.name} that does not have a HealthIcon.cs script and is part of the HealthIcon system! " +
                            $"Only HealthIcons should be part of the health icon parent '{healthIconParent.gameObject.name}'");
                    }
                }
            }
            */
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void UpdateHealthUI(int newHealth)
        {
            base.UpdateHealthUI(newHealth);
            if (newHealth > healthIcons.Count)
            {
                UnityEngine.Debug.LogError($"Tried to update health UI in {gameObject.name} but the new player health {newHealth} is greater than the amount of health icons {healthIcons}!");
                return;
            }

            //Health Increase
            if (newHealth > currentHealthIcons)
            {
                foreach(var icon in healthIcons) icon.GainHealth();
            }

            //Health Decrease
            else if (newHealth < currentHealthIcons)
            {
                foreach (var icon in healthIcons) icon.LoseHealth();
            }
            currentHealthIcons = newHealth;
        }

        public override void UpdateMaxHealthUI(int newMaxHealth)
        {
            base.UpdateMaxHealthUI(newMaxHealth);
            if (newMaxHealth > healthIcons.Count)
            {
                UnityEngine.Debug.LogError($"Tried to update max health UI in {gameObject.name} but the new max health {newMaxHealth} is greater than the amount of health icons {healthIcons}!");
                return;
            }

            //Enable or disable the health icons based on the max health
            for (int i=0; i< healthIcons.Count; i++)
            {
                if (i < newMaxHealth) healthIcons[i].gameObject.SetActive(true);
                else healthIcons[i].gameObject.SetActive(false);
            }
        }
    }
}
