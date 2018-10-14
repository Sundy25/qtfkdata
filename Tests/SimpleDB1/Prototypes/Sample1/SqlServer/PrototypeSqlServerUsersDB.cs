using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QTFK.Data;
using QTFK.Data.Abstracts;
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

        private class PrvUsers : AbstractSqlServerTable<IUser, ISqlServerStorage>, IUserTable
        {
            private readonly Query criteriaQuery;

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

DECLARE @id INT

SELECT @id = SCOPE_IDENTITY()

SELECT [id], [name], [birthDate], [isEnabled] 
FROM [user]
WHERE [user].[id] = @id
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

            protected override Query prv_getUpdateQuery(IUser item)
            {
                Query query;

                query = $@"
UPDATE [user] 
SET 
    [name] = @name
    , [birthDate] = @birthDate
    , [isEnabled] = @isEnabled
WHERE [user].[id] = @id
";

                query.Parameters.Add("@id", item.Id);
                query.Parameters.Add("@name", item.Name);
                query.Parameters.Add("@birthDate", item.BirthDate);
                query.Parameters.Add("@isEnabled", item.IsEnabled);

                return query;
            }

            protected override bool prv_needsReloadAfterInsert()
            {
                return true;
            }


            protected override Query prv_getPageSelectQuery(int offset, int pageSize)
            {
                Query outerQuery, innerQuery;

                innerQuery = $@"
SELECT  [id], [name], [birthDate], [isEnabled] 
        , ROW_NUMBER() OVER ( ORDER BY [name] ASC ) AS [__row]
FROM [user] 
" + this.criteriaQuery;

                outerQuery = new Query(innerQuery, inner => $@"
SELECT *
FROM ({inner}) as __s
WHERE @lowerRow <= [__row] AND [__row] < @upperRow 
");

                outerQuery.Parameters.Add("@lowerRow", offset);
                outerQuery.Parameters.Add("@upperRow", offset + pageSize);

                return outerQuery;
            }

            protected override Query prv_getSelectCountQuery()
            {
                return "SELECT COUNT(id) FROM [user]" + this.criteriaQuery;
            }

            protected override Query prv_getSelectQuery()
            {
                return $@"
SELECT [id], [name], [birthDate], [isEnabled] 
FROM [user]
" + this.criteriaQuery;
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

            public PrvUsers(ISqlServerStorage storage) : base(storage)
            {
                this.criteriaQuery = Query.Empty;
            }

            public PrvUsers(ISqlServerStorage storage, Query criteriaQuery) : base(storage)
            {
                this.criteriaQuery = criteriaQuery;
            }

            public IView<IUser> whereNameEquals(string name)
            {
                Query filterQuery;

                filterQuery = " WHERE [name] = @nameWhere ";
                filterQuery.Parameters.Add("@nameWhere", name);

                return new PrvUsers(this.storage, filterQuery);
            }

        }

        private readonly ISqlServerStorage storage;

        public PrototypeSqlServerUsersDB(ISqlServerStorage storage)
        {
            this.storage = storage;
            this.Users = new PrvUsers(this.storage);
        }

        public IUserTable Users { get; }

        public void save()
        {
            this.storage.commit();
        }

    }
}
