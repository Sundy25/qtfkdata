using System;
using QTFK.Models;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Attributes;

namespace QTFK.Services.Factories
{
    public class EntityQueryFactory : IEntityQueryFactory
    {
        private IDBIO db;
        private IQueryFactory queryFactory;
        private Type entity;
        private EntityDescription entityDescription;

        public EntityQueryFactory()
        {

            //EntityDescription = prv_buildEntityDescription(typeof(T));
        }

        public string Prefix { get; set; }

        public IDBIO DB
        {
            get
            {
                Asserts.isSomething(this.db, "DB not established!");
                return this.db;
            }
            set
            {
                Asserts.isSomething(value, "New DB cannot be null!");
                this.db = value;
            }
        }

        public IQueryFactory QueryFactory
        {
            get
            {
                Asserts.isSomething(this.queryFactory, "QueryFactory not established!");
                return this.queryFactory;
            }
            set
            {
                Asserts.isSomething(value, "New QueryFactory cannot be null!");
                this.queryFactory = value;
            }
        }

        public Type Entity
        {
            get
            {
                Asserts.isSomething(this.entity, "Entity not established!");
                return this.entity;
            }
            set
            {
                Asserts.isSomething(value, "New Entity cannot be null!");
                this.entity = value;
                this.entityDescription = prv_buildEntityDescription(this.entity);
            }
        }

        public EntityDescription EntityDescription
        {
            get
            {
                Asserts.isSomething(this.entity, "Entity not established!");
                return this.entityDescription;
            }
        }

        public IDBQueryDelete newDelete()
        {
            return this.queryFactory
                .newDelete()
                .SetTable(this.entityDescription.Name)
                ;
        }

        public IDBQueryInsert newInsert()
        {
            var q = this.queryFactory
                .newInsert()
                .SetTable(this.entityDescription.Name)
                ;

            foreach (var field in this.entityDescription.Fields)
                q.SetColumn(field, null, $"@{field}");

            return q;
        }

        public IDBQuerySelect newSelect()
        {
            var q = this.queryFactory
                .newSelect()
                .SetTable(this.entityDescription.Name)
                ;

            foreach (var field in this.entityDescription.Fields)
                q.AddColumn(field, null);

            return q;
        }

        public IDBQueryUpdate newUpdate()
        {
            var q = this.queryFactory
                .newUpdate()
                .SetTable(this.entityDescription.Name)
                ;

            foreach (var field in this.entityDescription.Fields)
                q.SetColumn(field, null, $"@{field}");

            return q;
        }

        //public TFilter Build<TFilter>() where TFilter : class, IQueryFilterFactory
        //{
        //    var t = typeof(TFilter);
        //    foreach (var factory in this.filterFactories)
        //    {
        //        if (factory.GetType().IsAssignableFrom(t))
        //            return (TFilter)factory;
        //    }
        //    return null;
        //}

        public IQueryFilter buildFilter(Type type)
        {
            return this.queryFactory.buildFilter(type);
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

            var alias = type.GetCustomAttribute<AliasAttribute>();

            var name = alias != null && !string.IsNullOrWhiteSpace(alias.Name)
                ? alias.Name
                : type.Name
                ;

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException($"Type {type.FullName} has no property with Key Attribute.");

            return new EntityDescription(key, keyProp, name, fields);
        }

    }
}