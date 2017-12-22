using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq.Expressions;

namespace QTFK.Models
{
    public interface IEntityDescription
    {
        Type Entity { get; }
        bool UsesAutoId { get; }
        string Name { get; }

        IEnumerable<IPropertyValue> getValues(object item);
        IEnumerable<IPropertyDescription> getDescriptions();
    }
}