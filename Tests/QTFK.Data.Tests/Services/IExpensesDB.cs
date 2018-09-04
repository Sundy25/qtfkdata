using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Data.Tests.Models;
using QTFK.Services;

namespace QTFK.Data.Tests.Services
{
    public interface IExpensesDB : IDB
    {
        IUserRepository Users { get; }
        IRepository<ICurrencyConversion> CurrencyExchanges { get; }
    }
}
