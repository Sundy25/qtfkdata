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
                throw new NotImplementedException();
            }

            protected override Query prv_getDeleteQuery(IUser item)
            {
                throw new NotImplementedException();
            }

            protected override Query prv_getInsertQuery(IUser entity)
            {
                throw new NotImplementedException();
            }

            protected override IUser prv_getNewEntity()
            {
                throw new NotImplementedException();
            }

            protected override Query prv_getPageSelectQuery(int offset, int pageSize)
            {
                throw new NotImplementedException();
            }

            protected override Query prv_getSelectCountQuery()
            {
                throw new NotImplementedException();
            }

            protected override Query prv_getSelectQuery()
            {
                throw new NotImplementedException();
            }

            protected override bool prv_getSelectQueryIfEntityHasAutoKeyColumn(IUser entity, out Query selectQuery)
            {
                throw new NotImplementedException();
            }

            protected override Query prv_getUpdateQuery(IUser item)
            {
                throw new NotImplementedException();
            }

            protected override IUser prv_mapEntity(IRecord record)
            {
                throw new NotImplementedException();
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
