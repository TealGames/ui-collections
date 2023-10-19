using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    /// <summary>
    /// Manages the game's main menu
    /// </summary>
    public class MainMenu : BaseUI
    {
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}

