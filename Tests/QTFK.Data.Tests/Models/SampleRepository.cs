using QTFK.Services;
using QTFK.Models;
using System.Collections.Generic;
using System.Linq;
using QTFK.Extensions.DBIO.QueryFactory;
using QTFK.Services.Repositories;
using System.Reflection;
using SampleLibrary.Models;
using SampleLibrary.Services;
using QTFK.Extensions.DBIO.DBQueries;

namespace QTFK.Data.Tests.Models
{
    public class SampleRepository : BaseRepository<SampleClass>, ISampleRepository
    {
        private IQueryFilter getByWalletCashBetweenFilter;
        private IQueryFilter getByNameFilter;

        public SampleRepository(
            IQueryFactory<SampleClass> queryFactory
            , IEnumerable<IMethodParser> methodParsers
            ) : base(queryFactory, methodParsers)
        {
        }

        public SampleClass GetByName(string name)
        {
            this.getByNameFilter = this.getByNameFilter
                ?? GetFilter(MethodBase.GetCurrentMethod())
                ?? throw new QueryFilterNotFoundException($"No suitable IQueryFilter found for method '{MethodBase.GetCurrentMethod().Name}'")
                ;

            this.getByNameFilter.SetValues(name);

            return this.queryFactory
                .Select(q => q.SetFilter(this.getByNameFilter))
                .Single()
                ;
        }

        public IEnumerable<SampleClass> GetByWalletCashBetween(decimal min, decimal max)
        {
            this.getByWalletCashBetweenFilter = this.getByWalletCashBetweenFilter 
                ?? GetFilter<decimal>(MethodBase.GetCurrentMethod())
                ?? throw new QueryFilterNotFoundException($"No suitable IQueryFilter found for method '{MethodBase.GetCurrentMethod().Name}'")
                ;

            this.getByWalletCashBetweenFilter.SetValues(min, max);

            return this.queryFactory
                .Select(q => q.SetFilter(this.getByWalletCashBetweenFilter))
                ;
        }
    }
}