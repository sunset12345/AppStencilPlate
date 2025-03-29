using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace YYDev.UserLabel
{
    [AttributeUsage(AttributeTargets.Method)]
    [ComVisible(true)]
    public sealed class UserLabelGetterAttribute : Attribute
    {
        private readonly string _field;
        public string Field => _field;

        public UserLabelGetterAttribute(string field)
        {
            _field = field;
        }
    }
}

