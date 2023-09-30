using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace Game.UI
{
    [System.Serializable]
    public class MemberInfo
    {
        public string Name { get; set; } = "";
        public FieldInfo FieldInfo { get; set; } = null;
        public PropertyInfo PropertyInfo { get; set; } = null;
        public MethodInfo MethodInfo { get; set; } = null;
        public Object ClassInstance { get; set; } = null;

        public MemberInfo(string name, Object classInstance, FieldInfo fieldInfo)
        {
            this.Name = name;
            this.FieldInfo = fieldInfo;
            this.ClassInstance = classInstance;
        }

        public MemberInfo(string name, Object classInstance, PropertyInfo propertyInfo)
        {
            this.Name = name;
            this.PropertyInfo = propertyInfo;
            this.ClassInstance = classInstance;
        }

        public MemberInfo(string name, Object classInstance, MethodInfo methodInfo)
        {
            this.Name = name;
            this.MethodInfo = methodInfo;
            this.ClassInstance = classInstance;
        }

        public bool HasFieldData(out FieldInfo fieldInfo)
        {
            fieldInfo = this.FieldInfo;
            return FieldInfo != null;
        }

        public bool HasFieldData(out Object fieldValue)
        {
            fieldValue = FieldInfo.GetValue(ClassInstance);
            return FieldInfo != null;
        }
        

        public bool HasPropertyData(out PropertyInfo propertyInfo)
        {
            propertyInfo = this.PropertyInfo;
            return PropertyInfo != null;
        }

        public bool HasPropertyData(out Object propertyValue)
        {
            propertyValue = PropertyInfo.GetValue(ClassInstance);
            return PropertyInfo != null;
        }

        public bool HasMethodData(out MethodInfo methodInfo)
        {
            methodInfo = this.MethodInfo;
            return MethodInfo != null;
        }

        public Object InvokeMethod()
        {
            if (MethodInfo != null) return null;
            return MethodInfo.Invoke(ClassInstance, new Object[] { });
        }

    }
}

