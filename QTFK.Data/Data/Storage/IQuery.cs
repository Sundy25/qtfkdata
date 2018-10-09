using System.Collections.Generic;

namespace QTFK.Data.Storage
{
    public interface IQuery
    {
        string Instruction { get; set; }
        IDictionary<string, object> Parameters { get; }
    }
}