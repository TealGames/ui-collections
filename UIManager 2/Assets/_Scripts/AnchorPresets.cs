using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// The anchor presets found in a RectTransform component. The names are ordered Height/Width
    /// </summary>
    [System.Serializable]
    public enum AnchorPresets
    {
        TopLeft,
        TopCenter,
        TopRight,
        TopStretch,

        MiddleLeft,
        MiddleCenter,
        MiddleRight,
        MiddleStretch,

        BottomLeft,
        BottomCenter,
        BottomRight,
        BottomStretch,

        StretchLeft,
        StretchCenter,
        StretchRight,
        StretchStretch,
    }
}
