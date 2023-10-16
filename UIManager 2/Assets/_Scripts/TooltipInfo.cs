using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    [System.Serializable]
    public class TooltipInfo
    {
        public string Text { get; private set; } = "";
        public Vector3? NewTooltipPosition { get; private set; } = null;
        public float? ExtraBuffer { get; private set; } = null;
        public float NextLineThreshold { get; private set; }

        public TooltipInfo(string text, Vector3 newTooltipPosition, float extraBuffer, float nextLineThreshold)
        {
            Text = text;
            this.NewTooltipPosition = newTooltipPosition;
            this.ExtraBuffer = extraBuffer;
            this.NextLineThreshold = nextLineThreshold;
        }

        public TooltipInfo(string text, float nextLineThreshold)
        {
            Text = text;
            this.NextLineThreshold = nextLineThreshold;
        }
    }
}

