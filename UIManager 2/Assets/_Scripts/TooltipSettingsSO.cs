using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(fileName = "TooltipSettingsSO", menuName = "ScriptableObjects/Tooltip Settings")]
    public class TooltipSettingsSO : ScriptableObject
    {
        [field: SerializeField] public Tooltip.TooltipType TooltipType { get; private set; }
        [field: SerializeField] public float EnableDelay { get; private set; }
        [Tooltip("Sometimes, preferred width is not enough for all of the text, so we also add this buffer")][SerializeField] private float extraWidthBuffer = 50;
        public float ExtraWidthBuffer { get => extraWidthBuffer; }

        [field: SerializeField] public bool SetTooltipPositionAsObjectPosition { get; private set; } = true;
        [Tooltip("If setTooltipPositionAsObjectPosition is true, will apply this offset to it. If false, will add this to the event hover postion")]
        [SerializeField] private Vector2 objectPositionOffset = new Vector3(0f, -50f, 0f);
        public Vector2 ObjectPositionOffset { get => objectPositionOffset; }

        [Tooltip("The width of the text size needed to have text wrap to the next line")][SerializeField] private float nextLineThreshold;
        public float NextLineThreshold { get => nextLineThreshold; }
    }
}
