using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Models;

namespace QTFK.Services
{
    public interface IDB
    {
        bool SupportsTransactions { get; }
        void transact(Func<bool> transactionBlock);
    }
}
