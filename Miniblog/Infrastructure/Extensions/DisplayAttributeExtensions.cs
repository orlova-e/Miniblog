using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Web.Infrastructure.Extensions
{
    public static class DisplayAttributeExtensions
    {
        public static string GetDisplayName(this object obj, string propertyName)
        {
            Type type = obj.GetType();
            MemberInfo property = type.GetProperty(propertyName);
            if (property.GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttribute)
            {
                return displayAttribute.Name;
            }
            return string.Empty;
        }
    }
}
