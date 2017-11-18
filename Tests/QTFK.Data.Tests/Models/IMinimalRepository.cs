using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Data.Tests.Models
{
    public interface IMinimalRepository : IRepository<SampleClass>
    {
    }
}
