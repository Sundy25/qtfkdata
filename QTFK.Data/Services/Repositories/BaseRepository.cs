using System.Collections.Generic;
using QTFK.Models;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DBIO.EngineAttribute;
using System;
using System.Linq.Expressions;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Models.QueryFilters;
using System.Linq;
using System.Data;

namespace QTFK.Services.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : new()
    {
        private readonly IEntityDescription entityDescription;
        private readonly IExpressionParser expressionParser;
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
            Asserts.check(db.getDBEngine() == queryFactory.getDBEngine(), $"Database engine missmatch for '{db.GetType().FullName}' and '{queryFactory.GetType().FullName}'.");

            this.db = db;
            this.queryFactory = queryFactory;
            this.entityDescription = entityDescriber.describe(typeof(T));
            this.expressionParser = expressionParserFactory.build(this.entityDescription, this.queryFactory);
            Asserts.isSomething(this.entityDescription, $"Field '{nameof(this.entityDescription)}' is null.");
            Asserts.isSomething(this.expressionParser, $"Field '{nameof(this.expressionParser)}' is null.");
        }

        public void add(T item)
        {
            IDBQueryInsert insertQuery;
            IEnumerable<IPropertyValue> itemValues;

            itemValues = this.entityDescription.getValues(item);
            prv_assertAutoIdKeyAreEmpty(itemValues);
            prv_assertNoAutoIdKeysHaveValue(itemValues);

            insertQuery = this.queryFactory.newInsert();
            insertQuery.Table = this.entityDescription.Name;
            foreach (var itemValue in itemValues)
                insertQuery.column(itemValue.Name, itemValue.Value);

            this.db.Set(cmd =>
            {
                int affected;
                object id;
                IPropertyDescription autoIdField;

                affected = cmd
                    .SetCommandText(insertQuery.Compile())
                    .AddParameters(insertQuery.getUniqueParameters())
                    .ExecuteNonQuery();

                Asserts.check(affected == 1, $"Insert of type {this.entityDescription.Entity.FullName} failed. Affected rows: {affected}.");

                if (this.entityDescription.UsesAutoId)
                {
                    id = this.db.GetLastID(cmd);
                    autoIdField = itemValues.First(field => field.IsAutonumeric && field.IsKey);
                    autoIdField.Property.SetValue(item, id);
                }
            });
        }

        public void delete(T item)
        {
            IDBQueryDelete deleteQuery;
            IKeyFilter filter;
            IEnumerable<IPropertyValue> keys;

            keys = this.entityDescription
                .getValues(item)
                .Where(v => v.IsKey);

            prv_assertKeysHaveValue(keys);

            deleteQuery = this.queryFactory.newDelete();
            deleteQuery.Table = this.entityDescription.Name;
            filter = this.queryFactory.buildFilter<IKeyFilter>();

            foreach (var key in keys)
                filter.setKey(key.Name, key.Value);

            deleteQuery.Filter = filter;

            this.db.Set(cmd =>
            {
                int affected;

                affected = cmd
                    .SetCommandText(deleteQuery.Compile())
                    .AddParameters(deleteQuery.getUniqueParameters())
                    .ExecuteNonQuery();

                Asserts.check(affected == 1, $"Delete of type {this.entityDescription.Entity.FullName} failed. Affected rows: {affected}.");
            });
        }

        public void update(T item)
        {
            IDBQueryUpdate updateQuery;
            IKeyFilter filter;
            IEnumerable<IPropertyValue> itemValues, keys, values;

            itemValues = this.entityDescription.getValues(item);
            prv_assertKeysHaveValue(itemValues);

            updateQuery = this.queryFactory.newUpdate();
            updateQuery.Table = this.entityDescription.Name;
            filter = this.queryFactory.buildFilter<IKeyFilter>();

            keys = itemValues.Where(field => field.IsKey);
            foreach (var key in keys)
                filter.setKey(key.Name, key.Value);

            values = itemValues.Where(field => !field.IsKey);
            foreach (var value in values)
                updateQuery.column(value.Name, value.Value);

            updateQuery.Filter = filter;

            this.db.Set(cmd =>
            {
                int affected;

                affected = cmd
                    .SetCommandText(updateQuery.Compile())
                    .AddParameters(updateQuery.getUniqueParameters())
                    .ExecuteNonQuery();

                Asserts.check(affected == 1, $"Failed updating of type {typeof(T).FullName}. More than one rows affected: {affected}.");
            });
        }

        public IEnumerable<T> get(Expression<Func<T, bool>> filterExpression)
        {
            IEnumerable<T> items;
            IDBQuerySelect selectQuery;
            IQueryFilter filter;

            selectQuery = this.queryFactory.newSelect();
            selectQuery.Table = this.entityDescription.Name;
            selectQuery.column("*");
            filter = this.expressionParser.parse<T>(filterExpression);
            selectQuery.setFilter(filter);
            items = this.db.Get<T>(selectQuery, prv_mapItems);

            return items;
        }

        private T prv_mapItems(IDataRecord record)
        {
            T item;

            item = new T();
            foreach (var field in this.entityDescription.getDescriptions())
                prv_mapField(record, field, item);

            return item;
        }

        private static void prv_mapField(IDataRecord record, IPropertyDescription field, T item)
        {
            int fieldIndex;
            object value;

            fieldIndex = record.GetOrdinal(field.Name);
            Asserts.check(fieldIndex >= 0, $"Returned field index below zero '{fieldIndex}' from '{record.GetType().FullName}'.");
            value = record[fieldIndex];
            field.Property.SetValue(item, value);
        }

        private void prv_assertNoAutoIdKeysHaveValue(IEnumerable<IPropertyValue> itemValues)
        {
            prv_assertKeysHaveValue(itemValues.Where(v => !v.IsAutonumeric));
        }

        private void prv_assertKeysHaveValue(IEnumerable<IPropertyValue> keys)
        {
            foreach (var key in keys.Where(k => k.IsKey))
                Asserts.check(key.IsNullOrDefault == false, $"Key '{key.Name}' for type {this.entityDescription.Entity.FullName} cannot be empty.");
        }

        private void prv_assertAutoIdKeyAreEmpty(IEnumerable<IPropertyValue> itemValues)
        {
            foreach (var key in itemValues.Where(k => k.IsKey && k.IsAutonumeric))
                Asserts.check(key.IsNullOrDefault , $"Key '{key.Name}' for type {this.entityDescription.Entity.FullName} must have null or default value.");
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