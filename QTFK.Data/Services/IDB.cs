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
        void transact(Func<bool> transactionBlock);
    }
}
