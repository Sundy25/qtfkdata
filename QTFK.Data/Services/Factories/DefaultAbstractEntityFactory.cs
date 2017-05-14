using System;
using QTFK.Services.DBIO;

namespace QTFK.Services.Factories
{
    public class DefaultAbstractEntityFactory : IAbstractEntityFactory
    {
        protected readonly ICRUDQueryFactory _crudFactory;

        public DefaultAbstractEntityFactory(ICRUDQueryFactory crudFactory)
        {
            _crudFactory = crudFactory;
        }

        public T Get<T>()
        {
            throw new NotImplementedException();
        }
    }
}