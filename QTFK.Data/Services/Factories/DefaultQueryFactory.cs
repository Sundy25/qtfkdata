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
    public class DefaultQueryFactory<T> : IQueryFactory<T> where T : new()
    {
        private readonly ISelectQueryFactory _selectQueryFactory;
        private readonly IInsertQueryFactory _insertQueryFactory;
        private readonly IUpdateQueryFactory _updateQueryFactory;
        private readonly IDeleteQueryFactory _deleteQueryFactory;
        private readonly IDBIO _db;
        private readonly IEnumerable<IQueryFilterFactory> _filterFactories;

        public DefaultQueryFactory(
            ISelectQueryFactory selectQueryFactory
            , IInsertQueryFactory insertQueryFactory
            , IUpdateQueryFactory updateQueryFactory
            , IDeleteQueryFactory deleteQueryFactory
            , IEnumerable<IQueryFilterFactory> filterFactories
            )
        {
            _selectQueryFactory = selectQueryFactory;
            _insertQueryFactory = insertQueryFactory;
            _updateQueryFactory = updateQueryFactory;
            _deleteQueryFactory = deleteQueryFactory;
            _db = _selectQueryFactory.DB;
            _filterFactories = filterFactories;

            EntityDescription = BuildEntityDescription();
        }

        private EntityDescription BuildEntityDescription()
        {
            var fields = typeof(T)
                .GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Select(p =>
                {
                    var fieldAlias = p.GetCustomAttribute<AliasAttribute>();
                    if (fieldAlias != null && !string.IsNullOrWhiteSpace(fieldAlias.Name))
                        return fieldAlias.Name;

                    return p.Name;
                })
                .ToList()
                ;

            var alias = typeof(T).GetCustomAttribute<AliasAttribute>();

            var name = alias != null && !string.IsNullOrWhiteSpace(alias.Name)
                ? alias.Name
                : typeof(T).Name
                ;

            return new EntityDescription(name, fields);
        }

        public IDBIO DB { get { return _db; } }

        public string Prefix { get; set; }

        public EntityDescription EntityDescription { get; }

        public IDBQueryDelete NewDelete()
        {
            return _deleteQueryFactory
                .NewDelete()
                .SetTable(EntityDescription.Name)
                ;
        }

        public IDBQueryInsert NewInsert()
        {
            var q = _insertQueryFactory
                .NewInsert()
                .SetTable(EntityDescription.Name)
                ;

            foreach (var field in EntityDescription.Fields)
                q.SetColumn(field, null);

            return q;
        }

        public IDBQuerySelect NewSelect()
        {
            var q = _selectQueryFactory
                .NewSelect()
                .SetTable(EntityDescription.Name)
                ;

            foreach (var field in EntityDescription.Fields)
                q.AddColumn(field, null);

            return q;
        }

        public IDBQueryUpdate NewUpdate()
        {
            var q = _updateQueryFactory
                .NewUpdate()
                .SetTable(EntityDescription.Name)
                ;

            foreach (var field in EntityDescription.Fields)
                q.SetColumn(field, null);

            return q;
        }

        public TFilter Build<TFilter>() where TFilter : class, IQueryFilterFactory
        {
            var t = typeof(TFilter);
            foreach (var factory in _filterFactories)
            {
                if (factory.GetType().IsAssignableFrom(t))
                    return (TFilter)factory;
            }
            return null;
        }
    }
}