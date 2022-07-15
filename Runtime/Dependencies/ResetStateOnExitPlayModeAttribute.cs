using System;
using JetBrains.Annotations;
using UnityEngine;

namespace DGF.Dependencies
{
    [BaseTypeRequired(typeof(ScriptableObject))]
    [AttributeUsage(AttributeTargets.Class)]
    public class ResetStateOnExitPlayModeAttribute : Attribute
    {
    }
}