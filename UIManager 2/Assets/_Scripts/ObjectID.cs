using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// Adds a unique ID to a gameObject that can be used to access it as long as it is in the scene
    /// </summary>
    [ExecuteInEditMode]
    public class ObjectID : MonoBehaviour
    {
        [Header("Object ID")]
        [Tooltip("In order for the save system to remember what object was isCollected or not, it saves the isCollected object's id. " +
            "To generate a random id, right click on script and select 'Generate guid for id'. A guid is a string of 32 random characters with low chance of repeating ids. To copy it, Right Click on the text field and select 'Copy'")]
        [ReadOnly][SerializeField] private string id = "";

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid() => id = HelperFunctions.GenerateRandomID();

        public string GetID() => id;

        public void Start()
        {
            if (id == "") GenerateGuid();
        }
    }
}


