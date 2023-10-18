using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// Extends the basic uses of the <see cref="Button"/> in order to allow more dynamic setups and customizability
    /// </summary>
    public class ExtendedButton : MonoBehaviour
    {
        [SerializeField] private Button button;

        [Header("Persistent Subscribers")]
        [SerializeField] private List<MemberSelectionSO> persistentSubscribers = new List<MemberSelectionSO>();
        // Start is called before the first frame update
        void Start()
        {
            foreach (var subscriber in persistentSubscribers)
            {
                UnityEngine.Debug.Log($"Added subscriber to {gameObject.name}. Method Info: name: {subscriber.SelectedMemberInfo.Name}; class instance {subscriber.SelectedMemberInfo.ClassInstance} ");
                AddOnClickAction(() =>
                {
                    subscriber.SelectedMemberInfo.InvokeMethod();
                    UnityEngine.Debug.Log("Invoked method!");
                });
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnValidate()
        {
            if (persistentSubscribers.Count > 0)
            {
                List<MemberSelectionSO> removedSubscribers = new List<MemberSelectionSO>(); 
                foreach (var subscriber in persistentSubscribers)
                {
                    if (subscriber==null) continue;
                    if ((subscriber.AttributeType & AttributeRestrictionType.Property & AttributeRestrictionType.Field) != 0) removedSubscribers.Add(subscriber);
                }
                foreach (var removedSubscriber in removedSubscribers)
                {
                    UnityEngine.Debug.LogError($"Removed persistent subscriber {removedSubscriber.name} because it does not have method data! Only methods are allowed!");
                    persistentSubscribers.Remove(removedSubscriber);
                }
            }
        }

        /// <summary>
        /// Will add an action to this button. Note: this will NOT show up in the inspector because it is anonymous
        /// </summary>
        /// <param name="action"></param>
        public void AddOnClickAction(UnityAction action) => button.onClick.AddListener(action);
    }

}
