using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Web.Infrastructure.Extensions
{
    public static class EnumExt
    {
        public static string GetEnumDisplayName(Type enumType, object value)
        {
            if (!enumType.IsEnum)
                throw new ArgumentException();

            string name = Enum.GetName(enumType, value);
            FieldInfo property = enumType.GetField(name);
            if (property.GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttribute)
            {
                return displayAttribute.Name;
            }
            return string.Empty;
        }
    }
}
