using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq.Expressions;

namespace QTFK.Models
{
    public interface IPropertyDescription
    {
        string Name { get; }
        bool IsAutonumeric { get; }
        bool IsKey { get; }
        PropertyInfo Property { get; }
    }
}