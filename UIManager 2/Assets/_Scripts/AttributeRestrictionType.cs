using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>
    /// The type of attribute/member that can be publically gettable from a class.
    /// </summary>
    [System.Flags]
    public enum AttributeRestrictionType
    {
        Field= 1<<0,
        Property= 1<<1,
        Method= 1<<2,
    }
}



