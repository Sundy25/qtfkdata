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
using QTFK.Models.QueryFilters;
using System.Linq;

namespace QTFK.Services.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : new()
    {
        private readonly IEntityDescription entityDescription;
        private readonly IExpressionParser<T> expressionParser;
        private readonly IDBIO db;
        private readonly IQueryFactory queryFactory;

        //private IEntityQueryFactory entityQueryFactory;

        //private readonly IEnumerable<IMethodParser> methodParsers;

        //public BaseRepository(
        //    //IEnumerable<IMethodParser> methodParsers
        //    )
        //{
        //    //this.methodParsers = methodParsers;
        //}

        public BaseRepository(IEntityDescriber entityDescriber, IExpressionParserFactory expressionParserFactory, IDBIO db, IQueryFactory queryFactory)
        {
            Asserts.isSomething(entityDescriber, $"Parameter '{nameof(entityDescriber)}' cannot be null.");
            Asserts.isSomething(expressionParserFactory, $"Parameter '{nameof(expressionParserFactory)}' cannot be null.");
            Asserts.isSomething(db, $"Parameter '{nameof(db)}' cannot be null.");
            Asserts.isSomething(queryFactory, $"Parameter '{nameof(queryFactory)}' cannot be null.");
            Asserts.check(this.db.getDBEngine() == this.queryFactory.getDBEngine(), $"Database engine missmatch for '{this.db.GetType().FullName}' and '{this.queryFactory.GetType().FullName}'.");

            this.db = db;
            this.queryFactory = queryFactory;
            this.entityDescription = entityDescriber.describe(typeof(T));
            this.expressionParser = expressionParserFactory.build<T>(this.entityDescription, this.queryFactory);
        }

        public void add(T item)
        {
            IDBQueryInsert insertQuery;
            IEnumerable<PropertyValue> itemValues;

            itemValues = this.entityDescription.getValues(item);
            prv_assertAutoIdIsEmpty(this.entityDescription, itemValues);
            prv_assertNonAutoIdIsFilled(this.entityDescription, itemValues);

            insertQuery = this.queryFactory.newInsert();
            insertQuery.Table = this.entityDescription.Name;
            foreach (var itemValue in itemValues)
                insertQuery.SetColumn(itemValue.Name, itemValue.Value);

            this.db.Set(cmd =>
            {
                int affected;
                object id;

                affected = cmd
                    .SetCommandText(insertQuery.Compile())
                    .AddParameters(insertQuery.Parameters)
                    .ExecuteNonQuery()
                    ;

                Asserts.check(affected == 1, $"Insert of type {typeof(T).FullName} failed. Affected rows: {affected}.");

                if (this.entityDescription.UsesAutoId)
                {
                    id = this.db.GetLastID(cmd);
                    this.entityDescription.setAutoId(id, item);
                }
            });
        }

        public void delete(T item)
        {
            int affected;
            IDBQueryDelete deleteQuery;
            IKeyFilter filter;
            IEnumerable<KeyValuePair<string, object>> keys;

            deleteQuery = this.queryFactory.newDelete();
            deleteQuery.Table = this.entityDescription.Name;
            filter = this.queryFactory.buildFilter<IKeyFilter>();
            keys = this.entityDescription.getKeyValues(item);
            filter.setKeys(keys);
            deleteQuery.Filter = filter;

            affected = this.db.Set(deleteQuery);
            Asserts.check(affected == 1, $"Failed deleting of type {typeof(T).FullName}. More than one rows affected: {affected}.");
        }

        public IEnumerable<T> get(Expression<Func<T, bool>> filterExpression)
        {
            IEnumerable<T> items;
            IDBQuerySelect selectQuery;
            IQueryFilter filter;

            //selectQuery = this.entityDescription.buildSelect();
            selectQuery = this.queryFactory.newSelect();
            filter = this.expressionParser.parse(filterExpression);
            selectQuery.SetFilter(filter);
            items = this.db.Get<T>(selectQuery, this.entityDescription.buildEntity<T>);

            return items;
        }

        public void update(T item)
        {
            IDBQueryUpdate updateQuery;

            updateQuery = this.entityDescription.buildUpdate(this.queryFactory, item);

            this.db.Set(cmd =>
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

        private static void prv_assertNonAutoIdIsFilled(IEntityDescription entityDescription, IEnumerable<PropertyValue> itemValues)
        {
            IEnumerable<PropertyValue> keys;

            keys = itemValues.Where(p => p.IsKey && !p.IsAutonumeric);
            foreach (var key in keys)
                Asserts.check(key.IsNullOrDefault == false, $"Item of type {entityDescription.Entity.FullName} must have its key '{key.Name}' setted to a non empty.");
        }

        private static void prv_assertAutoIdIsEmpty(IEntityDescription entityDescription, IEnumerable<PropertyValue> itemValues)
        {
            PropertyValue autoIdField;

            if (entityDescription.UsesAutoId)
            {
                autoIdField = itemValues.First(field => field.IsAutonumeric);
                Asserts.check(autoIdField.IsNullOrDefault, $"Item of type {entityDescription.Entity.FullName} must have its autonumeric field '{autoIdField.Name}' setted to null or default value.");
            }
        }


    }
}