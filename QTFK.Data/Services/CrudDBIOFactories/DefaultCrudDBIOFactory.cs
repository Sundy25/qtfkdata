using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.CrudDBIOFactories
{
    public class DefaultCrudDBIOFactory : ICrudDBIOFactory
    {
        public ICRUDDBIO Get<T>() where T : ICRUDDBIO, IDBIO
        {
            throw new NotImplementedException();
        }

        public ICRUDDBIO GetDefault()
        {
            throw new NotImplementedException();
        }
    }
}
