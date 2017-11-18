using System;
using System.Reflection;

namespace QTFK.Services.RepositoryBuilders
{
    public class DefaultRepositoryBuilder : IRepositoryBuilder
    {
        public DefaultRepositoryBuilder()
        {

        }

        public Assembly Build(Type interfaceType)
        {
            Asserts.IsInstance(interfaceType, $"'{nameof(interfaceType)}' cannot be null.");
            Asserts.IsTrue(interfaceType.IsInterface, $"Type '{interfaceType.FullName}' is not an interface.");
            Asserts.IsTrue(interfaceType.ContainsGenericParameters, $"Type '{interfaceType.FullName}' has no generic parameter.");
            Asserts.IsTrue(interfaceType.GetGenericArguments().Length == 1, $"Type '{interfaceType.FullName}' has more than one generic parameters.");

            Type parameterType;


            throw new NotImplementedException();
        }
    }
}