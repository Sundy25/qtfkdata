using System;
using System.Reflection;
using QTFK.Extensions.Assemblies;

namespace QTFK.Services.DbFactory
{

    public class InMemoryDbBuilder : IDbBuilder
    {
        private static string prv_createClassBody<TDB>(IDbMetadata<TDB> dbMetadata) where TDB: class, IDB
        {
            string body;

            body = $@"

namespace {dbMetadata.Namespace}
{{
    using System;

    public class {dbMetadata.Name} : {typeof(TDB).FullName}
    {{
        public void transact(Func<bool> transactionBlock)
        {{
            transactionBlock();
        }}

    }}
}}
";

            return body;
        }

        private readonly ICompilerWrapper compiler;

        public InMemoryDbBuilder(ICompilerWrapper compiler)
        {
            this.compiler = compiler;

        }

        public TDB createDb<TDB>(IDbMetadata<TDB> dbMetadata) where TDB : class, IDB
        {
            TDB instance;
            Type dbType;
            string dbClassBody;
            string[] assemblies;
            Assembly newAssembly;

            dbType = typeof(TDB);

            assemblies = new string[]
            {
                "Microsoft.CSharp.dll",
                "System.dll",
                "System.Core.dll",
                "System.Data.DataSetExtensions.dll",
                "System.Data.dll",
                "System.Net.Http.dll",
                "System.Xml.dll",
                "System.Xml.Linq.dll",
                "QTFK.Data.dll",
                dbType.Module.Name,
            };

            dbClassBody = prv_createClassBody(dbMetadata);

            this.compiler.CompilationResult += prv_compilerCompilationResult;
            newAssembly = this.compiler.build(dbClassBody, assemblies, settings =>
            {
                settings.GenerateInMemory = true;
                settings.GenerateExecutable = false;
                settings.IncludeDebugInformation = false;
            });
            instance = newAssembly.createAssignableInstance<TDB>();
            this.compiler.CompilationResult -= prv_compilerCompilationResult;

            return instance;
        }

        private static void prv_compilerCompilationResult(System.CodeDom.Compiler.CompilerResults obj)
        {
            if(obj.Errors.HasErrors)
            {
                for (int i = 0, n = obj.Errors.Count; i < n; i++)
                {
                }
            }
        }
    }
}