using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Describes the type of <see cref="ButtonPrompt"/>, specifically the location of the prompt on the UI screen. 
    /// You can create your own custom type of prompt identification by just changing the values
    /// </summary>
    public enum ButtonPromptType
    {
        BottomCenter,
        UpperRight,
        UpperLeft,
        Center,
        MiddleLeft,
        MiddleRight,
    }
}
