using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QTFK.Services;
using SampleLibrary.Models;
using QTFK.Services.Repositories;
using System.Collections.Generic;
using QTFK.Models;
using QTFK.Services.DBIO;

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

            db = new SQLServerDBIO("BOOOOM");
            queryFactory = SQLServerQueryFactory.buildDefault();
            employees = new Repository<Employee>();

            try
            {
                items = employees.Get();
                Assert.Fail();
            }
            catch
            {
            }

            employees.setDB(db, queryFactory);

            try
            {
                items = employees.Get();
                Assert.Fail();
            }
            catch(Exception e)
            {
            }

        }
    }
}
