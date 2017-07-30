using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Models;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions;
using QTFK.Services.Factories;
using QTFK.Services.DBIO;
using QTFK.Services;
using QTFK.Services.DBIO.QueryFilterFactories;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class NewTest
    {
        ISampleRepository DependencyInjectionFake_Build()
        {
            var db = new QTFK.Services.DBIO.OleDBIO("booooooom");
            var lowLevelqueryFactory = new OleDBQueryFactory(db);
            var filters = new IQueryFilterFactory[]
            {
                new OleDBDefaultFilterFactory(),
            };
            var queryFactory = new DefaultQueryFactory<SampleClass>(lowLevelqueryFactory, filters);
            return new SampleRepository(queryFactory);
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

            Assert.AreNotSame(item, item2);
            Assert.AreEqual(666m, item2.WalletCash);

            var itemsBetween = repo
                .GetByWalletCashBetween(500m, 1000m)
                .ToArray()
                ;

            Assert.AreEqual(1, itemsBetween.Count());
        }
    }
}
