using QTFK.Models;
using System;

namespace QTFK.Services
{
    public interface IEntityQueryFactory : IQueryFactory
    {
        IDBIO DB { get; set; }
        IQueryFactory QueryFactory { get; set; }
        Type Entity { get; set; }
        EntityDescription EntityDescription { get; }
    }
}
