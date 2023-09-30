using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ObjectID : MonoBehaviour
    {
        [Header("Object ID")]
        [Tooltip("In order for the save system to remember what object was isCollected or not, it saves the isCollected object's id. " +
            "To generate a random id, right click on script and select 'Generate guid for id'. A guid is a string of 32 random characters with low chance of repeating ids. ")]
        [SerializeField] private string id = "";

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid() => id = System.Guid.NewGuid().ToString();

        public string GetID() => id;
    }
}


