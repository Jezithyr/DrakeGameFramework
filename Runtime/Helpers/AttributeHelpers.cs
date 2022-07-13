using System;
using System.Reflection;
using Sirenix.Utilities;

namespace Helpers
{
    public static class AttributeHelpers
    {
        public static bool HasAttribute<T>(this ICustomAttributeProvider member) where T : Attribute
        {
            return member.HasAttribute(typeof(T));
        }

        public static bool HasAttribute(this ICustomAttributeProvider member, Type type)
        {
            // TODO optimize
            foreach (var attribute in member.GetAttributes())
            {
                if (attribute.GetType() == type)
                {
                    return true;
                }
            }

            return false;
        }
    }
}