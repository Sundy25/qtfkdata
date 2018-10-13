using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using QTFK.Extensions.DataReader;
using QTFK.Extensions.DBCommand;

namespace QTFK.Data.Storage
{
    public class SqlServerStorage : ISqlServerStorage
    {
        private class PrvRecord : IRecord
        {
            private readonly IDataReader reader;

            public PrvRecord(IDataReader reader)
            {
                this.reader = reader;
            }

            public int FieldCount
            {
                get
                {
                    return this.reader.FieldCount;
                }
            }

            public T get<T>(string fieldName)
            {
                return this.reader.Get<T>(fieldName);
            }

            public T get<T>(int i)
            {
                return this.reader.Get<T>(i);
            }
        }

        private class PrvReader : IEnumerable<IRecord>
        {
            private readonly IDbCommand command;
            private bool used;

            private IEnumerator<IRecord> prv_getEnumerator()
            {
                Asserts.check(this.used == false, $"Reader has already been used.");

                using (IDataReader reader = this.command.ExecuteReader())
                {
                    IRecord record;

                    this.used = true;
                    record = new PrvRecord(reader);

                    while (reader.Read())
                        yield return record;

                    if (!reader.IsClosed)
                        reader.Close();
                }

                this.command.Dispose();
            }

            public PrvReader(IDbCommand command)
            {
                this.command = command;
                this.used = false;
            }

            public IEnumerator<IRecord> GetEnumerator()
            {
                return prv_getEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return prv_getEnumerator();
            }

        }

        private bool disposed;
        private IDbConnection connection;
        private IDbTransaction transaction;
        private bool commited;

        private void prv_checkTransaction()
        {
            Asserts.check(this.disposed == false, "Object disposed.");
            Asserts.check(this.commited == false, "Transaction already commited.");

            if (this.connection.State == ConnectionState.Closed)
                this.connection.Open();

            if (this.transaction == null)
                this.transaction = this.connection.BeginTransaction();
        }

        private void prv_dispose()
        {
            if (this.transaction != null)
            {
                if (this.commited == false)
                    this.transaction.Rollback();

                this.transaction.Dispose();
                this.transaction = null;
            }

            if (this.connection.State != ConnectionState.Closed)
            {
                this.connection.Close();
                this.connection.Dispose();
            }
            this.connection = null;
            this.disposed = true;
        }

        ~SqlServerStorage()
        {
            prv_dispose();
        }

        public SqlServerStorage(string connectionString)
        {
            Asserts.isFilled(connectionString, "Argument 'connectionString' cannot be empty");
            this.connection = new SqlConnection(connectionString);
            this.transaction = null;
            this.commited = false;
            this.disposed = false;
        }

        public IEnumerable<IRecord> read(Query query)
        {
            IDbCommand command;

            Asserts.isSomething(query, $"'{nameof(query)}' parameter cannot be null.");

            prv_checkTransaction();

            command = this.transaction.Connection.CreateCommand();
            command.Transaction = this.transaction;
            command.CommandText = query.Instruction;
            command.addParameters(query.Parameters);

            return new PrvReader(command);
        }

        public T readSingle<T>(Query query) where T : struct
        {
            T value;

            Asserts.isSomething(query, $"'{nameof(query)}' parameter cannot be null.");

            value = default(T);
            prv_checkTransaction();

            using (IDbCommand command = this.transaction.Connection.CreateCommand())
            {
                object executionResult;

                command.Transaction = this.transaction;
                command.CommandText = query.Instruction;
                command.addParameters(query.Parameters);
                executionResult = command.ExecuteScalar();
                value = (T)executionResult;
            }

            return value;
        }

        public int write(Query query)
        {
            int affectedRows;

            Asserts.isSomething(query, $"'{nameof(query)}' parameter cannot be null.");

            affectedRows = 0;
            prv_checkTransaction();

            using (IDbCommand command = this.transaction.Connection.CreateCommand())
            {
                command.Transaction = this.transaction;
                command.CommandText = query.Instruction;
                command.addParameters(query.Parameters);
                affectedRows = command.ExecuteNonQuery();
            }

            return affectedRows;
        }

        public void commit()
        {
            Asserts.check(this.disposed == false, "Object disposed.");
            Asserts.check(this.commited == false, "Transaction already commited.");
            Asserts.check(this.connection.State == ConnectionState.Open, "Connection is not open.");
            Asserts.isSomething(this.transaction, "Transaction is null!");

            this.transaction.Commit();
            this.transaction.Dispose();
            this.transaction = null;
            this.commited = true;
        }

        public void Dispose()
        {
            Asserts.check(this.disposed == false, "Object has been already disposed.");

            prv_dispose();
        }

    }
}
