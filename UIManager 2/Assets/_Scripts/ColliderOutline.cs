using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Will draw an outline of a rectangular area in the Editor, which can be useful for having an area reference even when the GameObject is not selected
    /// </summary>
    public class ColliderOutline : MonoBehaviour
    {
        [Tooltip("The collider that should be drawn")][SerializeField] private BoxCollider2D colliderArea;
        [Tooltip("If true, will draw an outline of the collider in the Editor even when this GameObject is not selected. " +
            "This is helpful for having collider area references when this GameObject is not selected")][SerializeField] private bool drawBarrierOutline = true;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnDrawGizmos()
        {
            if (colliderArea == null || !drawBarrierOutline) return;

            float horizontal = colliderArea.bounds.size.x;
            float vertical = colliderArea.bounds.size.y;
            Vector3 min = colliderArea.bounds.min;
            Vector3 max = colliderArea.bounds.max;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(min, new Vector2(min.x, min.y + vertical));
            Gizmos.DrawLine(new Vector2(min.x, min.y + vertical), max);
            Gizmos.DrawLine(max, new Vector2(max.x, max.y - vertical));
            Gizmos.DrawLine(new Vector2(max.x, max.y - vertical), min);
        }
    }
}

