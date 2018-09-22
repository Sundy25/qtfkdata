using System;

namespace QTFK.Services.DbFactory
{
    public class DefaultMetadataBuilder : IMetadataBuilder
    {
        private class PrvDbMetaData<T> : IDbMetadata<T> where T : IDB
        {
            public IEntityMetaData[] Entities { get; set; }
            public string Name { get; set; }
            public string Namespace { get; set; }
        }

        private static string prv_composeNewClassName(Type type)
        {
            string name, newName;

            name = type.Name;

            if (name.Length > 1
                && name.StartsWith("I")
                && name[1] != 'I')
                newName = name.Substring(1);
            else
                newName = name;

            newName += DateTime.UtcNow.Ticks.ToString();

            return newName;
        }

        public IDbMetadata<T> scan<T>() where T : IDB
        {
            PrvDbMetaData<T> dbMetadata;
            Type interfaceType;

            interfaceType = typeof(T);

            dbMetadata = new PrvDbMetaData<T>()
            {
                Name = prv_composeNewClassName(interfaceType),
                Namespace = interfaceType.Namespace,
                Entities = new IEntityMetaData[] { },
            };

            return dbMetadata;
        }

    }

}