using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QTFK.Data.Tests
{
    [TestClass]
    public class IQueryTest
    {
        [TestMethod]
        public void query_extension_returns_expected_parameters_dictionary()
        {
            IQuery query = new PrvQuery
            {
                Statement = "SELECT * FROM test WHERE test.name = @Name AND test.age < @Age",
                Name = "pepe",
                Age = 34,
            };

            IDictionary<string, object> parameters = query.getParameters();
            Assert.AreEqual(2, parameters.Count);
            Assert.AreEqual("pepe", parameters["Name"]);
            Assert.AreEqual(34, parameters["Age"]);
        }
    }
}
