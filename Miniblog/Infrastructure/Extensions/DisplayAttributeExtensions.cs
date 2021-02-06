using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Web.Infrastructure.Extensions
{
    public static class DisplayAttributeExtensions
    {
        public static string GetDisplayName(this object obj)
        {
            Type type = obj.GetType();
            if (type.GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttribute)
            {
                return displayAttribute.Name;
            }
            return string.Empty;
        }
    }
}
