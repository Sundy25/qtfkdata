using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Services;
using QTFK.Services.DbFactory;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class UnitTest2
    {
        [TestMethod]
        public void TestMethod1()
        {
            IDbMetadata dbMetadata;
            IMetadataBuilder metadataBuilder;

            metadataBuilder = new DefaultMetadataBuilder();
            dbMetadata = metadataBuilder.scan<IExpensesDB>();

            Assert.IsNotNull(dbMetadata);
            Assert.AreEqual(typeof(IExpensesDB), dbMetadata.InterfaceType);
        }
    }
}
