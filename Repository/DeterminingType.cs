using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

//[assembly: InternalsVisibleTo("Repo.UnitTests")]

namespace Repo
{
    /// <summary>
    /// Determines whether this type is a database model type
    /// </summary>
    internal static class DeterminingType
    {
        internal static Type Determine(string type)
        {
            if (string.IsNullOrEmpty(type))
                throw new ArgumentNullException();

            PropertyInfo[] propertyInfos = typeof(MiniblogDb)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => pi.PropertyType.IsGenericType)
                .ToArray();

            Type searchable = null;

            foreach (var propertyInfo in propertyInfos)
            {
                if (propertyInfo.PropertyType.GetGenericArguments()[0].Name == type)
                {
                    searchable = propertyInfo.PropertyType.GetGenericArguments()[0];
                    break;
                }
            }

            if (searchable == null)
            {
                throw new ArgumentException($"{type} type not found");
            }

            return searchable;
        }
    }
}
