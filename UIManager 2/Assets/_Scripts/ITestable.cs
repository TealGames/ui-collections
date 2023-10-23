using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public interface ITestable<T> where T : TestProfileSO
    {
        public bool TestOnStart { get; }   
        public T[] Profiles { get; }
        public const float TIME_BETWEEN_PROFILES = 5f;

        public IEnumerator TestProfile();
    }

}
