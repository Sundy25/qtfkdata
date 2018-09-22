using System;

namespace QTFK.Services.DbFactory
{
    public interface IDbMetadata<T> : IClassMetaData where T : IDB
    {
        IEntityMetaData[] Entities { get; }
    }
}