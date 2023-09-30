using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class TabContainer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tabTitle;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetTitle(string title) => tabTitle.text = title;
    }
}

