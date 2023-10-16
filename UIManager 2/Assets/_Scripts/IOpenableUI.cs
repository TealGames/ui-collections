using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    public interface IOpenableUI
    {
        public GameObject Container { get; set; }

        public Action OnUIEnabledAction { get; set; }
        public Action OnUIDisabledAction { get; set; }

        public UnityEvent OnUIEnabled { get; set; }
        public UnityEvent OnUIDisabled { get; set; }

        public void EnableUI();
        public void DisableUI();
    }
}

