using System;
using System.Reflection;

namespace Helpers
{
    public static class AttributeHelpers
    {
        public static bool HasAttribute<T>(this ICustomAttributeProvider member, bool inherit = true) where T : Attribute
        {
            return member.IsDefined(typeof(T), inherit);
        }
    }
}