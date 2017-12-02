using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace QTFK.Services.RepositoryBuilders
{
    public class DefaultRepositoryBuilder : IRepositoryBuilder
    {
        private readonly ICompilerWrapper compilerWrapper;

        public DefaultRepositoryBuilder(
            ICompilerWrapper compilerWrapper
            )
        {
            this.compilerWrapper = compilerWrapper;
            this.compilerWrapper.CompilationResult += prv_checkCompilationResult;
        }

        public Assembly Build(Type interfaceType)
        {
            Asserts.isSomething(interfaceType, $"'{nameof(interfaceType)}' cannot be null.");
            Asserts.check(interfaceType.IsInterface, $"Type '{interfaceType.FullName}' is not an interface.");
            Asserts.check(interfaceType.ContainsGenericParameters == false, $"Type '{interfaceType.FullName}' cannot have generic parameters.");

            Type repositoryType, entityType;
            string code, repositoryNamespace;
            IEnumerable<string> referencedAssemblies;
            Assembly compiledAssembly;

            repositoryType = interfaceType.GetInterface(typeof(IRepository<>).FullName);
            Asserts.isSomething(repositoryType, $"Type '{interfaceType.FullName}' does not inherits from '{typeof(IRepository<>).FullName}'");

            entityType = repositoryType.GenericTypeArguments.First();
            referencedAssemblies = new string[]
            {
                "QTFK.Core.dll",
                "QTFK.Data.dll",
                "QTFK.Extensions.dll",
                "System.dll",
                "System.Data.dll",
                "System.Core.dll",
                "SampleLibrary.dll",
            };

            repositoryNamespace = "QTFK";
            code = prv_getCodeForRepository(repositoryNamespace, entityType, interfaceType);

            compiledAssembly = this.compilerWrapper.build(code, referencedAssemblies, s =>
            {
                s.GenerateInMemory = true;
                s.GenerateExecutable = false;
                s.IncludeDebugInformation = false;
            });

            return compiledAssembly;
        }

        private string prv_getCodeForRepository(string repositoryNamespace, Type entityType, Type interfaceType)
        {
            string code, className, entityClassName;

            className = $"{entityType.Name}Repository{DateTime.UtcNow.Ticks}";
            entityClassName = entityType.FullName;

            code = $@"
using QTFK.Services;
using QTFK.Models;
using System.Collections.Generic;
using System.Linq;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Services.Repositories;
using System.Reflection;

namespace {repositoryNamespace}
{{
    public class {className} : BaseRepository<{entityClassName}>, {interfaceType.FullName}
    {{
        public {className}(
            IQueryFactory<{entityClassName}> queryFactory
            , IEnumerable<IMethodParser> methodParsers
            ) : base(queryFactory, methodParsers)
        {{
        }}
    }} 
}} 
";

            return code;
        }

        private void prv_checkCompilationResult(System.CodeDom.Compiler.CompilerResults results)
        {
            if(results.Errors.HasErrors)
                throw new Exception();
        }

    }
}