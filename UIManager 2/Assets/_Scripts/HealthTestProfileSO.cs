using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// A profile that can be used when testing health UI objects such as <see cref="Game.UI.HealthBar"/> and <see cref="Game.UI.HealthIconManager"/>
    /// </summary>
    [CreateAssetMenu(fileName = "HealthTestProfileSO", menuName = "ScriptableObjects/Tests/Health Test Values")]
    public class HealthTestProfileSO : TestProfileSO
    {
        //TESTED VALUES
        [SerializeField] private int healthDelta;
        public int HealthDelta { get => healthDelta; }

        [System.Serializable]
        public enum HealthType
        {
            NormalHealth,
            MaxHealth,
        }
        [SerializeField] private HealthType healthChangeType;
        public HealthType HealthChangeType { get=> healthChangeType; }
    }
}