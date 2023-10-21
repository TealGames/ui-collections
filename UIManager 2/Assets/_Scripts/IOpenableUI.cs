using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

namespace Game.UI
{
    /// <summary>
    /// Any UI that can be opened and closed can implement this interface to require to include some basic UnityEvents, Action events as well as some methods and a property
    /// </summary>
    public interface IOpenableUI
    {
        /// <summary>
        /// The container that holds all the UI elements for a UI system. 
        /// This can be beneficial to allow the parent class to still be running while the container is disabled
        /// </summary>
        public GameObject Container { get; set; }

        public Action OnUIEnabledAction { get; set; }
        public Action OnUIDisabledAction { get; set; }

        public UnityEvent OnUIEnabled { get; set; }
        public UnityEvent OnUIDisabled { get; set; }

        public void EnableUI();
        public void DisableUI();
    }
}


