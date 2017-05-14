using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Services.Factories;
using QTFK.Services.DBIO;
using System.Collections.Generic;

namespace QTFK.Data.Tests
{
    public class SampleClass
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal WalletCash { get; set; }
    }

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
