using System;

namespace QTFK.Services
{
    public interface IDB
    {
        IEngineFeatures EngineFeatures { get; }
        void transact(Func<bool> transactionBlock);
    }
}
