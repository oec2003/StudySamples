using System;
using System.Reflection;

namespace Office2PDF.Extension
{
    public static class AttributeExtension
    {
        public static T GetAttribute<T>(this object obj) where T : class
        {
            return obj.GetType().GetAttribute<T>();
        }
        public static T GetAttribute<T>(this Type type) where T : class
        {
            Attribute customAttribute = type.GetCustomAttribute(typeof(T));
            if (customAttribute.IsNotNull())
            {
                return (customAttribute as T);
            }
            return default(T);
        }
    }
}
