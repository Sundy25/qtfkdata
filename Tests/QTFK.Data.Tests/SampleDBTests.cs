using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDB1.DataBases.Empty;
using SimpleDB1.DataBases.Sample1;
using SimpleDB1.Prototypes.Sample1.SqlServer;
using System.Configuration;
using QTFK.Services.DBIO;
using System.Collections.Generic;
using QTFK.Data.Factory;
using QTFK.Data.Factory.Metadata;
using QTFK.Data.Extensions;
using QTFK.Data.Storage;

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

        private readonly SqlServerStorage driver;

        public SampleDBTests()
        {
            string connectionString;

            connectionString = ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            Assert.IsFalse(string.IsNullOrWhiteSpace(connectionString), $"Invalid 'tests' connection string in app.config");
            this.driver = new SqlServerStorage(connectionString);

        }

        [TestMethod]
        public void TestEmptyDB()
        {
            IEmptyDB db;

            db = prv_createDb<IEmptyDB>();
            Assert.IsInstanceOfType(db, typeof(IEmptyDB));
            Assert.IsFalse(db.EngineFeatures.SupportsTransactions);
            Assert.IsFalse(db.EngineFeatures.SupportsStoredProcedures);

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
            IEnumerable<IUser> enumerableUsers;
            IUser[] allUsers;
            IPageCollection<IUser> pages;

            //db = prv_createDb<IReadonlyUsersDB>();
            db = new PrototypeSqlServerReadonlyUsersDB(this.driver);
            Assert.AreEqual(2, db.Users.Count);

            pages = db.Users.getPages(1);
            Assert.AreEqual(2, pages.Count);

            enumerableUsers = db.Users;
            allUsers = enumerableUsers.ToArray();
            Assert.AreEqual(2, allUsers.Length);
        }

        [TestMethod]
        public void TestWriteUsersDB()
        {
            IUsersDB db;
            IUser user, emptyUser;
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

            emptyUser = db.Users.create();
            Assert.IsTrue(emptyUser.Id != 0);
            Assert.AreNotEqual(emptyUser.Id, user.Id);
            Assert.AreEqual(new DateTime(), emptyUser.BirthDate);
            Assert.AreEqual(null, emptyUser.Name);
            Assert.IsFalse(emptyUser.IsEnabled);

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
