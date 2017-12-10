using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.EntityDescriptions
{
    public static class EntityDescriptionExtensions
    {
        public static object getId(this EntityDescription descriptor, object instance)
        {
            Asserts.isSomething(descriptor, $"Parameter '{nameof(descriptor)}' cannot be null.");
            Asserts.isSomething(instance, $"Parameter '{nameof(instance)}' cannot be null.");

            return descriptor.PropertyId.GetValue(instance);
        }

        public static void setId(this EntityDescription descriptor, object instance, object newId)
        {
            Asserts.isSomething(descriptor, $"Parameter '{nameof(descriptor)}' cannot be null.");
            Asserts.isSomething(instance, $"Parameter '{nameof(instance)}' cannot be null.");
            Asserts.isSomething(newId, $"Parameter '{nameof(newId)}' cannot be null.");

            descriptor.PropertyId.SetValue(instance, newId);
        }
    }
}
