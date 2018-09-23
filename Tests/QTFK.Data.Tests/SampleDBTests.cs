using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using QTFK.Services.DbFactory;
using SimpleDB1.DataBases.Empty;
using SimpleDB1.DataBases.Sample1;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class SampleDBTests
    {

        private static T prv_createDb<T>() where T: class, IDB
        {
            T db;
            IDbBuilder dbBuilder;
            IDbMetadata<T> dbMetadata;
            IMetadataBuilder metadataBuilder;

            metadataBuilder = new DefaultMetadataBuilder();
            dbMetadata = metadataBuilder.scan<T>();
            dbBuilder = new InMemoryDbBuilder();
            db = dbBuilder.createDb(dbMetadata);

            return db;
        }

        [TestMethod]
        public void TestEmptyDB()
        {
            IEmptyDB db;

            db = prv_createDb<IEmptyDB>();
            Assert.IsInstanceOfType(db, typeof(IEmptyDB));
            Assert.IsFalse(db.SupportsTransactions);

            try
            {
                db.transact(() =>
                {
                    return true;
                });
                Assert.Fail();
            }
            catch (NotSupportedException)
            {
            }

        }

        [TestMethod]
        public void TestReadonlyUserDB()
        {
            IReadonlyUsersDB db;
            IUser[] allUsers;

            db = prv_createDb<IReadonlyUsersDB>();
            Assert.AreEqual(0, db.Users.Count);

            allUsers = db.Users.ToArray();
            Assert.AreEqual(0, allUsers.Length);
        }

        [TestMethod]
        public void TestWriteUsersDB()
        {
            IUsersDB db;
            IUser user;
            IUser[] allUsers;
            int deletedUsers;

            db = prv_createDb<IUsersDB>();
            Assert.AreEqual(0, db.Users.Count);

            allUsers = db.Users.ToArray();
            Assert.AreEqual(0, allUsers.Length);

            user = db.Users.create(u =>
            {
                u.BirthDate = new DateTime(1955, 11, 12);
                u.IsEnabled = true;
                u.Name = "Pepe";
            });

            Assert.IsTrue(user.Id != 0);
            Assert.AreEqual(new DateTime(1955, 11, 12), user.BirthDate);
            Assert.AreEqual("Pepe", user.Name);
            Assert.IsTrue(user.IsEnabled);

            allUsers = db.Users.ToArray();
            Assert.AreEqual(1, allUsers.Length);

            user = allUsers[0];
            Assert.IsTrue(user.Id != 0);
            Assert.AreEqual(new DateTime(1955, 11, 12), user.BirthDate);
            Assert.AreEqual("Pepe", user.Name);
            Assert.IsTrue(user.IsEnabled);

            deletedUsers = db.Users.deleteAll();
            Assert.AreEqual(1, deletedUsers);

            allUsers = db.Users.ToArray();
            Assert.AreEqual(0, allUsers.Length);
        }

    }
}
