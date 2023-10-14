using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class HealthBar : HealthUI
    {
        [Header("Health Bar")]
        [SerializeField] private Slider healthSlider;
        [Tooltip("If true, will gradually change the value of the health bar (whether increasing max health or updating normal health)")]
        [SerializeField] private bool doGradualValueChange;
        [Tooltip("If doGradualValueChange is true, the time that it takes to update to the new health will be the absolute difference of the old health and new health times this factor. 0.1 will be fast, 2 will be slow")]
        [Range(0.1f, 2f)][SerializeField] private float valueChangeFactor;

        private int currentHealth = 0;
        private int currentMaxHealth = 0;
        private float widthPer1Health = 0;

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

        // Start is called before the first frame update
        void Awake()
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = 1;
            healthSlider.value = 1;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void UpdateHealthUI(int newHealth)
        {
            if (currentMaxHealth==0)
            {
                UnityEngine.Debug.LogError($"Tried to update health UI on {gameObject.name} with a newHealth value of {newHealth} but the max health has not been set yet! " +
                    $"UpdateMaxHealthUI() must be called at least once before any UpdateHealthUI() calls!");
                return;
            }

            base.UpdateHealthUI(newHealth);
            float newSlidervalue= newHealth / currentMaxHealth;
            int difference = Mathf.Abs(currentHealth - newHealth);

            if (!doGradualValueChange) healthSlider.value = newSlidervalue;
            else StartCoroutine(LerpValue(newSlidervalue, difference * valueChangeFactor));
            currentHealth = newHealth;
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
        }

        private IEnumerator LerpValue(float endValue, float time)
        {
            float elapsedTime = 0;
            float startValue = healthSlider.value;
            while (elapsedTime < time)
            {
                elapsedTime+= Time.unscaledDeltaTime;
                healthSlider.value= Mathf.Lerp(startValue, endValue, elapsedTime/time);
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
    }
}

