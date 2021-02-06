using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Web.Infrastructure.Extensions
{
    public static class Enums
    {
        public static string GetEnumName(object value)
        {
            Type enumType = value.GetType();
            if (!enumType.IsEnum)
                throw new ArgumentException();

            string name = Enum.GetName(enumType, value);
            FieldInfo fieldInfo = enumType.GetField(name);
            if (fieldInfo.GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttribute)
                return displayAttribute.Name;

            return name;
        }
    }
}
