using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Models;
using QTFK.Data.Tests.Services;
using QTFK.Services;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private static IDBIO getSomeDriver()
        {
            throw new NotImplementedException();
        }

        private static IExpensesDB buildDB(IDBIO driver)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void TestMethod1()
        {
            IDBIO driver;
            IExpensesDB db;
            IUser user;
            IEnumerable<IUser> users;

            driver = getSomeDriver();
            db = buildDB(driver);

            user = db.Users.create();
            user.Name = "pepe";
            user.Mail = "pepe@tronco.es";
            db.Users.insert(ref user);

            user.Mail = "pepe@gmail.com";
            db.Users.update(user);

            db.Users.delete(user);

            users = db.Users.getWhereMailContains("Frank");
            foreach (IUser filteredUser in users)
            {
                filteredUser.Name += "_check";
                db.Users.update(filteredUser);
            }

            foreach (IUser user1 in db.Users)
            {
                Console.WriteLine($"{user1.Name} - {user1.Mail}");
            }

            foreach (ICurrencyConversion exchange in db.CurrencyExchanges)
            {
                exchange.Value += 0.00005m;
            }

            foreach (IExpense expense in db.Expenses)
            {
                Console.WriteLine($"{expense.Concept} - {expense.Date}");
            }
        }

    }
}
