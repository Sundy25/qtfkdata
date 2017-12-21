using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq.Expressions;

namespace QTFK.Models
{
    public interface IEntityDescription
    {
        //string Name { get; }
        //IReadOnlyDictionary<string, PropertyInfo> Fields { get; }
        //IReadOnlyDictionary<string, PropertyInfo> Keys { get; }
        Type Entity { get; }

        bool UsesAutoId { get; }
        string Name { get; }

        object buildEntity(IDataRecord record);

        IDBQueryUpdate buildUpdate(IQueryFactory queryFactory, object item);

        void setAutoId(object id, object item);
        string getField(PropertyInfo property);
        IEnumerable<KeyValuePair<string, object>> getKeyValues(object item);
        IEnumerable<PropertyValue> getValues(object item);
    }
}