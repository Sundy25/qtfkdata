using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Data.Tests.Models;
using QTFK.Data.Tests.Services;
using QTFK.Services.DbFactory;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class UnitTest2
    {
        IDbMetadata dbMetadata;
        IMetadataBuilder metadataBuilder;

        public UnitTest2()
        {
            this.metadataBuilder = new DefaultMetadataBuilder();
            this.dbMetadata = this.metadataBuilder.scan<IExpensesDB>();
        }

        [TestMethod]
        public void dbMetaDataTest()
        {
            Assert.IsNotNull(this.dbMetadata);
            Assert.AreEqual(typeof(IExpensesDB), this.dbMetadata.InterfaceType);
            Assert.AreEqual("ExpensesDB", this.dbMetadata.Name);
            Assert.AreEqual(7, this.dbMetadata.Entities.Length);
        }

        [TestMethod]
        public void usersMetaDataTest()
        {
            IEntityMetaData entityMetaData;
            IColumnMetaData columnMetaData;

            entityMetaData = this.dbMetadata.Entities.Single(e => e.Name == "User");
            Assert.AreEqual(typeof(IUser), entityMetaData.InterfaceType);
            Assert.AreEqual("user", entityMetaData.Table);

            Assert.AreEqual(4, entityMetaData.Columns.Length);
            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Name");
            Assert.AreEqual("name", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(string), columnMetaData.ColumnType);

            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Mail");
            Assert.AreEqual("mail", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(string), columnMetaData.ColumnType);

            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "SignDate");
            Assert.AreEqual("creationDate", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(DateTime), columnMetaData.ColumnType);

            Assert.AreEqual(1, entityMetaData.Columns.Where(c => c.IsPrimaryKey).Count());
            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Id");
            Assert.AreEqual("id", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(int), columnMetaData.ColumnType);

            Assert.AreEqual(0, entityMetaData.Columns.Where(c => c.IsForeignKey).Count());
            Assert.AreEqual(0, entityMetaData.Columns.Where(c => c.IsAlternativeKey).Count());
        }

        [TestMethod]
        public void currencysMetaDataTest()
        {
            IEntityMetaData entityMetaData;
            IColumnMetaData columnMetaData;

            entityMetaData = this.dbMetadata.Entities.Single(e => e.Name == "Currency");
            Assert.AreEqual(typeof(ICurrency), entityMetaData.InterfaceType);
            Assert.AreEqual("currency", entityMetaData.Table);

            Assert.AreEqual(2, entityMetaData.Columns.Length);
            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Name");
            Assert.AreEqual("name", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(string), columnMetaData.ColumnType);

            Assert.AreEqual(1, entityMetaData.Columns.Where(c => c.IsPrimaryKey).Count());
            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Id");
            Assert.AreEqual("id", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(int), columnMetaData.ColumnType);

            Assert.AreEqual(0, entityMetaData.Columns.Where(c => c.IsForeignKey).Count());
            Assert.AreEqual(0, entityMetaData.Columns.Where(c => c.IsAlternativeKey).Count());
        }

        [TestMethod]
        public void expenseMetaDataTest()
        {
            IEntityMetaData entityMetaData;
            IColumnMetaData columnMetaData;

            entityMetaData = this.dbMetadata.Entities.Single(e => e.Name == "Expense");
            Assert.AreEqual(typeof(IExpense), entityMetaData.InterfaceType);
            Assert.AreEqual("expense", entityMetaData.Table);

            Assert.AreEqual(3, entityMetaData.Columns.Length);

            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Concept");
            Assert.AreEqual("concept", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(string), columnMetaData.ColumnType);

            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Date");
            Assert.AreEqual("date", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(DateTime), columnMetaData.ColumnType);

            Assert.AreEqual(1, entityMetaData.Columns.Where(c => c.IsPrimaryKey).Count());
            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Id");
            Assert.AreEqual("id", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(int), columnMetaData.ColumnType);

            Assert.AreEqual(0, entityMetaData.Columns.Where(c => c.IsForeignKey).Count());
            Assert.AreEqual(0, entityMetaData.Columns.Where(c => c.IsAlternativeKey).Count());
        }

        [TestMethod]
        public void categoryMetaDataTest()
        {
            IEntityMetaData entityMetaData;
            IColumnMetaData columnMetaData;

            entityMetaData = this.dbMetadata.Entities.Single(e => e.Name == "Category");
            Assert.AreEqual(typeof(ICategory), entityMetaData.InterfaceType);
            Assert.AreEqual("category", entityMetaData.Table);

            Assert.AreEqual(3, entityMetaData.Columns.Length);

            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Name");
            Assert.AreEqual("name", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(string), columnMetaData.ColumnType);

            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Parent");
            Assert.AreEqual("parentCategoryId", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(int), columnMetaData.ColumnType);
            Assert.IsTrue(columnMetaData.IsForeignKey);
            Assert.IsFalse(columnMetaData.IsPrimaryKey);
            Assert.IsTrue(columnMetaData.IsHidden);

            Assert.AreEqual(1, entityMetaData.Columns.Where(c => c.IsPrimaryKey).Count());

            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Id");
            Assert.AreEqual("id", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(int), columnMetaData.ColumnType);
        }

        [TestMethod]
        public void paymentMetaDataTest()
        {
            IEntityMetaData entityMetaData;
            IColumnMetaData columnMetaData;

            entityMetaData = this.dbMetadata.Entities.Single(e => e.Name == "Payment");
            Assert.AreEqual(typeof(IPayment), entityMetaData.InterfaceType);
            Assert.AreEqual("payment", entityMetaData.Table);

            Assert.AreEqual(5, entityMetaData.Columns.Length);

            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Amount");
            Assert.AreEqual("amount", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(decimal), columnMetaData.ColumnType);

            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Expense");
            Assert.AreEqual("expenseId", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(int), columnMetaData.ColumnType);
            Assert.IsTrue(columnMetaData.IsForeignKey);
            Assert.IsFalse(columnMetaData.IsPrimaryKey);
            Assert.IsTrue(columnMetaData.IsHidden);

            Assert.AreEqual(1, entityMetaData.Columns.Where(c => c.IsPrimaryKey).Count());

            columnMetaData = entityMetaData.Columns.Single(c => c.Name == "Id");
            Assert.AreEqual("id", columnMetaData.ColumnName);
            Assert.AreEqual(typeof(int), columnMetaData.ColumnType);
        }

    }
}
