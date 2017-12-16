using QTFK.Services;
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

        object build(IDataRecord record);

        IDBQueryDelete buildDelete(IQueryFactory queryFactory, object item);
        IDBQueryInsert buildInsert(IQueryFactory queryFactory, object item);
        IDBQueryUpdate buildUpdate(IQueryFactory queryFactory, object item);
        void setId(object id, object item);
    }
}