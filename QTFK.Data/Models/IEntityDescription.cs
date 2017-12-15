using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace QTFK.Models
{
    public interface IEntityDescription
    {
        //string Name { get; }
        //IReadOnlyDictionary<string, PropertyInfo> Fields { get; }
        //IReadOnlyDictionary<string, PropertyInfo> Keys { get; }
        //Type Entity { get; }

        bool UsesAutoId { get; }
        bool hasId(object item);

        object build(IDataRecord record);

        void prepare(object item, IDBQueryDelete deleteQuery);
        void prepare(object item, IDBQueryInsert deleteQuery);
        void prepare(object item, IDBQueryUpdate deleteQuery);
    }
}