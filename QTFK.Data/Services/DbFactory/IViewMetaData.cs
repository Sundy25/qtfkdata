using System;

namespace QTFK.Services.DbFactory
{
    public interface IViewMetaData : IClassMetaData
    {
        Type InterfaceType { get; }
        string TableOrView { get; }
    }
}