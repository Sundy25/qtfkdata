﻿using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Text;
using QTFK.Extensions.Assemblies;
using QTFK.Services.Compilers;

namespace QTFK.Services.DbFactory
{

    public class InMemoryDbBuilder : IDbBuilder
    {
        private class PrvEngineFeatures : IEngineFeatures
        {
            public PrvEngineFeatures()
            {
                this.SupportsTransactions = false;
                this.SupportsStoredProcedures = false;
            }

            public bool SupportsTransactions { get; }
            public bool SupportsStoredProcedures { get; }
        }

        private static string prv_createClassBody<TDB>(IDbMetadata<TDB> dbMetadata) where TDB : class, IDB
        {
            string body, engineFeaturesTypeFullName, views;

            engineFeaturesTypeFullName = typeof(IEngineFeatures).FullName;

            views = prv_createViewProperties(dbMetadata);

            body = $@"

namespace {dbMetadata.Namespace}
{{
    using System;

    public class {dbMetadata.Name} : {typeof(TDB).FullName}
    {{
        private readonly {engineFeaturesTypeFullName} engineFeatures;

        public {dbMetadata.Name}({engineFeaturesTypeFullName} engineFeatures)
        {{
            this.engineFeatures = engineFeatures;
        }}
        
        public {engineFeaturesTypeFullName} EngineFeatures
        {{ 
            get
            {{
                return this.engineFeatures;
            }}
        }}

        public void transact(Func<bool> transactionBlock)
        {{
            throw new {typeof(NotSupportedException).FullName}(""Transactions are not supported by InMemoryDbBuilder"");
        }}

        {views}
    }}
}}
";
            return body;
        }

        private static string prv_createViewProperties<TDB>(IDbMetadata<TDB> dbMetadata) where TDB : class, IDB
        {
            StringBuilder stringBuilder;

            stringBuilder = new StringBuilder();

            foreach (IViewMetaData viewMetaData in dbMetadata.Views)
            {
                string view, fieldName;

                fieldName = prv_lowerCamelCase(viewMetaData.InterfaceType.Name);

                view = $@"
private {viewMetaData.InterfaceType.FullName} {fieldName};

public {viewMetaData.InterfaceType.FullName} {viewMetaData.Name}
{{
    get
    {{
        if(this.{fieldName} == null)
            this.{fieldName} = new {viewMetaData.Name}();

        return this.{fieldName};
    }}
}}
";
                stringBuilder.AppendLine(view);
            }

            return stringBuilder.ToString();
        }

        private static string prv_lowerCamelCase(string name)
        {
            return name[0].ToString().ToLower() + name.Substring(1);
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
            PrvEngineFeatures engineFeatures;
            object[] contructorParameters;

            dbType = typeof(TDB);

            assemblies = new string[]
            {
                //"Microsoft.CSharp.dll",
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

            try
            {
                newAssembly = CompilerWrapper.buildInMemoryAssembly(dbClassBody, assemblies);
            }
            catch (CompilerException e)
            {
                throw;
            }
            engineFeatures = new PrvEngineFeatures();
            contructorParameters = new object[] { engineFeatures };
            instance = newAssembly.createAssignableInstance<TDB>(contructorParameters);

            return instance;
        }

    }
}