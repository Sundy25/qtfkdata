using System;

namespace QTFK.Services.DbFactory
{
    public interface IDbMetadata : IMetaData
    {
        Type InterfaceType { get; }
        IEntityMetaData[] Entities { get; }
    }
}