using System;
using QTFK.Services;
using QTFK.Models;

namespace QTFK.Data.Tests.Models
{
    public class SampleRepository : IRepository<SampleClass> //BaseRepository<SampleClass> 
    {
        public IQueryFactory<SampleClass> QueryFactory => throw new NotImplementedException();

        public MetaDataInfo Entity => throw new NotImplementedException();
    }
}