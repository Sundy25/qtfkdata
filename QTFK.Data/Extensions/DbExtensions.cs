using QTFK.Services;
using System;

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


        public static T create<T>(this ITable<T> table) where T : IEntity
        {
            T entity;

            Asserts.isSomething(table, $"Parameter '{table}' cannot be null.");

            entity = table.create(e => true);

            return entity;
        }

    }
}
