using QTFK.Models;
using QTFK.Services.DBIO;
using QTFK.Services.Loggers;
using System;

namespace QTFK.Services.DbFactory
{
    public class SqlServerDbBuilder : IDbBuilder
    {
        private ILogger<LogLevel> logger;
        private readonly ISqlServerDBIO dbio;

        public SqlServerDbBuilder(ISqlServerDBIO dbio, ILogger<LogLevel> logger = null)
        {
            this.logger = logger ?? NullLogger.Instance;
            this.dbio = dbio;
        }

        public SqlServerDbBuilder(IDBIO dbio, ILogger<LogLevel> logger = null)
        {
            Asserts.check(dbio is ISqlServerDBIO, $"Contructor parameter '{nameof(dbio)}' must implement {typeof(ISqlServerDBIO).FullName}.");
            this.logger = logger ?? NullLogger.Instance;
            this.dbio = dbio as ISqlServerDBIO;
        }

        public TDB createDb<TDB>(DbMetadata dbMetadata) where TDB : IDB
        {
            throw new NotImplementedException();
        }
    }
}