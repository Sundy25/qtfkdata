using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.DBIO;

namespace QTFK.Extensions.Getters
{
    public static class RepositoryGetExtension
    {
        public static IEnumerable<T> GetAll<T>(this IRepository<T> repo) where T : new()
        {
            var query = repo.QueryFactory.NewSelect();
            return repo.QueryFactory.DB.Get<T>(query);
        }

        public static T GetByID<T, V>(this IRepository<T> repo, V id) where T : new()
        {
            throw new NotImplementedException();
            var query = repo.QueryFactory
                .NewSelect()
                //.SetFilter($"{repo.Entity.Key} = {id}")
                ;

            return repo.QueryFactory.DB
                .Get<T>(query)
                .SingleOrDefault()
                ;
        }

        public static IEnumerable<T> GetBetween<T, V>(this IRepository<T> repo, Func<T, V> fieldSelector, V min, V max) where T : new()
        {
            throw new NotImplementedException();
            string field = ""; //fieldSelector.GetMemberName
            var query = repo.QueryFactory
                .NewSelect()
                //.SetWhere($"{field} >= {min} AND {field} <= {max}")
                ;

            return repo.QueryFactory.DB
                .Get<T>(query)
                ;
        }
    }
}
