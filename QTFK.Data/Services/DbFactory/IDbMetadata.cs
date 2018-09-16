using System;

namespace QTFK.Services.DbFactory
{
    public interface IDbMetadata
    {
        Type InterfaceType { get; }
    }
}