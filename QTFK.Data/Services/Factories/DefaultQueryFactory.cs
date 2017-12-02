using System;
using QTFK.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Attributes;

namespace QTFK.Services.Factories
{
    public class DefaultQueryFactory<T> : IQueryFactory<T> where T : new()
    {
        private readonly IQueryFactory queryFactory;
        private readonly IDBIO db;
        private readonly IEnumerable<IQueryFilterFactory> filterFactories;

        public DefaultQueryFactory(IQueryFactory queryFactory, IEnumerable<IQueryFilterFactory> filterFactories
            )
        {
            this.queryFactory = queryFactory;
            this.db = this.queryFactory.DB;
            this.filterFactories = filterFactories;

            EntityDescription = prv_buildEntityDescription(typeof(T));
        }

        private static EntityDescription prv_buildEntityDescription(Type type)
        {
            string key = null;
            PropertyInfo keyProp = null;

            var fields = type
                .GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Select(p =>
                {
                    bool isKey = p.GetCustomAttribute<KeyAttribute>() != null;

                    string fieldName;
                    var fieldAlias = p.GetCustomAttribute<AliasAttribute>();
                    if (fieldAlias != null && !string.IsNullOrWhiteSpace(fieldAlias.Name))
                        fieldName = fieldAlias.Name;
                    else
                        fieldName = p.Name;

                    if (isKey)
                    {
                        key = fieldName;
                        keyProp = p;
                    }

                    return fieldName;
                })
                .ToList()
                ;

            var alias = typeof(T).GetCustomAttribute<AliasAttribute>();

            var name = alias != null && !string.IsNullOrWhiteSpace(alias.Name)
                ? alias.Name
                : typeof(T).Name
                ;

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException($"Type {typeof(T).FullName} has no property with Key Attribute.");

            return new EntityDescription(key, keyProp, name, fields);
        }

        public IDBIO DB { get { return this.db; } }

        public string Prefix { get; set; }

        public EntityDescription EntityDescription { get; }

        public IDBQueryDelete NewDelete()
        {
            return this.queryFactory
                .NewDelete()
                .SetTable(EntityDescription.Name)
                ;
        }

        public IDBQueryInsert NewInsert()
        {
            var q = this.queryFactory
                .NewInsert()
                .SetTable(EntityDescription.Name)
                ;

            foreach (var field in EntityDescription.Fields)
                q.SetColumn(field, null, $"@{field}");

            return q;
        }

        public IDBQuerySelect NewSelect()
        {
            var q = this.queryFactory
                .NewSelect()
                .SetTable(EntityDescription.Name)
                ;

            foreach (var field in EntityDescription.Fields)
                q.AddColumn(field, null);

            return q;
        }

        public IDBQueryUpdate NewUpdate()
        {
            var q = this.queryFactory
                .NewUpdate()
                .SetTable(EntityDescription.Name)
                ;

            foreach (var field in EntityDescription.Fields)
                q.SetColumn(field, null, $"@{field}");

            return q;
        }

        public TFilter Build<TFilter>() where TFilter : class, IQueryFilterFactory
        {
            var t = typeof(TFilter);
            foreach (var factory in this.filterFactories)
            {
                if (factory.GetType().IsAssignableFrom(t))
                    return (TFilter)factory;
            }
            return null;
        }
    }
}