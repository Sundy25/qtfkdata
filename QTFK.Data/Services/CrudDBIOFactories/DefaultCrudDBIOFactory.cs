using QTFK.Extensions.Collections.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;

namespace QTFK.Services.CrudDBIOFactories
{
    public class DefaultCrudDBIOFactory : ICrudDBIOFactory
    {
        protected readonly IDictionary<ICRUDDBIO, Func<ICRUDDBIO>> _providers;
        private string _defaultType;

        public DefaultCrudDBIOFactory()
        {
            _providers = new Dictionary<ICRUDDBIO, Func<ICRUDDBIO>>();
        }

        public ICRUDDBIO Get<T>() where T : ICRUDDBIO, IDBIO
        {
            //string assemblyName = typeof(T).Assembly.FullName;
            //string typeName = typeof(T).FullName;

            //var info = _providers
            //    .FirstOrDefault(i => i.AssemblyName == assemblyName
            //        && i.FullTypeName == typeName);

            //if(info != null)
            //{
                
            //}

            throw new NotImplementedException();
        }

        public ICRUDDBIO GetDefault()
        {
            if (string.IsNullOrWhiteSpace(_defaultType))
                return null;

            throw new NotImplementedException();
        }

        public void Register<T>(Func<T> builder, bool isDefault) where T : ICRUDDBIO
        {
            throw new NotImplementedException();
        }
    }
}
