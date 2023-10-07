using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Add this script to any GameObject that should have a UI tooltip in game that can show more information. 
    /// It must be added to the GameObject that has the UI component (Image, RawImage, TextMeshProUGUI, Text) that will be interacted with
    /// </summary>
    public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [TextArea(2, 5)][SerializeField] private string tooltipText;
        [SerializeField] private TooltipSettingsSO tooltipSettings;
#if UNITY_EDITOR
        public TooltipSettingsSO TooltipSettings { get => tooltipSettings; set => tooltipSettings = value; }
#endif
        /*[SerializeField] private TooltipType tooltipType;
        [SerializeField] private float enableDelay;
        [Tooltip("Sometimes, preferred width is not enough for all of the text, so we also add this buffer")][SerializeField] private float extraWidthBuffer = 50;

        [SerializeField] private bool setTooltipPositionAsObjectPosition = true;
        [Tooltip("If setTooltipPositionAsObjectPosition is true, will apply this offset to it. If false, will add this to the event hover postion")]
        [SerializeField] private Vector2 objectPositionOffset = new Vector3(0f, -50f, 0f);*/

        public enum TooltipType
        {
            MouseHover,
        }

        private void Start()
        {
            if (string.IsNullOrEmpty(tooltipText)) 
                UnityEngine.Debug.LogWarning($"Tooltip gameObject {gameObject.name} has no tooltip text defined! If it does not have any tooltips, remove the Tooltip component!");

            //If we have an image or a text component, we make sure it is a raucast 
            if (gameObject.TryGetComponent<Image>(out Image image)) image.raycastTarget = true;
            else if (gameObject.TryGetComponent<RawImage>(out RawImage rawImage)) rawImage.raycastTarget = true;
            else if (gameObject.TryGetComponent<TextMeshProUGUI>(out TextMeshProUGUI textMeshPro)) textMeshPro.raycastTarget = true;
            else if (gameObject.TryGetComponent<Text>(out Text text)) text.raycastTarget = true;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tooltipSettings.TooltipType != TooltipType.MouseHover || tooltipText=="") return;
            UnityEngine.Debug.Log("Pointer entered!");
            StartCoroutine(Delay(eventData));
        }

        private IEnumerator Delay(PointerEventData eventData)
        {
            yield return new WaitForSeconds(tooltipSettings.EnableDelay);
            EnableTooltip(eventData);
        }

        private void EnableTooltip(PointerEventData eventData)
        {
            if (tooltipSettings.SetTooltipPositionAsObjectPosition) UIManager.Instance.EnableTooltip(new TooltipInfo(tooltipText, 
                gameObject.GetComponent<RectTransform>().position + (Vector3)tooltipSettings.ObjectPositionOffset, tooltipSettings.ExtraWidthBuffer, tooltipSettings.NextLineThreshold), eventData);
            else UIManager.Instance.EnableTooltip(new TooltipInfo(tooltipText, newTooltipPosition: eventData.position + tooltipSettings.ObjectPositionOffset, 
                tooltipSettings.ExtraWidthBuffer, tooltipSettings.NextLineThreshold), eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopAllCoroutines();
            UnityEngine.Debug.Log("Pointer exited!");
            UIManager.Instance.DisableTooltip();
        }

    }

}

