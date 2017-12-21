using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq.Expressions;

namespace QTFK.Models
{
    public interface IPropertyValue : IPropertyDescription
    {
        bool IsNullOrDefault { get; }
        object Value { get; }
    }
}