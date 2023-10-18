using System;

namespace Game.UI
{
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

