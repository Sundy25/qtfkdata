using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Services;
using QTFK.Services;
using QTFK.Services.CompilerWrappers;
using QTFK.Services.DbFactory;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class InMemoryTests
    {
        private IExpensesDB db;

        public InMemoryTests()
        {
            IMetadataBuilder metadataBuilder;
            IDbBuilder dbBuilder;
            IDbMetadata<IExpensesDB> dbMetadata;
            ICompilerWrapper compilerWrapper;

            metadataBuilder = new DefaultMetadataBuilder();
            compilerWrapper = new CompilerWrapper();
            dbBuilder = new InMemoryDbBuilder(compilerWrapper);

            dbMetadata = metadataBuilder.scan<IExpensesDB>();
            this.db = dbBuilder.createDb(dbMetadata);
        }
    }
}
