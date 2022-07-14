using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Dependencies
{
    [BaseTypeRequired(typeof(ScriptableObject))]
    [AttributeUsage(AttributeTargets.Class)]
    public class ResetStateOnExitPlayModeAttribute : Attribute
    {
    }
}