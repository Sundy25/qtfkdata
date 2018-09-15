using QTFK.Models;
using QTFK.Services.DBIO;
using QTFK.Services.Loggers;

namespace QTFK.Services.DbFactory
{
    public class SqlServerDbBuilder : IDbBuilder
    {
        private ILogger<LogLevel> logger;

        public SqlServerDbBuilder(ISqlServerDBIO driver, ILogger<LogLevel> logger = null)
        {
            this.logger = logger ?? NullLogger.Instance;
        }

        public TDB createDb<TDB>(DbMetadata dbMetadata) where TDB : IDB
        {
            throw new System.NotImplementedException();
        }
    }
}