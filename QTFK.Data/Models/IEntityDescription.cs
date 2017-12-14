using System;
using System.Collections.Generic;
using System.Reflection;

namespace QTFK.Models
{
    public interface IEntityDescription
    {
        string Name { get; }
        IReadOnlyDictionary<string, PropertyInfo> Fields { get; }
        IReadOnlyDictionary<string, PropertyInfo> Keys { get; }
        Type Entity { get; }
    }
}