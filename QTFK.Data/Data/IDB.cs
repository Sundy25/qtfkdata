using System;

namespace QTFK.Data
{
    public interface IDB
    {
        IEngineFeatures EngineFeatures { get; }
        void transact(Func<bool> transactionBlock);
    }
}
