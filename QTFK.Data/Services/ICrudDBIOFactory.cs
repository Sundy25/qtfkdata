using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface ICrudDBIOFactory
    {
        ICRUDDBIO GetDefault();
        ICRUDDBIO Get<T>() where T : ICRUDDBIO, IDBIO;
        void Register<T>(Func<T> builder, bool isDefault) where T : ICRUDDBIO;
    }
}
