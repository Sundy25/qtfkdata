using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions
{
    public static class DbExtensions
    {
        public static T create<T>(this ITable<T> table, Action<T> setterDelegate) where T: IEntity
        {
            T entity;

            Asserts.isSomething(table, $"Parameter '{table}' cannot be null.");
            Asserts.isSomething(setterDelegate, $"Parameter '{setterDelegate}' cannot be null.");

            entity = table.create(e =>
            {
                setterDelegate(e);
                return true;
            });

            return entity;
        }

    }
}
