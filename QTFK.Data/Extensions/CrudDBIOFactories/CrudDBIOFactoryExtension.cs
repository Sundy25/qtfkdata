using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.CrudDBIOFactories
{
    public static class CrudDBIOFactoryExtension
    {
        public static ICrudDBIOFactory Register<T>(this ICrudDBIOFactory factory, Func<T> builder) where T : ICRUDDBIO
        {
            return factory.Register(builder, false);
        }
    }
}
