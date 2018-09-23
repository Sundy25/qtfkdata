using System;
using System.Reflection;
using QTFK.Extensions.Assemblies;
using QTFK.Services.Compilers;

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
        public bool SupportsTransactions 
        {{
            get
            {{
                return false;
            }}
        }}

        public void transact(Func<bool> transactionBlock)
        {{
            throw new System.NotSupportedException(""Transactions are not supported by InMemoryDbBuilder"");
        }}

    }}
}}
";
            return body;
        }

        private static void prv_compilerCompilationResult(System.CodeDom.Compiler.CompilerResults obj)
        {
            if (obj.Errors.HasErrors)
            {
                for (int i = 0, n = obj.Errors.Count; i < n; i++)
                {
                }
            }
        }

        public InMemoryDbBuilder()
        {
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

            newAssembly = CompilerWrapper.buildInMemoryAssembly(dbClassBody, assemblies);
            instance = newAssembly.createAssignableInstance<TDB>();

            return instance;
        }

    }
}