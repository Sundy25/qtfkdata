using System.Collections.Generic;
using QTFK.Data.Storage;

namespace QTFK.Data.Extensions.Storages
{
    public static class StorageExtensions
    {
        public static IEnumerable<IRecord> read(this IStorage storage, Query query)
        {
            using (IStorageTransaction transaction = storage.beginTransaction())
                return transaction.read(query);
        }

        public static T readSingle<T>(this IStorage storage, Query query) where T : struct
        {
            using (IStorageTransaction transaction = storage.beginTransaction())
                return transaction.readSingle<T>(query);
        }

        public static int write(this IStorage storage, Query query)
        {
            using (IStorageTransaction transaction = storage.beginTransaction())
                return transaction.write(query);
        }

    }
}
