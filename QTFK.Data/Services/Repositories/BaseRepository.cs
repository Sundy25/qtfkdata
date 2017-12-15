using System.Collections.Generic;
using QTFK.Models;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DBIO.EngineAttribute;
using System;
using System.Linq.Expressions;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.EntityDescription;

namespace QTFK.Services.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : new()
    {
        private readonly IEntityDescription entityDescription;
        //private IEntityQueryFactory entityQueryFactory;

        //private readonly IEnumerable<IMethodParser> methodParsers;

        //public BaseRepository(
        //    //IEnumerable<IMethodParser> methodParsers
        //    )
        //{
        //    //this.methodParsers = methodParsers;
        //}

        public BaseRepository(IEntityDescriber entityDescriber)
        {
            Asserts.isSomething(entityDescriber, $"Parameter '{nameof(entityDescriber)}' cannot be null.");

            this.entityDescription = entityDescriber.describe(typeof(T));
        }

        public IDBIO DB { get; set; }
        public IQueryFactory QueryFactory { get; set; }

        public void add(T item)
        {
            IDBQueryInsert insertQuery;
            object id;

            prv_prepareEngine();

            if (this.entityDescription.UsesAutoId)
                Asserts.check(this.entityDescription.hasId(item) == false, $"Because of type '{typeof(T).FullName}' has autonumeric Id, parameter '{nameof(item)}' must have no id in order to add to repository.");
            else
                Asserts.check(this.entityDescription.hasId(item) == true, $"Because of type '{typeof(T).FullName}' has no autonumeric Id, parameter '{nameof(item)}' must have setted id in order to add to repository.");

            id = null;
            insertQuery = this.QueryFactory.newInsert();
            this.entityDescription.prepare(item, insertQuery);

            this.DB.Set(cmd =>
            {
                int affected;

                affected = cmd
                    .SetCommandText(insertQuery.Compile())
                    .AddParameters(insertQuery.Parameters)
                    .ExecuteNonQuery()
                    ;

                Asserts.check(affected == 1, $"Insert of type {typeof(T).FullName} failed. Affected rows: {affected}.");

                if (this.entityDescription.UsesAutoId)
                    id = this.DB.GetLastID(cmd);
            });
            this.entityDescription.setId(id, item);
        }

        public void delete(T item)
        {
            int affected;
            IDBQueryDelete deleteQuery;

            prv_prepareEngine();

            deleteQuery = this.QueryFactory.newDelete();
            this.entityDescription.prepare(item, deleteQuery);

            affected = this.DB.Set(deleteQuery);
            Asserts.check(affected == 1, $"Failed deleting of type {typeof(T).FullName}. More than one rows affected: {affected}.");
        }

        public IEnumerable<T> get(Expression<Func<T, bool>> filterExpression)
        {
            IEnumerable<T> items;
            IDBQuerySelect selectQuery;
            IQueryFilter filter;

            prv_prepareEngine();

            selectQuery = this.QueryFactory.newSelect();
            filter = this.QueryFactory.buildFilter(filterExpression);
            selectQuery.SetFilter(filter);
            items = this.DB.Get<T>(selectQuery, this.entityDescription.build<T>);

            return items;
        }

        public void update(T item)
        {
            IDBQueryUpdate updateQuery;

            prv_prepareEngine();

            Asserts.check(this.entityDescription.hasId(item), $"Parameter '{nameof(item)}' must have setted id in order to update repository.");

            updateQuery = this.QueryFactory.newUpdate();
            this.entityDescription.prepare(item, updateQuery);

            this.DB.Set(cmd =>
            {
                int affected;

                affected = cmd
                    .SetCommandText(updateQuery.Compile())
                    .AddParameters(updateQuery.Parameters)
                    .ExecuteNonQuery()
                    ;

                Asserts.check(affected == 1, $"Failed updating of type {typeof(T).FullName}. More than one rows affected: {affected}.");
            });
        }

        private void prv_prepareEngine()
        {
            Asserts.isSomething(DB, $"Property '{nameof(DB)}' not established.");
            Asserts.isSomething(QueryFactory, $"Property '{nameof(QueryFactory)}' not established.");
            Asserts.check(DB.getDBEngine() == QueryFactory.getDBEngine(), $"Database engine missmatch for '{DB.GetType().FullName}' and '{QueryFactory.GetType().FullName}'.");
        }

        //protected IQueryFilter GetFilter(MethodBase method)
        //{
        //    return this.methodParsers
        //        .Select(p => p.Parse(method, this.entityQueryFactory.EntityDescription, this.entityQueryFactory))
        //        .SingleOrDefault()
        //        ;
        //}

        //protected IQueryFilter GetFilter<T1>(MethodBase method) where T1 : struct
        //{
        //    return this.methodParsers
        //        .Select(p => p.Parse<T1>(method, this.entityQueryFactory.EntityDescription, this.entityQueryFactory))
        //        .SingleOrDefault()
        //        ;
        //}


    }
}