using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.DBIO.QueryFactory
{
    public static class QueryFactoryDataExtension
    {
        public static IEnumerable<T> Select<T>(this IQueryFactory<T> factory) where T : new()
        {
            return Select<T>(factory, q => { });
        }

        public static IEnumerable<T> Select<T>(this IQueryFactory<T> factory, Action<IDBQuerySelect> queryBuild) where T : new()
        {
            var query = factory.NewSelect();
            queryBuild(query);
            return factory.DB.Get<T>(query);
        }

        public static int Insert<T>(this IQueryFactory<T> factory, Action<IDBQueryInsert> queryBuild) where T : new()
        {
            var query = factory.NewInsert();
            queryBuild(query);
            return factory.DB.Set(query);
        }

        public static int Update<T>(this IQueryFactory<T> factory, Action<IDBQueryUpdate> queryBuild) where T : new()
        {
            var query = factory.NewUpdate();
            queryBuild(query);
            return factory.DB.Set(query);
        }

        public static int Delete<T>(this IQueryFactory<T> factory, Action<IDBQueryDelete> queryBuild) where T : new()
        {
            var query = factory.NewDelete();
            queryBuild(query);
            return factory.DB.Set(query);
        }
    }
}
