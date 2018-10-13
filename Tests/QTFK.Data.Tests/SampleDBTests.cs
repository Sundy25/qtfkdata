using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleDB1.DataBases.Empty;
using SimpleDB1.DataBases.Sample1;
using SimpleDB1.Prototypes.Sample1.SqlServer;
using System.Configuration;
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
        }

        [TestMethod]
        public void TestReadonlyUserDB()
        {
            IReadonlyUsersDB db;
            IEnumerable<IUser> users;
            IUser[] usersArray;
            IPageCollection<IUser> pages;
            IPageView<IUser> page;

            //db = prv_createDb<IReadonlyUsersDB>();
            db = new PrototypeSqlServerReadonlyUsersDB(this.driver);
            Assert.AreEqual(5, db.Users.Count);

            pages = db.Users.getPages(2);
            Assert.AreEqual(3, pages.Count);

            users = db.Users;
            usersArray = users.ToArray();
            Assert.AreEqual(5, usersArray.Length);

            page = pages[0];
            Assert.AreEqual(2, page.Count);
            usersArray = page.ToArray();
            Assert.AreEqual(2, usersArray.Length);

            page = pages[1];
            Assert.AreEqual(2, page.Count);
            usersArray = page.ToArray();
            Assert.AreEqual(2, usersArray.Length);

            page = pages[2];
            Assert.AreEqual(1, page.Count);
            usersArray = page.ToArray();
            Assert.AreEqual(1, usersArray.Length);

            db.save();
        }

        [TestMethod]
        public void TestWriteUsersDB()
        {
            IUsersDB db;
            IUser user, emptyUser, updatedUser;
            IUser[] allUsers;
            int deletedUsers;

            //db = prv_createDb<IUsersDB>();
            db = new PrototypeSqlServerUsersDB(this.driver);
            Assert.AreEqual(5, db.Users.Count);

            allUsers = db.Users.ToArray();
            Assert.AreEqual(5, allUsers.Length);

            user = db.Users.create(u =>
            {
                u.BirthDate = new DateTime(1955, 11, 12);
                u.IsEnabled = true;
                u.Name = "Pepe1";
            });

            Assert.IsTrue(user.Id != 0);
            Assert.AreEqual(new DateTime(1955, 11, 12), user.BirthDate);
            Assert.AreEqual("Pepe1", user.Name);
            Assert.IsTrue(user.IsEnabled);

            emptyUser = db.Users.create();
            Assert.IsTrue(emptyUser.Id != 0);
            Assert.AreNotEqual(emptyUser.Id, user.Id);
            Assert.AreEqual(new DateTime(), emptyUser.BirthDate);
            Assert.AreEqual(null, emptyUser.Name);
            Assert.IsFalse(emptyUser.IsEnabled);

            allUsers = db.Users.ToArray();
            Assert.AreEqual(7, allUsers.Length);

            user = allUsers.Single(u => u.Name == "Pepe1");
            Assert.IsTrue(user.Id != 0);
            Assert.AreEqual(new DateTime(1955, 11, 12), user.BirthDate);
            Assert.AreEqual("Pepe1", user.Name);
            Assert.IsTrue(user.IsEnabled);


            user.IsEnabled = false;
            user.BirthDate = user.BirthDate.AddYears(10);
            db.Users.update(user);
            updatedUser = db.Users.Single(u => u.Name == "Pepe1");
            Assert.AreEqual(user.Id, updatedUser.Id);
            Assert.AreEqual(updatedUser.BirthDate, new DateTime(1965, 11, 12));
            Assert.AreEqual(updatedUser.IsEnabled, false);
            Assert.AreNotSame(user, updatedUser);

            db.Users.delete(updatedUser);
            allUsers = db.Users.ToArray();
            Assert.AreEqual(6, allUsers.Length);
            user = db.Users.SingleOrDefault(u => u.Name == "Pepe1");
            Assert.IsNull(user);

            deletedUsers = db.Users.deleteAll();
            Assert.AreEqual(6, deletedUsers);

            allUsers = db.Users.ToArray();
            Assert.AreEqual(0, allUsers.Length);
        }

    }
}
