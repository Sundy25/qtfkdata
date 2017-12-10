using QTFK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IEntityDescriber
    {
        EntityDescription getDescription(Type entityType);
        //string getName(Type entity);
        //IEnumerable<string> getFields(Type entity);
        //IEnumerable<string> getId(Type entity);
    }
}
