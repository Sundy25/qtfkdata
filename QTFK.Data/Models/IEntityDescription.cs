using System;
using System.Collections.Generic;
using System.Reflection;

namespace QTFK.Models
{
    public interface IEntityDescription
    {
        string Name { get; }
        IReadOnlyList<string> Fields { get; }
        IReadOnlyList<string> Keys { get; }
        Type Entity { get; }
    }
}