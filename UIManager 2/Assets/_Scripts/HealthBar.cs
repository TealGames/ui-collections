using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Used to display a health bar that updates based on <see cref="Game.Player.PlayerCharacter"/> events
    /// </summary>
    public class HealthBar : HealthUI, ITestable<HealthTestProfileSO>
    {
        [Header("Health Bar")]
        [Tooltip("The main health slider that displays the current health")]
        [SerializeField] private Slider healthSlider;
        
        [Tooltip("If true, will gradually change the value of the health bar (whether increasing max health or updating normal health). By default, it will just set the value instantly if this is false")]
        [SerializeField] private bool doGradualValueChange;
        [Tooltip("If doGradualValueChange is true, the time that it takes to update to the new health will be the absolute difference of the old health and new health times this factor. 0.1 will be fast, 2 will be slow")]
        [Range(0.01f, 2f)][SerializeField] private float valueChangeFactor;

        private int currentHealth = 0;
        private int currentMaxHealth = 0;
        private float widthPer1Health = 0;

        [Header("Health Delta")]
        [Tooltip("If true, will display the change in health whether losing or gaining health")]
        [SerializeField] private bool displayHealthDelta;
        [SerializeField] private Slider healthDeltaSlider;
        [Tooltip("The fill image that displays the value of the slider for health changes")]
        [SerializeField] private Image healthDeltaFillImage;
        private float healthDeltaFillImageAlpha = 1f;
        [Tooltip("WHen health is lost ONLY, the amount of time before the health difference slider will begin its animation")]
        [SerializeField] private float healthDeltaAnimationDelay;
        private enum HealthAnimation
        {
            FadeAway,
            GradualDecrease,
        }
        [Tooltip("The type of animation for the health difference slider when health is lost ONLY. Since health gaining would cover up the health difference slider, it has no animation. " +
            "If FadeAway, then will fade the fill area's alpha to 0 in the time calculated by the absolute difference of the old health and new health times valueChangeFactor." +
            "If GradualDecrease, then will gradually decrease the value to the current health the same as the regular health slider if doGradualValueChange is true.")]
        [SerializeField] private HealthAnimation healthDeltaAnimation;

        /// <summary>
        /// The width of the slider that represents 1 health value. 
        /// Used to calculate new health bar sizes when max health is increased
        /// </summary>
        public float WidthPer1Health { get => widthPer1Health; }
        public int CurrentHealth { get => currentHealth; }
        public int CurrentMaxHealth { get => currentMaxHealth; }

        /// <summary>
        /// The value change factor of this healthBar. 
        /// Returns -1 if gradualValueChange is false, otherwise returns the factor value
        /// </summary>
        public float ValueChangeFactor
        {
            get
            {
                if (doGradualValueChange) return valueChangeFactor;
                else return -1;
            }
        }


        [Header("Tests")]
        [SerializeField] private bool testOnStart;
        public bool TestOnStart { get => testOnStart; }

        public HealthTestProfileSO[] Profiles { get => healthTestProfiles; }
        [SerializeField] private HealthTestProfileSO[] healthTestProfiles;

        private const float TIME_BETWEEN_PROFILES = 10f;

        // Start is called before the first frame update
        void Awake()
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = 1;
            healthSlider.value = 1;

            healthDeltaFillImageAlpha = healthDeltaFillImage.color.a;
        }

        private void Start()
        {
            if (testOnStart && healthTestProfiles.Length > 0 && TIME_BETWEEN_PROFILES > 0) StartCoroutine(TestProfile());
        }

        // Update is called once per frame
        void Update()
        {
            UnityEngine.Debug.Log($"Current health is {currentHealth}. Current max health is {currentMaxHealth}");
        }

        public override void UpdateHealthUI(int newHealth)
        {
            if (currentMaxHealth==0)
            {
                UnityEngine.Debug.LogError($"Tried to update health UI on {gameObject.name} with a newHealth value of {newHealth} but the max health has not been set yet! " +
                    $"UpdateMaxHealthUI() must be called at least once before any UpdateHealthUI() calls!");
                return;
            }
            

            newHealth = Mathf.Clamp(newHealth, 0, currentMaxHealth);
            int oldHealth = currentHealth;
            currentHealth = newHealth;

            base.UpdateHealthUI(newHealth);
            float newSlidervalue= (float)newHealth / (float)currentMaxHealth;
            UnityEngine.Debug.Log($"New slider value is: {newSlidervalue}: {newHealth} / {currentMaxHealth}");
            int difference = Mathf.Abs(oldHealth - newHealth);

            if (!doGradualValueChange) healthSlider.value = newSlidervalue;
            else StartCoroutine(LerpValue(healthSlider, newSlidervalue, difference * valueChangeFactor));

            //SHOW THE HEALTH CHANGE DIFFERENCE
            if (displayHealthDelta)
            {
                StopCoroutine(HealthDeltaAnimationDelay());
                healthDeltaFillImage.color = new Color(healthDeltaFillImage.color.r, healthDeltaFillImage.color.g, healthDeltaFillImage.color.b, healthDeltaFillImageAlpha);
                
                //Only do the animation if we lose health since that is only when the animation will be visible
                //(because the current health slider will cover it up when gaining health)
                if (newHealth < oldHealth)
                {
                    healthDeltaSlider.value = (float)oldHealth / (float)currentMaxHealth;
                    StartCoroutine(HealthDeltaAnimationDelay());
                }
                else if (newHealth> oldHealth) healthDeltaSlider.value = (float)newHealth / (float)currentMaxHealth;
            }

            IEnumerator HealthDeltaAnimationDelay()
            {
                yield return new WaitForSecondsRealtime(healthDeltaAnimationDelay);
                if (healthDeltaAnimation == HealthAnimation.GradualDecrease)
                    StartCoroutine(LerpValue(healthDeltaSlider, newSlidervalue, difference * valueChangeFactor));
                else if (healthDeltaAnimation == HealthAnimation.FadeAway)
                    StartCoroutine(UIManager.Instance.LerpImageAlpha(healthDeltaFillImage, difference * valueChangeFactor, 0f));
            }
        }

        public override void UpdateMaxHealthUI(int newMaxHealth)
        {
            base.UpdateMaxHealthUI(newMaxHealth);
            RectTransform rect = healthSlider.GetComponent<RectTransform>();
            if (widthPer1Health == 0) widthPer1Health= rect.sizeDelta.x / newMaxHealth;
            else
            {
                Vector2 newSize= new Vector2(newMaxHealth * widthPer1Health, rect.sizeDelta.y);
                int difference = Mathf.Abs(currentMaxHealth - newMaxHealth);

                if (!doGradualValueChange) rect.sizeDelta = newSize;
                else StartCoroutine(UIManager.Instance.LerpRectSize(rect, rect.sizeDelta, newSize, difference* valueChangeFactor));
            }

            currentMaxHealth=newMaxHealth;
            currentHealth = currentMaxHealth;
        }

        private IEnumerator LerpValue(Slider slider, float endValue, float time)
        {
            float elapsedTime = 0;
            float startValue = slider.value;
            UnityEngine.Debug.Log($"Lerping between {startValue} to {endValue}");
            while (elapsedTime < time)
            {
                elapsedTime+= Time.unscaledDeltaTime;
                slider.value= Mathf.Lerp(startValue, endValue, elapsedTime/time);
                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Will reset the max health to the current max health. 
        /// Useful if the rect transform was changed (while the current max health was not updated) and you need to reset the rect transform to the actual value defined
        /// </summary>
        public void RevertMaxHealth() => UpdateMaxHealthUI(currentMaxHealth);

        /// <summary>
        /// Will reset the health UI to the current health. 
        /// Useful if the health bar value was changed (while the current health was not updated) and you need to reset the value to the actual value defined
        /// </summary>
        public void RevertHealthUI() => UpdateHealthUI(currentHealth);

        public IEnumerator TestProfile()
        {
            foreach (var profile in healthTestProfiles)
            {
                yield return new WaitForSecondsRealtime(TIME_BETWEEN_PROFILES);
                if (profile.HealthChangeType == HealthTestProfileSO.HealthType.NormalHealth)
                    UpdateHealthUI(currentHealth + profile.HealthDelta);
                else if (profile.HealthChangeType == HealthTestProfileSO.HealthType.MaxHealth)
                    UpdateMaxHealthUI(currentHealth + profile.HealthDelta);
            }
        }
    }
}

