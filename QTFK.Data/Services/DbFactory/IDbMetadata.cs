using System;
using System.Collections.Generic;

namespace QTFK.Services.DbFactory
{
    public interface IDbMetadata<T> : IClassMetaData where T : IDB
    {
        IEntityMetaData[] Entities { get; }
        IViewMetaData[] Views { get; }
    }
}