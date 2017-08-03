using System;
using QTFK.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.Objects.Manipulator;
using QTFK.Attributes;

namespace QTFK.Services.Factories
{
    public class DefaultQueryFactory<T> : IQueryFactory<T> where T: new()
    {
        private readonly IEnumerable<IQueryFilterFactory> _filterFactories;
        private readonly IQueryFactory _queryFactory;
        private readonly IEnumerable<string> _typeFields;
        private readonly string _typeName;

        public DefaultQueryFactory(
            IQueryFactory queryFactory
            , IEnumerable<IQueryFilterFactory> filterFactories
            )
        {
            _filterFactories = filterFactories;
            _queryFactory = queryFactory;

            _typeFields = typeof(T)
                .GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Select(p =>
                {
                    var alias = p.GetCustomAttribute<AliasAttribute>();
                    if (alias != null && !string.IsNullOrWhiteSpace(alias.Name))
                        return alias.Name;

                    return p.Name;
                })
                .ToList()
                ;

            {
                var alias = typeof(T).GetCustomAttribute<AliasAttribute>();

                _typeName = alias != null && !string.IsNullOrWhiteSpace(alias.Name)
                    ? alias.Name
                    : typeof(T).Name
                    ;
            }
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
                .SetTable(_typeName)
                ;
        }

        public IDBQueryInsert NewInsert()
        {
            var q = _queryFactory
                .NewInsert()
                .SetTable(_typeName)
                ;

            foreach (var field in _typeFields)
                q.SetColumn(field, null);

            return q;
        }

        public IDBQuerySelect NewSelect()
        {
            var q = _queryFactory
                .NewSelect()
                .SetTable(_typeName)
                ;

            foreach (var field in _typeFields)
                q.AddColumn(field, null);

            return q;
        }

        public IDBQueryUpdate NewUpdate()
        {
            var q = _queryFactory
                .NewUpdate()
                .SetTable(_typeName)
                ;

            foreach (var field in _typeFields)
                q.SetColumn(field, null);

            return q;
        }
    }
}