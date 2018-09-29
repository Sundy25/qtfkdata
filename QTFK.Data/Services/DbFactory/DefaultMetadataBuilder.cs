using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services.DbFactory
{
    public class DefaultMetadataBuilder : IMetadataBuilder
    {
        private class PrvDbMetaData<T> : IDbMetadata<T> where T : IDB
        {
            public string Name { get; set; }
            public string Namespace { get; set; }
            public IEntityMetaData[] Entities { get; set; }
            public IViewMetaData[] Views { get; set; }
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
            IList<IEntityMetaData> entities;
            IList<IViewMetaData> views;

            entities = new List<IEntityMetaData>();
            views = new List<IViewMetaData>();
            interfaceType = typeof(T);

            dbMetadata = new PrvDbMetaData<T>()
            {
                Name = prv_composeNewClassName(interfaceType),
                Namespace = interfaceType.Namespace,
                Entities = entities.ToArray(),
                Views = views.ToArray(),
            };

            return dbMetadata;
        }

    }

}