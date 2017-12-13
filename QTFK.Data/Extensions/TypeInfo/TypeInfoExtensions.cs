using QTFK.Attributes;
using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.TypeInfo
{
    public static class TypeInfoExtensions
    {
        public static string getNameOrAlias(this Type type)
        {
            AliasAttribute alias;

            Asserts.isSomething(type, $"Parameter '{nameof(type)}' cannot be null.");

            alias = type.GetCustomAttribute<AliasAttribute>();

            return alias?.Name ?? type.Name;
        }

        public static string getNameOrAlias(this PropertyInfo property)
        {
            AliasAttribute alias;

            Asserts.isSomething(property, $"Parameter '{nameof(property)}' cannot be null.");

            alias = property.GetCustomAttribute<AliasAttribute>();

            return alias?.Name ?? property.Name;
        }

        public static bool isKey(this PropertyInfo property)
        {
            Asserts.isSomething(property, $"Parameter '{nameof(property)}' cannot be null.");

            return property.GetCustomAttribute<KeyAttribute>() != null;
        }
    }
}
