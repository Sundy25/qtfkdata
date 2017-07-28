using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Data.Tests.Models
{
    public interface ISampleRepository : IRepository<SampleClass>
    {
        SampleClass GetByName(string name);
        IEnumerable<SampleClass> GetByWalletCashBetween(decimal min, decimal max);
    }
}
