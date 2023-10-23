using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Used to display ahealth icons that update based on <see cref="Game.Player.PlayerCharacter"/> events
    /// </summary>
    public class HealthIconManager : HealthUI, ITestable<HealthTestProfileSO>
    {
        [Header("Health Icons UI")]
        [Tooltip("All the health icons for the player. It should also include any potential health icons for max health upgrades")]
        [SerializeField] private List<HealthIcon> healthIcons= new List<HealthIcon>();
        public List<HealthIcon> HealthIcons { get => healthIcons; }
        private int currentHealthIcons = 0;
        public int CurrentHealthIcons { get => currentHealthIcons; }
        
        

        [Header("Tests")]
        [SerializeField] private bool testOnStart;
        public bool TestOnStart { get => testOnStart; }

        public HealthTestProfileSO[] Profiles { get => healthTestProfiles; }
        [SerializeField] private HealthTestProfileSO[] healthTestProfiles;

        private const float TIME_BETWEEN_PROFILES= 3f;


        // Start is called before the first frame update
        void Start()
        {
            if (testOnStart && healthTestProfiles.Length > 0 && TIME_BETWEEN_PROFILES > 0f) StartCoroutine(TestProfile());
        }

        // Update is called once per frame
        void Update()
        {
            UnityEngine.Debug.Log($"Current health icons: {currentHealthIcons}");
        }

        public override void UpdateHealthUI(int newHealth)
        {
            UnityEngine.Debug.Log("Update health ui called!");
            base.UpdateHealthUI(newHealth);
            if (newHealth > healthIcons.Count)
            {
                UnityEngine.Debug.LogError($"Tried to update health UI in {gameObject.name} but the new player health {newHealth} is greater than the amount of health icons {healthIcons}!");
                return;
            }

            //Health Increase
            if (newHealth > currentHealthIcons)
            {
                for (int i = Mathf.Clamp(currentHealthIcons-1, 0, HealthIcons.Count-1) ; i < newHealth; i++)
                {
                    healthIcons[i].GainHealth();
                }
            }

            //Health Decrease
            else if (newHealth < currentHealthIcons)
            {
                for (int i= Mathf.Clamp(currentHealthIcons - 1, 0, HealthIcons.Count - 1); i>newHealth-1; i--)
                {
                    healthIcons[i].LoseHealth();
                }
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
            currentHealthIcons = newMaxHealth;
        }

        public IEnumerator TestProfile()
        {
            foreach (var profile in healthTestProfiles)
            {
                yield return new WaitForSecondsRealtime(TIME_BETWEEN_PROFILES);
                if (profile.HealthChangeType == HealthTestProfileSO.HealthType.NormalHealth)
                    UpdateHealthUI(currentHealthIcons + profile.HealthDelta);
                else if (profile.HealthChangeType == HealthTestProfileSO.HealthType.MaxHealth)
                    UpdateMaxHealthUI(currentHealthIcons + profile.HealthDelta);
            }
        }
    }
}
