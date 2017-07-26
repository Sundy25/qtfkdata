using System;
using System.Reflection;

namespace QTFK.Services
{
    public interface ITypeBuilder : IDependency
    {
        Type BuildForInterface(Type parentInterface, string name, Module module, Type baseClass);
    }
}