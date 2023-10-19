using System;

namespace Game.UI
{
    /// <summary>
    /// Any class for UI containers can implement this interface in order to be show background UI blur when enabled. 
    /// This blur can only be see if the game is visible while the UI is open.
    /// </summary>
    public interface IUIBlurUser
    {
        /// <summary>
        /// Invoke this event in the implementing class when you want the blur to be enabled for that object
        /// </summary>
        public event Action OnEnableBlur;

        /// <summary>
        /// Invoke this event in the implementing class when you want the blur to be disabled for that object
        /// </summary>
        public event Action OnDisableBlur;
    }
}

