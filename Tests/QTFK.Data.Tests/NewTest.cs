using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Models;
using QTFK.Models;
using System.Linq;
using QTFK.Services.Factories;
using QTFK.Services.DBIO;
using QTFK.Services;
using QTFK.Services.FilterParsers;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class NewTest
    {
        ISampleRepository DependencyInjectionFake_Build()
        {
            var db = new OleDBIO("booooooom");
            var lowLevelqueryFactory = new OleDBQueryFactory(db);

            var methodParsers = new IMethodParser[]
            {
                new ByParamEqualsFilterParser(),
                new ByParamBetweenFilterParser(),
            };

            var filterFactories = new IQueryFilterFactory[]
            {
                lowLevelqueryFactory
            };

            var queryFactory = new DefaultQueryFactory<SampleClass>(
                lowLevelqueryFactory, lowLevelqueryFactory, lowLevelqueryFactory, lowLevelqueryFactory
                , filterFactories
                );

            return new SampleRepository(queryFactory, methodParsers);
        }

        [TestMethod]
        public void TestMethod1()
        {
            ISampleRepository repo = DependencyInjectionFake_Build();

            RepositoryOperationResult result;

            var items = repo.Get();

            Assert.IsFalse(items.Any());

            var item = new SampleClass
            {
                Name = "pepe",
                WalletCash = 666m
            };

            result = repo.Set(item);
            Assert.AreEqual(RepositoryOperationResult.Added, result);
            Assert.IsNotNull(item.ID);

            item.WalletCash = 3.14159265m;
            result = repo.Set(item);
            Assert.AreEqual(RepositoryOperationResult.Updated, result);

            var item2 = repo
                .Get()
                .Where(i => i.ID == item.ID)
                .Single()
                ;

            var item3 = repo
                .GetByName("pepe")
                ;

            Assert.AreNotSame(item, item2);
            Assert.AreEqual(666m, item2.WalletCash);

            Assert.AreNotSame(item, item3);
            Assert.AreEqual(666m, item2.WalletCash);
            Assert.AreSame(item.ID, item3.ID);

            var itemsBetween = repo
                .GetByWalletCashBetween(500m, 1000m)
                .ToArray()
                ;

            Assert.AreEqual(1, itemsBetween.Count());
        }
    }
}
