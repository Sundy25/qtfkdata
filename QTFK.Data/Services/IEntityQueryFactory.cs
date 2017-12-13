using QTFK.Models;
using System;

namespace QTFK.Services
{
    public interface IEntityQueryFactory : IQueryFactory
    {
        IQueryFactory QueryFactory { get; set; }
        IEntityDescription EntityDescription { get; set; }
    }
}
