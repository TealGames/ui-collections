using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Game.UI
{
    [ExecuteInEditMode]
    public class DynamicRect : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private bool setHeightFromChildren;
        [SerializeField] private float padding;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (setHeightFromChildren) FitChildren();
        }

        private void OnValidate()
        {
            //if (setHeightFromChildren) FitChildren();
        }

        private void FitChildren()
        {
            float oldY = rectTransform.anchoredPosition.y;
            float oldHeight = rectTransform.sizeDelta.y;
            float lastChildY = rectTransform.transform.GetChild(rectTransform.transform.childCount - 1).GetComponent<RectTransform>().anchoredPosition.y;
            UnityEngine.Debug.Log($"The last child {rectTransform.transform.GetChild(rectTransform.transform.childCount - 1).gameObject.name} y: {lastChildY}");
            rectTransform.SetAnchorPreset(AnchorPresets.TopCenter);

            float newHeight = Mathf.Abs(lastChildY) + padding;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, rectTransform.anchoredPosition.y + (oldHeight - newHeight) / 2);
            //rectTransform.anchoredPosition= new Vector2(rectTransform.anchoredPosition.x, currentY);
        }
    }
}

