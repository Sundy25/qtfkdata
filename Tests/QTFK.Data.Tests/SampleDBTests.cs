using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services.DbFactory;
using SimpleDB1;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class SampleDBTests
    {

        private static IUsersDB prv_createDb()
        {
            IUsersDB db;
            IDbBuilder dbBuilder;
            IDbMetadata<IUsersDB> dbMetadata;
            IMetadataBuilder metadataBuilder;

            metadataBuilder = new DefaultMetadataBuilder();
            dbMetadata = metadataBuilder.scan<IUsersDB>();
            dbBuilder = new InMemoryDbBuilder();
            db = dbBuilder.createDb(dbMetadata);

            return db;
        }

        [TestMethod]
        public void TestMethod1()
        {
            IUsersDB db;
            IUser user;
            IUser[] allUsers;
            int deletedUsers;

            db = prv_createDb();
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
