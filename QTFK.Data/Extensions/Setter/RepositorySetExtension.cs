using QTFK.Models;
using QTFK.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Extensions.Setter
{
    public static class RepositorySetExtension
    {
        public static RepositoryOperationResult Set<T>(this IRepository<T> repo, T item) where T : new()
        {
            throw new NotImplementedException();
            //return RepositoryOperationResult.Added;
        }
    }
}
