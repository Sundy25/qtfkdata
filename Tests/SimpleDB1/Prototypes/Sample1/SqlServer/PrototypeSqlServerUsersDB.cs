using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Data;
using QTFK.Data.Storage;
using SimpleDB1.DataBases.Sample1;

namespace SimpleDB1.Prototypes.Sample1.SqlServer
{
    public class PrototypeSqlServerUsersDB : IUsersDB
    {
        private class PrvUser : IUser
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime BirthDate { get; set; }
            public bool IsEnabled { get; set; }
        }

        private class PrvUsers : QTFK.Data.Abstracts.AbstractSqlServerTable<IUser, ISqlServerStorage>
        {
            public PrvUsers(ISqlServerStorage storage) : base(storage)
            {
            }

            protected override Query prv_getDeleteAllQuery()
            {
                return "DELETE FROM [user]";
            }

            protected override Query prv_getDeleteQuery(IUser item)
            {
                Query query;

                query = $@"
DELETE FROM [user]
WHERE [user].[id] = @id
";
                query.Parameters.Add("@id", item.Id);

                return query;
            }

            protected override Query prv_getInsertQuery(IUser item)
            {
                Query query;

                query = $@"
INSERT INTO [user] ([name], [birthDate], [isEnabled])
VALUES (@name, @birthDate, @isEnabled)
";
                query.Parameters.Add("@name", item.Name);
                query.Parameters.Add("@birthDate", item.BirthDate);
                query.Parameters.Add("@isEnabled", item.IsEnabled);

                return query;
            }

            protected override IUser prv_getNewEntity()
            {
                return new PrvUser();
            }

            protected override Query prv_getPageSelectQuery(int offset, int pageSize)
            {
                Query outerQuery;
                string innerQuery;

                innerQuery = $@"
SELECT  [id], [name], [birthDate], [isEnabled] 
        , ROW_NUMBER() OVER ( ORDER BY [name] ASC ) AS [__row]
FROM [user]
";

                outerQuery = $@"
SELECT *
FROM ({innerQuery}) as __s
WHERE @lowerRow <= [__row] AND [__row] < @upperRow 
";

                outerQuery.Parameters.Add("@lowerRow", offset);
                outerQuery.Parameters.Add("@upperRow", offset + pageSize);

                return outerQuery;
            }

            protected override Query prv_getSelectCountQuery()
            {
                return "SELECT COUNT(id) FROM [user]";
            }

            protected override Query prv_getSelectQuery()
            {
                return $@"
SELECT [id], [name], [birthDate], [isEnabled] 
FROM [user]
";
            }

            protected override bool prv_getSelectQueryIfEntityHasAutoKeyColumn(IUser item, out Query selectQuery)
            {
                int newId;
                Query query;

                newId = this.storage.readSingle<int>("SELECT SCOPE_IDENTITY()");
                query = $@"
SELECT [id], [name], [birthDate], [isEnabled] 
FROM [user]
WHERE [user].[id] = @id
";
                query.Parameters.Add("@id", newId);
                selectQuery = query;
                return true;
            }

            protected override Query prv_getUpdateQuery(IUser item)
            {
                Query query;

                query = $@"
UPDATE [user] 
SET 
(
    [name] = @name
    , [birthDate] = @birthDate
    , [isEnabled] = @isEnabled
)
WHERE [user].[id] = @id
";

                query.Parameters.Add("@id", item.Id);
                query.Parameters.Add("@name", item.Name);
                query.Parameters.Add("@birthDate", item.BirthDate);
                query.Parameters.Add("@isEnabled", item.IsEnabled);

                return query;
            }

            protected override IUser prv_mapEntity(IRecord record)
            {
                return new PrvUser
                {
                    Id = record.get<int>("id"),
                    Name = record.get<string>("name"),
                    BirthDate = record.get<DateTime>("birthDate"),
                    IsEnabled = record.get<bool>("isEnabled"),
                };
            }
        }

        private readonly ISqlServerStorage storage;

        public PrototypeSqlServerUsersDB(ISqlServerStorage storage)
        {
            this.storage = storage;
            this.Users = new PrvUsers(this.storage);
        }

        public ITable<IUser> Users { get; }

        public void save()
        {
            this.storage.commit();
        }

    }
}
