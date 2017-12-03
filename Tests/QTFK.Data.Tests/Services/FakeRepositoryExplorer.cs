using System;
using System.Collections.Generic;
using QTFK.Services;
using QTFK.Data.Tests.Models;
using SampleLibrary.Services;

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