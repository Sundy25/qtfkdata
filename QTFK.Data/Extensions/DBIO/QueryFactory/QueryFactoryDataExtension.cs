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
        public static IEnumerable<T> Select<T>(this IEntityQueryFactory factory) where T : new()
        {
            return prv_select<T>(factory, null);
        }

        public static IEnumerable<T> Select<T>(this IEntityQueryFactory factory, Action<IDBQuerySelect> queryBuild) where T : new()
        {
            return prv_select<T>(factory, queryBuild);
        }
        private static IEnumerable<T> prv_select<T>(IEntityQueryFactory factory, Action<IDBQuerySelect> queryBuild) where T : new()
        {
            var query = factory.newSelect();
            queryBuild?.Invoke(query);
            return factory.DB.Get<T>(query);
        }

        public static int Insert<T>(this IEntityQueryFactory factory, Action<IDBQueryInsert> queryBuild) where T : new()
        {
            var query = factory.newInsert();
            queryBuild(query);
            return factory.DB.Set(query);
        }

        public static int Update<T>(this IEntityQueryFactory factory, Action<IDBQueryUpdate> queryBuild) where T : new()
        {
            var query = factory.newUpdate();
            queryBuild(query);
            return factory.DB.Set(query);
        }

        public static int Delete<T>(this IEntityQueryFactory factory, Action<IDBQueryDelete> queryBuild) where T : new()
        {
            var query = factory.newDelete();
            queryBuild(query);
            return factory.DB.Set(query);
        }
    }
}
