using System;

namespace QTFK.Data.Factory.Metadata
{
    public interface IViewMetaData : IClassMetaData
    {
        Type InterfaceType { get; }
        string TableOrView { get; }
    }
}