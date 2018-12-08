using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Factory;
using QTFK.Data.Factory.Metadata;
using QTFK.Data.Tests.Services;

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

            metadataBuilder = new DefaultMetadataBuilder();
            dbBuilder = new InMemoryDbBuilder();

            dbMetadata = metadataBuilder.scan<IExpensesDB>();
            this.db = dbBuilder.createDb(dbMetadata);
        }
    }
}
