using QTFK.Services;
using SampleLibrary.Models;
using System.Collections.Generic;

namespace SampleLibrary.Services
{
    public interface ISampleRepository : IRepository<SampleClass>
    {
        SampleClass GetByName(string name);
        IEnumerable<SampleClass> GetByWalletCashBetween(decimal min, decimal max);
    }
}
