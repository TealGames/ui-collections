using System;

namespace Game.UI
{
    public interface IUIBlurUser
    {
        public event Action OnEnableBlur;
        public event Action OnDisableBlur;
    }
}

