using QTFK.Extensions.Collections.Dictionaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;

namespace QTFK.Services.Factories
{
    public class DefaultFactory<TService> : IFactory<TService> where TService : class
    {
        protected readonly IDictionary<Type, Func<TService>> _providers;
        protected Type _default;

        public DefaultFactory()
        {
            _providers = new Dictionary<Type, Func<TService>>();
            _default = null;
        }

        public TService Get(Type type)
        {
            if (_providers.ContainsKey(type))
                return _providers[type]();

            return null;
        }

        public TService Get<T>() 
        {
            return Get(typeof(T));
        }


        public TService GetDefault()
        {
            return Get(_default);
        }

        public IFactory<TService> Register<T>(Func<TService> builder, bool asDefault = false)
        {
            Type t = typeof(T);
            _providers.Set(t, builder);
            if (asDefault)
                _default = t;
            return this;
        }

        public IEnumerable<Type> RegisteredTypes()
        {
            return _providers.Keys;
        }
    }
}
