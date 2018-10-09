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
            private readonly SqlDataReader reader;

            public PrvRecord(SqlDataReader reader)
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
            private readonly SqlCommand command;
            private bool used;

            private IEnumerator<IRecord> prv_getEnumerator()
            {
                Asserts.check(this.used == false, $"Reader has already been used.");

                using (SqlDataReader reader = this.command.ExecuteReader())
                {
                    IRecord record = new PrvRecord(reader);

                    while (reader.Read())
                        yield return record;

                    if (!reader.IsClosed)
                        reader.Close();
                }
            }

            public PrvReader(SqlCommand command)
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

        private class PrvTransaction : ITransaction
        {
            private IDbTransaction transaction;
            private readonly IStorage storage;
            private bool disposed;

            private void prv_rollback()
            {
                Asserts.isSomething(this.transaction, $"The transaction has been used.");
                this.transaction.Rollback();
                this.transaction = null;
            }

            public PrvTransaction(IDbTransaction transaction, IStorage sqlServerStorage)
            {
                this.transaction = transaction;
                this.storage = sqlServerStorage;
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
                Asserts.check(this.disposed == false, $"This transaction has been disposed.");
                if (this.transaction != null)
                    prv_rollback();

                this.disposed = true;
            }

            public IEnumerable<IRecord> read(Query query)
            {
                return this.storage.read(query);
            }

            public T readSingle<T>(Query query) where T : struct
            {
                return this.storage.readSingle<T>(query);
            }

            public int write(Query query)
            {
                return this.storage.write(query);
            }
        }

        private bool disposed;
        private SqlConnection connection;

        public SqlServerStorage(string connectionString)
        {
            Asserts.isFilled(connectionString, "Argument 'connectionString' cannot be empty");
            this.connection = new SqlConnection(connectionString);
        }

        public ITransaction beginTransaction()
        {
            SqlTransaction transaction;

            if (this.connection.State == ConnectionState.Closed)
                this.connection.Open();

            transaction = this.connection.BeginTransaction($"Transaction{DateTime.UtcNow.Ticks}");

            return new PrvTransaction(transaction, this);
        }

        public void Dispose()
        {
            Asserts.check(this.disposed == false, "Object has been already disposed.");
            if (this.connection.State != System.Data.ConnectionState.Closed)
            {
                this.connection.Close();
                this.connection.Dispose();
            }
            this.connection = null;
            this.disposed = true;
        }

        public IEnumerable<IRecord> read(Query query)
        {
            SqlCommand command;

            Asserts.isSomething(query, $"'{nameof(query)}' parameter cannot be null.");

            if (this.connection.State == System.Data.ConnectionState.Closed)
                this.connection.Open();

            command = this.connection.CreateCommand();
            command.CommandText = query.Instruction;
            command.addParameters(query.Parameters);

            return new PrvReader(command);
        }

        public T readSingle<T>(Query query) where T : struct
        {
            T value;

            Asserts.isSomething(query, $"'{nameof(query)}' parameter cannot be null.");

            value = default(T);

            if (this.connection.State == System.Data.ConnectionState.Closed)
                this.connection.Open();

            using (SqlCommand command = this.connection.CreateCommand())
            {
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

            if (this.connection.State == System.Data.ConnectionState.Closed)
                this.connection.Open();

            using (SqlCommand command = this.connection.CreateCommand())
            {
                command.CommandText = query.Instruction;
                command.addParameters(query.Parameters);
                affectedRows = command.ExecuteNonQuery();
            }

            return affectedRows;
        }
    }
}
