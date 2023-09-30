using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.UI
{
    [CreateAssetMenu(fileName = "MemberSelectionSO", menuName = "ScriptableObjects/Member Selection")]
    public class MemberSelectionSO : ScriptableObject
    {
        public MemberInfo SelectedMemberInfo { get; set; } = null;

        [field: SerializeField] public bool AllowFields { get; set; } = false;
        [field: SerializeField] public bool AllowProperties { get; set; } = false;

        [Tooltip("Only methods with no parameters will be found")]
        [SerializeField] private bool allowMethods = false;
        public bool AllowMethods
        {
            get { return allowMethods; }
            set { allowMethods = value; }
        }

        [Space(10)]
        [Tooltip("This is the type of a property/field or the return type of a method")][SerializeField] private UserSelectedType memberType;
        public UserSelectedType MemberType { get => memberType;}
    }
}