using System;
using QTFK.Services;
using QTFK.Models;
using System.Collections.Generic;
using System.Linq;
using QTFK.Extensions.DBIO.DBQueries;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Services.Repositories;
using System.Reflection;

namespace QTFK.Data.Tests.Models
{
    public class SampleRepository : BaseRepository<SampleClass>, ISampleRepository
    {
        private IQueryFilter _GetByWalletCashBetweenFilter;
        private IQueryFilter _GetByNameFilter;

        public SampleRepository(IQueryFactory<SampleClass> queryFactory) : base(queryFactory)
        {
        }

        public SampleClass GetByName(string name)
        {
            _GetByNameFilter = _GetByNameFilter
                ?? _queryFactory.GetFilter(MethodBase.GetCurrentMethod())
                ?? throw new QueryFilterNotFoundException($"No suitable IQueryFilter found for method '{MethodBase.GetCurrentMethod().Name}'")
                ;


            _GetByNameFilter.SetValues(name);

            return _queryFactory
                .Select<SampleClass>(q => q.SetFilter(_GetByNameFilter))
                .Single()
                ;
        }

        public IEnumerable<SampleClass> GetByWalletCashBetween(decimal min, decimal max)
        {
            _GetByWalletCashBetweenFilter = _GetByWalletCashBetweenFilter 
                ?? _queryFactory.GetFilter(MethodBase.GetCurrentMethod())
                ?? throw new QueryFilterNotFoundException($"No suitable IQueryFilter found for method '{MethodBase.GetCurrentMethod().Name}'")
                ;

            _GetByWalletCashBetweenFilter.SetValues(min, max);

            return _queryFactory
                .Select<SampleClass>(q => q.SetFilter(_GetByWalletCashBetweenFilter))
                ;
        }
    }
}