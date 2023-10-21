using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Player
{
    /// <summary>
    /// Manages the player. Only one ever exists in a scene. Access the Singleton Instance with <see cref="PlayerCharacter.Instance"/>
    /// </summary>
    public sealed class PlayerCharacter : MonoBehaviour
    {
        [Header("Health")]
        [Tooltip("The max health of the player by default (on Start())")]
        [SerializeField] private int defaultMaxHealth;

        private int currentMaxHealth = 0;
        /// <summary>
        /// The current max health of the player. If a player gets a health increase, this should be updated! When this is set, the player will get all their health back (if it was lost)
        /// </summary>
        public int CurrentMaxHealth
        {
            get => currentMaxHealth;
            private set
            {
                currentMaxHealth = value;
                OnMaxHealthChange?.Invoke(currentMaxHealth);
                OnMaxHealthUpdated?.Invoke(currentMaxHealth);

                //Remove this if you do not want to regain all health when a player gets health upgrade
                CurrentHealth = currentMaxHealth;
            }
        }

        /// <summary>
        /// Where int is the new max health;
        /// </summary>
        public event Action<int> OnMaxHealthChange;

        /// <summary>
        /// Where int is the new max health;
        /// </summary>
        public UnityEvent<int> OnMaxHealthUpdated;


        private int currentHealth = 0;
        public int CurrentHealth
        {
            get => currentHealth;
            private set
            {
                currentHealth = Mathf.Clamp(value, 0, currentMaxHealth);
                OnHealthChange?.Invoke(currentHealth);
                OnHealthUpdated?.Invoke(currentHealth);
            }
        }

        /// <summary>
        /// Where int is the new health amount
        /// </summary>
        public event Action<int> OnHealthChange;

        /// <summary>
        /// Where int is the new health amount
        /// </summary>
        public UnityEvent<int> OnHealthUpdated;


        public static PlayerCharacter Instance { get; private set; }

        // Start is called before the first frame update
        void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            CurrentMaxHealth = defaultMaxHealth;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

