using System;
using QTFK.Services;
using QTFK.Models;
using System.Collections.Generic;
using System.Linq;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Services.Repositories;

namespace QTFK.Data.Tests.Models
{
    public class SampleRepository : BaseRepository<SampleClass>, ISampleRepository
    {
        public SampleRepository(IQueryFactory<SampleClass> queryFactory) : base(queryFactory)
        {
        }

        public SampleClass GetByName(string name)
        {
            IQueryFilter filter = _queryFactory.GetFilterForMethodName(nameof(GetByName), name);
            return _queryFactory
                .Select<SampleClass>(q => q.SetFilter(filter))
                .Single()
                ;
        }

        public IEnumerable<SampleClass> GetByWalletCashBetween(decimal min, decimal max)
        {
            IQueryFilter filter = _queryFactory.GetFilterForMethodName(nameof(GetByName), min, max);
            return _queryFactory
                .Select<SampleClass>(q => q.SetFilter(filter))
                ;
        }
    }
}