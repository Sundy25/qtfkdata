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
        private IQueryFactory queryFactory;
        private IEntityDescription entityDescription;

        public EntityQueryFactory()
        {
        }

        public string Prefix { get; set; }

        public IQueryFactory QueryFactory
        {
            get
            {
                return this.queryFactory;
            }
            set
            {
                Asserts.isSomething(value, $"Value parameter for '{nameof(QueryFactory)}' cannot be null.");
                this.queryFactory = value;
            }
        }

        public IEntityDescription EntityDescription
        {
            get
            {
                return this.entityDescription;
            }
            set
            {
                Asserts.isSomething(value, $"Value parameter for '{nameof(EntityDescription)}' cannot be null.");
                this.entityDescription = value;
            }
        }

        public IDBQueryDelete newDelete()
        {
            return this.queryFactory
                .newDelete()
                .SetTable(this.entityDescription.Name)
                .SetPrefix(this.Prefix)
                ;
        }

        public IDBQueryInsert newInsert()
        {
            var q = this.queryFactory
                .newInsert()
                .SetTable(this.entityDescription.Name)
                .SetPrefix(this.Prefix)
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
                .SetPrefix(this.Prefix)
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
                .SetPrefix(this.Prefix)
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

    }
}