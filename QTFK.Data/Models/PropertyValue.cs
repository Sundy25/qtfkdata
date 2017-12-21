using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq.Expressions;

namespace QTFK.Models
{
    public class PropertyValue : PropertyDescription
    {
        public bool IsNullOrDefault { get; }
        public object Value { get; }
    }
}