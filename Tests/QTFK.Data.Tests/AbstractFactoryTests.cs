using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Services.Factories;
using QTFK.Services.DBIO;
using System.Collections.Generic;
using QTFK.Data.Tests.Models;

namespace QTFK.Data.Tests
{
    public interface ISampleClassRepository : ICRUDRepository<int, SampleClass>
    {
        IEnumerable<SampleClass> GetByWalletCash(decimal walletCash);
        IEnumerable<SampleClass> GetWithWalletCashBetween(decimal minWalletCash, decimal maxWalletCash);
    }

    [TestClass]
    public class AbstractFactoryTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            IAbstractEntityFactory superFactory = new DefaultAbstractEntityFactory(
                new SqlServerCrudFactory(
                    new SQLServerDBIO("")
                    ));

            ISampleClassRepository repository = superFactory.Get<ISampleClassRepository>();

            var samples = repository.GetWithWalletCashBetween(-1000m, 100000m);
        }
    }
}
