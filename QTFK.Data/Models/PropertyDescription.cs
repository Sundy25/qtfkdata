using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq.Expressions;

namespace QTFK.Models
{
    public class PropertyDescription
    {
        public string Name { get; }
        public bool IsAutonumeric { get; }
        public bool IsKey { get; }
    }
}