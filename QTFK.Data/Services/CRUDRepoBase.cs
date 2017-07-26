using QTFK.Extensions.DBIO.DBQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Extensions.DBIO;
using QTFK.Extensions.Collections.Strings;
using QTFK.Extensions.Objects.DictionaryConverter;
using QTFK.Extensions.DBCommand;
using QTFK.Extensions.DBIO.QueryFactory;

namespace QTFK.Services
{
    [Obsolete("It will be removed and/or recoded in other classes")]
    public abstract class CRUDRepoBase<T> : ICRUDRepo<T> where T : class, new()
    {
        private readonly IQueryFactory _crudFactory;
        private readonly IMetaDataProvider _metadataProvider;

        public CRUDRepoBase(IQueryFactory crudFactory, IMetaDataProvider metadataProvider)
        {
            _crudFactory = crudFactory;
            _metadataProvider = metadataProvider;
        }

        public T Get(T item)
        {
            var t = typeof(T);
            var dt = item.ToDictionary();

            var q = _crudFactory.NewSelect()
                .SetTable(_metadataProvider.GetEntityName(t))
                ;

            foreach (var prop in _metadataProvider.GetPropertyNames(t))
                q.AddColumn(prop);

            var keys = _metadataProvider.GetKeys(t);

            throw new NotImplementedException();
            //q.SetWhere(keys.Stringify(i => $" {i} = @{i}", " AND "));
            foreach (var key in keys)
                q.SetParam($"@{key}", dt[key]);

            T result = _crudFactory.DB
                .Get<T>(q)
                .FirstOrDefault()
                ;

            return result;
        }

        public T Add(T item)
        {
            var t = typeof(T);
            var dt = item.ToDictionary();

            var q = _crudFactory.NewInsert()
                .SetTable(_metadataProvider.GetEntityName(t))
                ;

            foreach (var prop in _metadataProvider.GetPropertyNames(t))
                if (dt.ContainsKey(prop))
                    q.SetColumn(prop, dt[prop], $"@{prop}");

            _crudFactory.DB.Set(q);

            _crudFactory.DB.Set(cmd =>
            {
                cmd.SetCommandText(q.Compile());
                cmd.AddParameters(q.Parameters);
                cmd.ExecuteNonQuery();

                //GetLastID(cmd)
                //TKey id =  _crudFactory.DB.GetLastID(cmd);
                return 0;
            });
            throw new NotImplementedException();
        }

        public bool Remove(T id)
        {
            throw new NotImplementedException();
        }

        public bool Set(T item)
        {
            throw new NotImplementedException();
        }
    }
}
