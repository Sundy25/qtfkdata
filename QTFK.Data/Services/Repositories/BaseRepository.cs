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
        private readonly IExpressionParser<T> expressionParser;
        //private IEntityQueryFactory entityQueryFactory;

        //private readonly IEnumerable<IMethodParser> methodParsers;

        //public BaseRepository(
        //    //IEnumerable<IMethodParser> methodParsers
        //    )
        //{
        //    //this.methodParsers = methodParsers;
        //}

        public BaseRepository(IEntityDescriber entityDescriber, IExpressionParserFactory expressionParserFactory)
        {
            Asserts.isSomething(entityDescriber, $"Parameter '{nameof(entityDescriber)}' cannot be null.");
            Asserts.isSomething(expressionParserFactory, $"Parameter '{nameof(expressionParserFactory)}' cannot be null.");

            this.entityDescription = entityDescriber.describe(typeof(T));
            this.expressionParser = expressionParserFactory.build<T>(this.entityDescription);
        }

        public IDBIO DB { get; set; }
        public IQueryFactory QueryFactory { get; set; }

        public void add(T item)
        {
            IDBQueryInsert insertQuery;
            object id;

            prv_prepareEngine();

            id = null;
            insertQuery = this.entityDescription.buildInsert( this.QueryFactory, item);

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
            this.entityDescription.setAutoId(id, item);
        }

        public void delete(T item)
        {
            int affected;
            IDBQueryDelete deleteQuery;

            prv_prepareEngine();

            deleteQuery = this.entityDescription.buildDelete(this.QueryFactory, item);

            affected = this.DB.Set(deleteQuery);
            Asserts.check(affected == 1, $"Failed deleting of type {typeof(T).FullName}. More than one rows affected: {affected}.");
        }

        public IEnumerable<T> get(Expression<Func<T, bool>> filterExpression)
        {
            IEnumerable<T> items;
            IDBQuerySelect selectQuery;
            IQueryFilter filter;

            prv_prepareEngine();

            //selectQuery = this.entityDescription.buildSelect();
            selectQuery = this.QueryFactory.newSelect();
            filter = this.expressionParser.parse(filterExpression);
            selectQuery.SetFilter(filter);
            items = this.DB.Get<T>(selectQuery, this.entityDescription.buildEntity<T>);

            return items;
        }

        public void update(T item)
        {
            IDBQueryUpdate updateQuery;

            prv_prepareEngine();

            updateQuery = this.entityDescription.buildUpdate(this.QueryFactory, item);

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

            this.expressionParser.QueryFactory = this.QueryFactory;
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