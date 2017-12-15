using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.EntityDescription
{
    public static class EntityDescriptionExtension
    {
        //public static IEnumerable<KeyValuePair<string, PropertyInfo>> getKeysAndFields(this IEntityDescription entityDescription)
        //{
        //    foreach (var pair in entityDescription.Keys)
        //        yield return pair;

        //    foreach (var pair in entityDescription.Fields)
        //        yield return pair;
        //}

        public static T build<T>(this IEntityDescription entityDescription, IDataRecord record) where T: new()
        {
            return (T)entityDescription.build(record);
        }
    }
}
