using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services.CRUDRepositoryBuilders
{
    public class DefaultCRUDRepositoryBuilder : ICRUDRepositoryBuilder
    {
        private readonly ITypeBuilder _typeBuilder;

        public DefaultCRUDRepositoryBuilder(ITypeBuilder typeBuilder)
        {
            _typeBuilder = typeBuilder;
        }

        public IEnumerable<Type> Build(IEnumerable<Assembly> assemblies)
        {
            var interfaceRepositories = assemblies
                .SelectMany(ass => ass
                    .GetTypes()
                    .Where(t => t.IsInterface && t.GetInterface(typeof(IRepository<>).FullName) != null)
                    )
                ;

            foreach (var irepo in interfaceRepositories)
            {
                //var x1 = irepo.GetGenericArguments();
                //var x2 = irepo.GenericTypeArguments;
                string name = irepo.Name.StartsWith("I") ? irepo.Name.Substring(1) : irepo.Name;
                name = $"{name}_{DateTime.Now.Ticks}";
                yield return _typeBuilder.BuildForInterface(irepo, name, irepo.Module, typeof(CRUDRepoBase<>));

                //yield return MyTypeBuilder.CompileResultType(name, irepo.Module.Name, typeof(CRUDRepoBase<>));
            }

            throw new NotImplementedException();
        }
    }
}
