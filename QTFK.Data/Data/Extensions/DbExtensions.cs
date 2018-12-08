using System;

namespace QTFK.Data.Extensions
{
    public static class DbExtensions
    {
        public static T create<T>(this ITable<T> table, Action<T> setterDelegate) where T: IEntity
        {
            T entity;

            Asserts.isNotNull(table);
            Asserts.isNotNull(setterDelegate);

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

            Asserts.isNotNull(table);

            entity = table.create(e => true);

            return entity;
        }

    }
}
