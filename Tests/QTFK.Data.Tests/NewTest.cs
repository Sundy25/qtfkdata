using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Models;
using QTFK.Models;
using System.Linq;
using QTFK.Extensions;
using QTFK.Extensions.Getters;
using QTFK.Extensions.Setter;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class NewTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var repo = new SampleRepository();

            RepositoryOperationResult result;

            var items = repo
                .GetAll()
                ;

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

            //var item2 = repo
            //    .Get()
            //    .Where(i => i.ID)
            //    .IsEqualTo(item.ID)
            //    .Items
            //    .Single()
            //    ;

            var item2 = repo
                .GetByID(item.ID)
                ;

            Assert.AreNotSame(item, item2);
            Assert.AreEqual(666m, item2.WalletCash);

            var itemsBetween = repo
                .GetBetween(i => i.WalletCash, 500m, 1000m)
                .ToArray()
                ;

            Assert.AreEqual(1, itemsBetween.Count());
        }
    }
}
