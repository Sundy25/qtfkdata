using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using SampleLibrary.Models;
using QTFK.Services.Repositories;
using System.Collections.Generic;
using QTFK.Models;
using QTFK.Services.DBIO;
using System.IO;
using QTFK.Services.Loggers;
using QTFK.Services.EntityDescribers;
using QTFK.Services.ExpressionParsers;
using QTFK.Models.QueryFilters;
using System.Linq;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestCategory("Repositories")]
        [TestMethod]
        public void creating_reporitory()
        {
            IRepository<Employee> employees;
            IDBIO db;
            IQueryFactory queryFactory;
            IEnumerable<Employee> items;
            string connectionString;
            ILogger<LogLevel> logger;
            Employee jacintoEmployee;
            IEntityDescriber entityDescriber;
            IExpressionParserFactory expressionParserFactory;
            DateTime age1980, age1990;

            connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={Path.Combine(Environment.CurrentDirectory, "Database1.accdb")};Persist Security Info = False;";
            logger = new DebugLogger<LogLevel>("QTFK Repositories");
            db = new OleDBIO(connectionString, logger);
            queryFactory = new OleDBQueryFactory(new Type[]
            {
                typeof(OleDBOrQueryFilter),
                typeof(OleDBAndQueryFilter),
                typeof(OleDBEqualQueryFilter),
            });
            expressionParserFactory = new FilterExpressionParserFactory();
            entityDescriber = new DefaultEntityDescriber();
            employees = new Repository<Employee>(entityDescriber, expressionParserFactory, db, queryFactory);

            age1980 = new DateTime(1980, 01, 01);
            age1990 = new DateTime(1990, 09, 10);

            items = employees.get(employee => employee.Birth == age1980 || employee.Birth == age1990);
            Assert.AreEqual(2, items.Count());

            items = employees.get(employee => employee.Name == "Rosa" || employee.Name == "Narciso");
            Assert.AreEqual(2, items.Count());

            items = employees.get(employee => employee.Birth == age1980 || employee.Id == 2);
            Assert.AreEqual(1, items.Count());

            items = employees.get(employee => employee.Birth == age1980);
            Assert.AreEqual(1, items.Count());

            items = employees.get(employee => employee.Id == 2);
            Assert.AreEqual(1, items.Count());

            items = employees.get(e => e.Name == "Rosa");
            Assert.AreEqual(1, items.Count());

            //minimumAge = DateTime.Now.AddYears(-18);
            items = employees.get(employee =>
                employee.Birth == age1980
                || employee.LastName == "Céspedes" && employee.Id == 10000
                || employee.Name == "Pepe"
                || employee.Name == "Tronco"
                );
            //items = employees.get(employee =>
            //    employee.Birth > minimumAge
            //    || employee.LastName == "Céspedes" && employee.Id < 10000
            //    || employee.Name == "Pepe"
            //    || employee.Name == "Tronco"
            //    );

            jacintoEmployee = new Employee
            {
                Birth = DateTime.Now.AddYears(-20),
                Name = "Jacinto",
                LastName = "Olmos De la encina",
            };
            employees.add(jacintoEmployee);
            Assert.IsTrue(jacintoEmployee.Id > 0, $"{nameof(jacintoEmployee)} must have a non zero Id after Set call.");

            jacintoEmployee.LastName = "Rosales Robles";
            employees.update(jacintoEmployee);

            employees.delete(jacintoEmployee);

            items = employees.get(employee => employee.Birth > DateTime.Now.AddYears(-18));


        }
    }
}
