using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Input
{
    /// <summary>
    /// The most common types of gamepads. Add to this list if you want more options.
    /// </summary>
    [System.Serializable]
    public enum GamepadType
    {
        None,
        Xbox,
        PlayStation,
        Switch,
        SteamDeck,
        WiiU,
        AmazonLuna,
        GoogleStadia,
        Ouya,
    }
}

