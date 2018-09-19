using System;

namespace QTFK.Services.DbFactory
{
    public interface IDbMetadata<T> : IMetaData where T : IDB
    {
        IEntityMetaData[] Entities { get; }
    }
}