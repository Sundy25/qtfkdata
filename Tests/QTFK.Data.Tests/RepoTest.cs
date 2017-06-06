using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Services.DBIO;
using QTFK.Data.Tests.Models;
using QTFK.Services.MetaDataProviders;

namespace QTFK.Data.Tests
{
    class MyRepo : CRUDRepoBase<SampleClass>
    {
        public MyRepo(ICRUDQueryFactory crudFactory, IMetaDataProvider metadataProvider) 
            : base(crudFactory, metadataProvider)
        {
        }

    }

    [TestClass]
    public class RepoTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var crudFactory = new SqlServerCrudFactory(new SQLServerDBIO(""));
            var metadataProvider = new DefaultMetaDataProvider();
            var myRepo = new MyRepo(crudFactory, metadataProvider);

            SampleClass item = myRepo.Get(new SampleClass { ID = 3 });
        }
    }
}
