using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ColliderOutline : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D colliderArea;
        [SerializeField] private bool drawBarrierOutline = true;

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

