using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    [System.Serializable]
    public class OptionSelectEnums
    {
        /// <summary>
        /// All the Quality Levels found in Edit -> Project Settings -> Quality. 
        /// If you add another level, add it to this enum. Eeach space should is represented with underscore
        /// </summary>
        public enum QualityLevels
        {
            Very_Low,
            Low,
            Medium,
            High,
            Very_High,
            Ultra,
        }

        /// <summary>
        /// Sets the Anti-Aliasing Level (Anti Aliasing blurs jagged pixels to improve curves and realism) using MSAA
        /// </summary>
        public enum AntiAliasingOptions
        {
            None=0,
            Two=2,
            Four=4,
            Eight=8,
        }
    }
}

