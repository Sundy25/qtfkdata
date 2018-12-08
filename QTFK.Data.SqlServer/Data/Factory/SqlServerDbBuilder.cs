using System;
using QTFK.Data.Factory.Metadata;
using QTFK.Data.Storage;
using QTFK.Models;
using QTFK.Services;
using QTFK.Services.Loggers;

namespace QTFK.Data.Factory
{
    public class SqlServerDbBuilder : IDbBuilder
    {
        private ILogger<LogLevel> logger;
        private readonly ISqlServerStorage storage;

        public SqlServerDbBuilder(ISqlServerStorage storage, ILogger<LogLevel> logger = null)
        {
            Asserts.isNotNull(storage);

            this.logger = logger ?? NullLogger.Instance;
            this.storage = storage;
        }

        public SqlServerDbBuilder(IStorage storage, ILogger<LogLevel> logger = null)
        {
            Asserts.isNotNull(storage);
            Asserts.isInstanceOf<ISqlServerStorage>(storage);

            this.logger = logger ?? NullLogger.Instance;
            this.storage = this.storage as ISqlServerStorage;
        }

        public TDB createDb<TDB>(IDbMetadata<TDB> dbMetadata) where TDB : class, IDB
        {
            throw new NotImplementedException();
        }
    }
}