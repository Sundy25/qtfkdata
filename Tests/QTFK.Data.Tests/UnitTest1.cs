using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Models;
using QTFK.Data.Tests.Services;
using QTFK.Services;
using System.Linq;
using QTFK.Services.DbFactory;
using QTFK.Services.DBIO;
using System.Configuration;
using QTFK.Models;
using QTFK.Services.Loggers;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private IExpensesDB db;

        public UnitTest1()
        {
            IMetadataBuilder metadataBuilder;
            IDbBuilder dbBuilder;
            IDBIO driver;
            IDbMetadata<IExpensesDB> dbMetadata;
            string connectionString;

            connectionString = ConfigurationManager.ConnectionStrings["tests"]?.ConnectionString;
            Assert.IsTrue(string.IsNullOrWhiteSpace(connectionString), $"Invalid 'tests' connection string in app.config");

            metadataBuilder = new DefaultMetadataBuilder();
            driver = new SQLServerDBIO(connectionString);
            dbBuilder = new SqlServerDbBuilder(driver);

            dbMetadata = metadataBuilder.scan<IExpensesDB>();
            this.db = dbBuilder.createDb(dbMetadata);
        }

        [TestMethod]
        public void userTestMethod1()
        {
            IEnumerable<IUser> users;

            foreach (IUser user1 in this.db.Users)
            {
                Console.WriteLine($"{user1.Name} - {user1.Mail}");
            }

            this.db.transact(() =>
            {
                IUser user;

                user = this.db.Users.create(u =>
                {
                    u.Name = "pepe";
                    u.Mail = "pepe@tronco.es";
                    u.SignDate = DateTime.Now;
                });

                user.Mail = "pepe@gmail.com";
                this.db.Users.update(user);

                this.db.Users.delete(user);

                return true;
            });

            users = this.db.Users.whereMailContains("Frank");
            foreach (IUser filteredUser in users)
            {
                filteredUser.Name += "_check";
                this.db.Users.update(filteredUser);
            }

        }

        [TestMethod]
        public void currencyTestMethod1()
        {
            int expenseAmountsCount;
            IPageView<ExpenseAmount> expenseAmountPage;

            this.db.transact(() =>
            {
                ICurrency euroCurrency, dollardCurrency;
                ICurrencyConversion eurUsd;

                this.db.CurrencyExchanges.deleteAll();
                this.db.Currencies.deleteAll();

                euroCurrency = this.db.Currencies.create(euro => euro.Name = "Euro");
                dollardCurrency = this.db.Currencies.create(dollard => dollard.Name = "US Dollard");

                eurUsd = this.db.CurrencyExchanges.create(x =>
                {
                    x.Date = DateTime.Now;
                    x.From = euroCurrency;
                    x.To = dollardCurrency;
                    x.Value = 1.16m;
                });

                return true;
            });

            foreach (ICurrencyConversion exchange in this.db.Currencies
                .First(c => c.Name == "Euro")
                .Exchanges)
            {
                Console.WriteLine($"1 {exchange.From.Name} = {exchange.Value} {exchange.To.Name}s at {exchange.Date}");
            }

            foreach (ICurrencyConversion exchange in this.db.CurrencyExchanges)
            {
                exchange.Value += 0.00005m;
            }

            foreach (IExpense expense in this.db.Expenses)
            {
                Console.WriteLine($"{expense.Concept} - {expense.Date}");
            }

            expenseAmountsCount = this.db.ExpenseAmounts.Count;
            expenseAmountPage = this.db.ExpenseAmounts.paginate(10, 0);

            Console.WriteLine($"Current Page: {expenseAmountPage.CurrentPage}");
            Console.WriteLine($"Total Pages: {expenseAmountPage.PagesCount}");
            Console.WriteLine($"Page size: {expenseAmountPage.PageSize}");
            foreach (ExpenseAmount amount in expenseAmountPage)
            {
                Console.WriteLine($"{amount.Concept} - {amount.Amount} - {amount.TotalContributors}");
            }

            foreach (ExpenseAmount amount in this.db.ExpenseAmounts.paginate(10, 1))
            {
                Console.WriteLine($"{amount.Concept} - {amount.Amount} - {amount.TotalContributors}");
            }

        }
    }
}
