using System;
using QTFK.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using QTFK.Extensions.DBIO.DBQueries;

namespace QTFK.Services.Factories
{
    public class DefaultQueryFactory<T> : IQueryFactory<T> where T: new()
    {
        private readonly IEnumerable<IQueryFilterFactory> _filterFactories;
        private readonly IQueryFactory _queryFactory;

        public DefaultQueryFactory(
            IQueryFactory queryFactory
            , IEnumerable<IQueryFilterFactory> filterFactories
            )
        {
            _filterFactories = filterFactories;
            _queryFactory = queryFactory;
        }

        public IDBIO DB => _queryFactory.DB;

        public string Prefix { get => _queryFactory.Prefix; set => _queryFactory.Prefix = value; }

        public IQueryFilter GetFilter(MethodBase method)
        {
            return _filterFactories
                .Select(f => f.Build(method, typeof(T)))
                .SingleOrDefault()
                ;
        }

        public IDBQueryDelete NewDelete()
        {
            return _queryFactory
                .NewDelete()
                .SetTable(typeof(T).Name)
                ;
        }

        public IDBQueryInsert NewInsert()
        {
            return _queryFactory
                .NewInsert()
                .SetTable(typeof(T).Name)
                ;
        }

        public IDBQuerySelect NewSelect()
        {
            return _queryFactory
                .NewSelect()
                .SetTable(typeof(T).Name)
                ;
        }

        public IDBQueryUpdate NewUpdate()
        {
            return _queryFactory
                .NewUpdate()
                .SetTable(typeof(T).Name)
                ;
        }
    }
}