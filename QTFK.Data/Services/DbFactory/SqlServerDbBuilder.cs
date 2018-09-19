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
        private readonly IMetadataBuilder metaDataBuilder;

        public SqlServerDbBuilder(IMetadataBuilder metadataBuilder, ISqlServerDBIO dbio, ILogger<LogLevel> logger = null)
        {
            Asserts.isSomething(metadataBuilder, $"Contructor parameter '{nameof(metadataBuilder)}' cannot be null");
            Asserts.isSomething(dbio, $"Contructor parameter '{nameof(dbio)}' cannot be null");

            this.logger = logger ?? NullLogger.Instance;
            this.dbio = dbio;
            this.metaDataBuilder = metadataBuilder;
        }

        public SqlServerDbBuilder(IMetadataBuilder metadataBuilder, IDBIO dbio, ILogger<LogLevel> logger = null)
        {
            Asserts.isSomething(metadataBuilder, $"Contructor parameter '{nameof(metadataBuilder)}' cannot be null");
            Asserts.isSomething(dbio, $"Contructor parameter '{nameof(dbio)}' cannot be null");
            Asserts.check(dbio is ISqlServerDBIO, $"Contructor parameter '{nameof(dbio)}' must implement {typeof(ISqlServerDBIO).FullName}.");

            this.logger = logger ?? NullLogger.Instance;
            this.dbio = dbio as ISqlServerDBIO;
            this.metaDataBuilder = metadataBuilder;
        }

        public TDB createDb<TDB>() where TDB : IDB
        {
            IDbMetadata dbMetadata;

            dbMetadata = this.metaDataBuilder.scan<TDB>();
            throw new NotImplementedException();
        }
    }
}