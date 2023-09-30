using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.UI
{
    public class UIManager: MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Editor")]
        [Tooltip("When \"Replace Scene TMPro Text With Override\" button clicked below, ALL TMPro Text GameObjects will have their font replaced with this one")] 
        [SerializeField] private TMP_FontAsset overrideFontAsset = null;

        public TMP_FontAsset OverrideFontAsset { get => overrideFontAsset; }


        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }

        /// <summary>
        /// Gradually increases or decreases an image component's color alpha. Note: targetAlpha must be between 0-1. 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="time"></param>
        /// <param name="newAlpha"></param>
        /// <returns></returns>
        public IEnumerator LerpImageAlpha(Image image, float time, float newAlpha, bool disableOnEnd = false, bool enableOnEnd = false)
        {
            Color originalColor = image.color;
            float elapsedTime = 0;
            float targetAlpha = Mathf.Clamp(newAlpha, 0, 1);

            //enable in case it is deactivated
            image.gameObject.SetActive(true);

            while (elapsedTime < time)
            {
                //we use unscaled time since if the game has 0 timeScale, we can still successfully call this
                elapsedTime += Time.unscaledDeltaTime;
                image.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, targetAlpha, elapsedTime / time));
                yield return new WaitForEndOfFrame();
            }

            if (disableOnEnd) image.gameObject.SetActive(false);
            else if (enableOnEnd) image.gameObject.SetActive(true);
        }


        /// <summary>
        /// Gradually increases or decreases an textMeshPro component's color alpha. Note: targetAlpha must be between 0-1. 
        /// </summary>
        /// <param name="image"></param>
        /// <param name="time"></param>
        /// <param name="newAlpha"></param>
        /// <returns></returns>
        public IEnumerator LerpTextAlpha(TextMeshProUGUI text, float time, float newAlpha, bool disableOnEnd = false, bool enableOnEnd = false)
        {
            Color originalColor = text.color;
            float elapsedTime = 0;
            float targetAlpha = Mathf.Clamp(newAlpha, 0, 1);

            //make sure to enable in case it is deactivated for some reason
            text.gameObject.SetActive(true);

            while (elapsedTime < time)
            {
                //we use unscaled time since if the game has 0 timeScale, we can still successfully call this
                elapsedTime += Time.unscaledDeltaTime;
                text.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(originalColor.a, targetAlpha, elapsedTime / time));
                yield return new WaitForEndOfFrame();
            }

            if (disableOnEnd) text.gameObject.SetActive(false);
            else if (enableOnEnd) text.gameObject.SetActive(true);
        }

    }

    
}


