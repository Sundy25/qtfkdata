using System;
using System.Collections.Generic;
using QTFK.Services;
using QTFK.Data.Tests.Models;

namespace QTFK.Data.Tests.Services
{
    public class FakeRepositoryExplorer : IRepositoryExplorer
    {
        public IEnumerable<Type> GetInterfaceTypes()
        {
            yield return typeof(ISampleRepository);
        }
    }
}