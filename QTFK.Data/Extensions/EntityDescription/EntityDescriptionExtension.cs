using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.EntityDescription
{
    public static class EntityDescriptionExtension
    {
        public static IEnumerable<KeyValuePair<string, PropertyInfo>> getKeysAndFields(this IEntityDescription entityDescription)
        {
            foreach (var pair in entityDescription.Keys)
                yield return pair;

            foreach (var pair in entityDescription.Fields)
                yield return pair;
        }
    }
}
