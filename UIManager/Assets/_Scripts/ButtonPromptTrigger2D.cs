using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Collider2D))]
    public class ButtonPromptTrigger2D : Trigger2D
    {
        //[Header("Prompt Trigger 2D")]
        [SerializeField] private ButtonPromptPresetSO triggeredPreset;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            base.OnEnter(collider);
            UIManager.Instance.GetComponentInChildren<ButtonPrompt>().EnableButtonPromptMessage(triggeredPreset);
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            base.OnExit(collider);
            UIManager.Instance.GetComponentInChildren<ButtonPrompt>().DisableButtonPromptMessage();
        }
    }
}

