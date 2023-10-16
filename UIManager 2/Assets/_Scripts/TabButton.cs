using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    public class TabButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI title;

        public event Action OnTabClick;

        // Start is called before the first frame update
        void Start()
        {
            if (button != null) AddClickAction(() => OnTabClick?.Invoke());
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetTitle(string title) => this.title.text = title;
        public string GetTitle() => this.title.text;
        public void AddClickAction(UnityAction action) => button.onClick.AddListener(action);
    }

}
