using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Models;
using QTFK.Data.Tests.Services;
using QTFK.Services;
using System.Linq;

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
            ICurrency euroCurrency, dollardCurrency;
            ICurrencyConversion eurUsd;
            int expenseAmountsCount;
            IPageView<ExpenseAmount> expenseAmountPage;

            driver = getSomeDriver();
            db = buildDB(driver);

            db.CurrencyExchanges.deleteAll();
            db.Currencies.deleteAll();

            euroCurrency = db.Currencies.create();
            euroCurrency.Name = "Euro";
            db.Currencies.insert(ref euroCurrency);
            dollardCurrency = db.Currencies.create();
            dollardCurrency.Name = "US Dollard";
            db.Currencies.insert(ref dollardCurrency);

            eurUsd = db.CurrencyExchanges.create();
            eurUsd.Date = DateTime.Now;
            eurUsd.From = euroCurrency;
            eurUsd.To = dollardCurrency;
            eurUsd.Value = 1.16m;
            db.CurrencyExchanges.insert(ref eurUsd);


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

            expenseAmountsCount = db.ExpenseAmounts.Count;
            expenseAmountPage = db.ExpenseAmounts.paginate(10, 0);

            Console.WriteLine($"Current Page: {expenseAmountPage.CurrentPage}");
            Console.WriteLine($"Total Pages: {expenseAmountPage.PagesCount}");
            Console.WriteLine($"Page size: {expenseAmountPage.PageSize}");
            foreach (ExpenseAmount amount in expenseAmountPage)
            {
                Console.WriteLine($"{amount.Concept} - {amount.Amount} - {amount.TotalContributors}");
            }

            foreach (ExpenseAmount amount in db.ExpenseAmounts.paginate(10, 1))
            {
                Console.WriteLine($"{amount.Concept} - {amount.Amount} - {amount.TotalContributors}");
            }

        }
    }
}
