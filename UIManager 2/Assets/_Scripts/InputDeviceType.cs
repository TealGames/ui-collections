using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Input
{
    /// <summary>
    /// The types of devices allowed in the InputSystem and used in the root of the <see cref="UnityEngine.InputSystem.InputBinding"/> paths
    /// </summary>
    [System.Serializable]
    public enum InputDeviceType
    {
        Gamepad,
        Joystick,
        Keyboard,
        Mouse,
        Pen,
        Pointer,
        Sensor,
        Touchscreen,
        TrackedDevice,
        XRController,
        XRHMD,
    }
}


