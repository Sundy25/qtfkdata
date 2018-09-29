using System;
using System.Collections.Generic;
using System.Linq;

namespace QTFK.Services.DbFactory
{
    public class DefaultMetadataBuilder : IMetadataBuilder
    {
        private class PrvDbMetaData<T> : IDbMetadata<T> where T : IDB
        {
            public PrvDbMetaData()
            {
                this.EntitiesList = new List<IEntityMetaData>();
                this.ViewsList = new List<IViewMetaData>();
            }

            public IList<IEntityMetaData> EntitiesList { get; }
            public IList<IViewMetaData> ViewsList { get; }

            public string Name { get; set; }
            public string Namespace { get; set; }

            public IEntityMetaData[] Entities
            {
                get
                {
                    return this.EntitiesList.ToArray();
                }
            }

            public IViewMetaData[] Views
            {
                get
                {
                    return this.ViewsList.ToArray();
                }
            }
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
            };

            return dbMetadata;
        }

    }

}