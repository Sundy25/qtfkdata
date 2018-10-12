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

        private class PrvTransaction : IStorageTransaction
        {
            private IDbTransaction transaction;

            private void prv_rollback()
            {
                Asserts.isSomething(this.transaction, $"The transaction has been used.");
                this.transaction.Rollback();
                this.transaction = null;
            }

            public PrvTransaction(IDbTransaction transaction)
            {
                this.transaction = transaction;
                this.Disposed = false;
            }

            public void commit()
            {
                Asserts.isSomething(this.transaction, $"The transaction has been used.");
                this.transaction.Commit();
                this.transaction = null;
            }

            public void rollback()
            {
                prv_rollback();
            }

            public void Dispose()
            {
                Asserts.check(this.Disposed == false, $"This transaction has been disposed.");
                if (this.transaction != null)
                    prv_rollback();

                this.Disposed = true;
            }

            public bool Disposed { get; private set; }

            public IEnumerable<IRecord> read(Query query)
            {
                IDbCommand command;

                Asserts.isSomething(query, $"'{nameof(query)}' parameter cannot be null.");

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

                using (IDbCommand command = this.transaction.Connection.CreateCommand())
                {
                    command.Transaction = this.transaction;
                    command.CommandText = query.Instruction;
                    command.addParameters(query.Parameters);
                    value = (T)command.ExecuteScalar();
                }

                return value;
            }

            public int write(Query query)
            {
                int affectedRows;

                Asserts.isSomething(query, $"'{nameof(query)}' parameter cannot be null.");

                affectedRows = 0;

                using (IDbCommand command = this.transaction.Connection.CreateCommand())
                {
                    command.Transaction = this.transaction;
                    command.CommandText = query.Instruction;
                    command.addParameters(query.Parameters);
                    affectedRows = command.ExecuteNonQuery();
                }

                return affectedRows;
            }
        }

        private bool disposed;
        private SqlConnection connection;
        private PrvTransaction transaction;

        public SqlServerStorage(string connectionString)
        {
            Asserts.isFilled(connectionString, "Argument 'connectionString' cannot be empty");
            this.connection = new SqlConnection(connectionString);
            this.transaction = null;
        }

        public IStorageTransaction getTransaction()
        {
            if (this.transaction == null || this.transaction.Disposed)
            {
                SqlTransaction sqlTransaction;

                this.transaction = null;

                if (this.connection.State == ConnectionState.Closed)
                    this.connection.Open();

                sqlTransaction = this.connection.BeginTransaction($"Transaction{DateTime.UtcNow.Ticks}");
                this.transaction = new PrvTransaction(sqlTransaction);
            }

            return this.transaction;
        }

        public void Dispose()
        {
            Asserts.check(this.disposed == false, "Object has been already disposed.");

            if (this.transaction != null && this.transaction.Disposed == false)
                this.transaction.Dispose();


            if (this.connection.State != System.Data.ConnectionState.Closed)
            {
                this.connection.Close();
                this.connection.Dispose();
            }
            this.transaction = null;
            this.connection = null;
            this.disposed = true;
        }

    }
}
