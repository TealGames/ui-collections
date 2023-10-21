using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Management
{
    /// <summary>
    /// Manages the game and game states. Only one ever exists in a scene. Access the Singleton Instance with <see cref="GameManager.Instance"/>
    /// </summary>
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
