using System;
using QTFK.Models;
using System.Collections.Generic;

namespace QTFK.Services.Factories
{
    public class DefaultQueryFactory<T> : IQueryFactory<T> where T: new()
    {
        private readonly IEnumerable<IQueryFilter> _filters;
        private readonly IQueryFactory _queryFactory;

        public DefaultQueryFactory(
            IQueryFactory queryFactory
            , IEnumerable<IQueryFilter> filters
            )
        {
            _filters = filters;
            _queryFactory = queryFactory;
        }

        public IDBIO DB => _queryFactory.DB;

        public IQueryFilter GetFilterForMethodName(string methodName, params object[] args)
        {
            throw new NotImplementedException();
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