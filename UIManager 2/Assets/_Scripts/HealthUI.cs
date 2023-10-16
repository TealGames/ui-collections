using Game.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class HealthUI : MonoBehaviour
    {
        //[Header("HealthUI")]
        //[SerializeField]

        // Start is called before the first frame update
        void Start()
        {
            //PlayerCharacter.Instance.OnHealthChange += UpdateHealthUI;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void UpdateHealthUI(int newHealth)
        {

        }

        public virtual void UpdateMaxHealthUI (int newMaxHealth)
        {

        }
    }

}
