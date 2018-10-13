using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using QTFK.Data.Factory.Metadata;
using QTFK.Extensions.Assemblies;
using QTFK.Services.Compilers;

namespace QTFK.Data.Factory
{

    public class InMemoryDbBuilder : IDbBuilder
    {
        private static string prv_createClassBody<TDB>(IDbMetadata<TDB> dbMetadata) where TDB : class, IDB
        {
            string body, views;

            views = prv_createViewProperties(dbMetadata);

            body = $@"

namespace {dbMetadata.Namespace}
{{
    using System;

    public class {dbMetadata.Name} : {typeof(TDB).FullName}
    {{
        public {dbMetadata.Name}()
        {{
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

        private void prv_buildDb<TDB>(IDbMetadata<TDB> dbMetadata, ICollection<string> sources) where TDB : class, IDB
        {
            string dbSource;

            foreach (IViewMetaData viewMetadata in dbMetadata.Views)
                prv_buildView(dbMetadata, viewMetadata, sources);



            throw new NotImplementedException();
        }

        private void prv_buildView<TDB>(IDbMetadata<TDB> dbMetadata, IViewMetaData viewMetadata, ICollection<string> sources) where TDB : class, IDB
        {
            throw new NotImplementedException();
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
            object[] contructorParameters;
            ICollection<string> sources;

            /* contruir db
             *  DB      --> {BClass} {DbBody} LView LCrud {EClass}
             *  LView   --> {E}
             *          `-> View LView
             *  View    --> Entity {BClass} {ViewBody} {EClass}
             *  LCrud   --> {E}
             *          `-> Crud LCrud
             *  Crud    --> Entity {BClass} {CrudBody} {EClass}
             *  Entity  --> {BClass} {EntityBody} {EClass}
             */


            sources = new LinkedList<string>();
            prv_buildDb(dbMetadata, sources);



            dbClassBody = prv_createClassBody(dbMetadata);

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

            

            try
            {
                newAssembly = CompilerWrapper.buildInMemoryAssembly(dbClassBody, assemblies);
            }
            catch (CompilerException)
            {
                throw;
            }
            contructorParameters = new object[] { };
            instance = newAssembly.createAssignableInstance<TDB>(contructorParameters);

            return instance;
        }

    }
}