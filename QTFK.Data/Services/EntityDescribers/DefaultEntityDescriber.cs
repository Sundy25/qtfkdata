using System;
using System.Collections.Generic;
using System.Linq;
using QTFK.Models;
using System.Reflection;
using QTFK.Attributes;
using QTFK.Extensions.TypeInfo;
using System.Data;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Models.QueryFilters;
using System.Linq.Expressions;

namespace QTFK.Services.EntityDescribers
{
    public class DefaultEntityDescriber : IEntityDescriber
    {
        const string DEFAULT_ID_FIELD = "id";

        public IEntityDescription describe(Type entityType)
        {
            PrvEntityDescription entityDescription;
            PrvPropertyValue propertyValue;
            IEnumerable<PropertyInfo> fields;
            int numKeys;

            entityDescription = new PrvEntityDescription
            {
                Entity = entityType,
                Name = entityType.getNameOrAlias(),
            };

            fields = entityType
                .GetProperties()
                .Where(p => p.CanRead && p.CanWrite);

            numKeys = 0;
            foreach (var field in fields)
            {
                propertyValue = new PrvPropertyValue
                {
                    Name = field.getNameOrAlias(),
                    IsKey = field.isKey(),
                    IsAutonumeric = field.isAutonumeric(),
                    Property = field,
                };
                if(propertyValue.IsKey)
                    numKeys++;

                entityDescription.Fields.Add(propertyValue.Name, propertyValue);
            }
            if (numKeys == 0)
            {
                PrvPropertyValue id;
                StringComparer comparer;

                comparer = StringComparer.InvariantCultureIgnoreCase;
                id = (PrvPropertyValue)entityDescription
                    .Fields
                    .Values
                    .FirstOrDefault(field => comparer.Equals(field.Name, DEFAULT_ID_FIELD));

                if (id != null)
                {
                    id.IsKey = true;
                    numKeys++;
                }
            }

            Asserts.check(numKeys > 0, $"Type '{entityType.FullName}' has no property tagged with '{typeof(KeyAttribute).FullName}' neither a '{DEFAULT_ID_FIELD}' named property.");
            return entityDescription;
        }

        private class PrvPropertyValue : IPropertyValue
        {
            internal PrvPropertyValue() { }

            internal PrvPropertyValue(IPropertyDescription description)
            {
                this.IsAutonumeric = description.IsAutonumeric;
                this.IsKey = description.IsKey;
                this.Name = description.Name;
                this.Property = description.Property;
            }

            public bool IsNullOrDefault { get; set; }
            public object Value { get; set; }
            public string Name { get; set; }
            public bool IsAutonumeric { get; set; }
            public bool IsKey { get; set; }
            public PropertyInfo Property { get; set; }
        }

        private class PrvEntityDescription : IEntityDescription
        {

            internal PrvEntityDescription()
            {
                this.Fields = new Dictionary<string, IPropertyDescription>();
            }

            public string Name { get; internal set; }
            public Type Entity { get; internal set; }
            public bool UsesAutoId
            {
                get
                {
                    return this.Fields.Values.Any(f => f.IsAutonumeric);
                }
            }
            internal IDictionary<string, IPropertyDescription> Fields { get; }

            public IEnumerable<IPropertyValue> getValues(object item)
            {
                Asserts.isSomething(item, $"Parameter '{nameof(item)}' cannot be null.");

                foreach (var field in this.Fields.Values)
                    yield return prv_getField(field, item);
            }

            public IEnumerable<IPropertyDescription> getDescriptions()
            {
                return this.Fields.Values;
            }

            private IPropertyValue prv_getField(IPropertyDescription field, object item)
            {
                PrvPropertyValue propertyValue;
                object defaultValue;
                Type type;

                propertyValue = new PrvPropertyValue(field);
                propertyValue.Value = propertyValue.Property.GetValue(item);

                type = propertyValue.Value.GetType();
                if (type.IsValueType)
                {
                    defaultValue = Activator.CreateInstance(type);
                    propertyValue.IsNullOrDefault = propertyValue.Value.Equals(defaultValue);
                }
                else
                {
                    propertyValue.IsNullOrDefault = propertyValue.Value == null;
                }

                return propertyValue;
            }
        }

    }
}
