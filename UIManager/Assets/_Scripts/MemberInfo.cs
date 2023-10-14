using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;

namespace Game
{
    [System.Serializable]
    public class MemberInfo
    {
        public string Name { get; set; } = "";
        public FieldInfo FieldInfo { get; set; } = null;
        public PropertyInfo PropertyInfo { get; set; } = null;
        public MethodInfo MethodInfo { get; set; } = null;
        public Object ClassInstance { get; set; } = null;

        /// <summary>
        /// Constructs a field variation of member info
        /// </summary>
        /// <param name="name"></param>
        /// <param name="classInstance"></param>
        /// <param name="fieldInfo"></param>
        public MemberInfo(string name, Object classInstance, FieldInfo fieldInfo)
        {
            this.Name = name;
            this.FieldInfo = fieldInfo;
            this.ClassInstance = classInstance;
        }

        /// <summary>
        /// Constructs a property variation of member info.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="classInstance"></param>
        /// <param name="propertyInfo"></param>
        public MemberInfo(string name, Object classInstance, PropertyInfo propertyInfo)
        {
            this.Name = name;
            this.PropertyInfo = propertyInfo;
            this.ClassInstance = classInstance;
        }

        /// <summary>
        /// Constructs a method variation of member info
        /// </summary>
        /// <param name="name"></param>
        /// <param name="classInstance"></param>
        /// <param name="methodInfo"></param>
        public MemberInfo(string name, Object classInstance, MethodInfo methodInfo)
        {
            this.Name = name;
            this.MethodInfo = methodInfo;
            this.ClassInstance = classInstance;
        }

        /// <summary>
        /// Returns true if it is set, otherwise returns false. 
        /// The out argument will return the field's info if set, otherwise returns null
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <returns></returns>
        public bool HasFieldData(out FieldInfo fieldInfo)
        {
            fieldInfo = this.FieldInfo;
            return FieldInfo != null;
        }

        /// <summary>
        /// Returns true if it is set, otherwise returns false.
        /// The out argument will return the actual value stored in the field
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns></returns>
        public bool HasFieldData(out Object fieldValue)
        {
            fieldValue = FieldInfo.GetValue(ClassInstance);
            return FieldInfo != null;
        }

        /// <summary>
        /// Returns true if it is set, otherwise returns false.
        /// The out argument will return the property's info if set, otherwise returns null
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public bool HasPropertyData(out PropertyInfo propertyInfo)
        {
            propertyInfo = this.PropertyInfo;
            return PropertyInfo != null;
        }

        /// <summary>
        /// Returns true if it is set, otherwise returns false.
        /// The out argument will return the actual value stored in the property
        /// </summary>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public bool HasPropertyData(out Object propertyValue)
        {
            propertyValue = PropertyInfo.GetValue(ClassInstance);
            return PropertyInfo != null;
        }

        /// <summary>
        /// Returns true if it is set, otherwise returns false.
        /// The out argument will return the method's info if set, otherwise returns null
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public bool HasMethodData(out MethodInfo methodInfo)
        {
            methodInfo = this.MethodInfo;
            return MethodInfo != null;
        }

        /// <summary>
        /// Will invoke the method if method info is set, otherwise nothing happens. 
        /// Returns the return object of the method invoked. Note: this method has no parameters
        /// </summary>
        /// <returns></returns>
        public Object InvokeMethod()
        {
            if (MethodInfo == null) return null;
            return MethodInfo.Invoke(ClassInstance, new Object[] { });
        }

    }
}

