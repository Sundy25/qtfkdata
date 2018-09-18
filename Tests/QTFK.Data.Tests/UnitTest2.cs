using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Models;
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
            IEntityMetaData entityMetaData;

            metadataBuilder = new DefaultMetadataBuilder();
            dbMetadata = metadataBuilder.scan<IExpensesDB>();

            Assert.IsNotNull(dbMetadata);
            Assert.AreEqual(typeof(IExpensesDB), dbMetadata.InterfaceType);
            Assert.AreEqual(7, dbMetadata.Entities.Length);

            entityMetaData = dbMetadata.Entities.Single(e => e.Name == "User");
            Assert.AreEqual(typeof(IUser), entityMetaData.InterfaceType);
            Assert.AreEqual("user", entityMetaData.Table);

        }
    }
}
