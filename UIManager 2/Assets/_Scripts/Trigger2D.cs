using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The base class for any trigger for 2D
    /// </summary>
    public class Trigger2D : MonoBehaviour
    {
        [Header("Trigger")]
        [SerializeField] private LayerMask triggerMask;
        [SerializeField][Tooltip("On enter, it turns inactive")] protected bool disableOnEnter;
        [SerializeField][Tooltip("On exit, it turns inactive")] protected bool disableOnExit;

        protected bool inTrigger = false;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        protected void OnEnter(Collider2D collider)
        {
            if (collider != null && (triggerMask.value & (1 << collider.gameObject.layer)) != 0)
            {
                inTrigger = true;
                if (disableOnEnter) gameObject.SetActive(false);
            }
        }

        protected void OnExit(Collider2D collider)
        {
            if (collider != null && (triggerMask.value & (1 << collider.gameObject.layer)) != 0)
            {
                inTrigger = false;
                if (disableOnExit) gameObject.SetActive(false);
            }
        }
    }
}
