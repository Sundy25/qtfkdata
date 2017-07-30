using System;
using QTFK.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

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

        public IQueryFilter GetFilter(MethodBase method)
        {
            return _filterFactories
                .Select(f => f.Build(method, typeof(T)))
                .SingleOrDefault()
                ;
        }

        public IDBQueryDelete NewDelete()
        {
            var q = _queryFactory.NewDelete();

            throw new NotImplementedException();
        }

        public IDBQueryInsert NewInsert()
        {
            var q = _queryFactory.NewInsert();
            throw new NotImplementedException();
        }

        public IDBQuerySelect NewSelect()
        {
            var q = _queryFactory.NewSelect();
            throw new NotImplementedException();
        }

        public IDBQueryUpdate NewUpdate()
        {
            var q = _queryFactory.NewUpdate();
            throw new NotImplementedException();
        }
    }
}