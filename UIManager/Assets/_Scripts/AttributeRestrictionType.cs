using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [System.Flags]
    public enum AttributeRestrictionType
    {
        Field= 1<<0,
        Property= 1<<1,
        Method= 1<<2,
    }
}



