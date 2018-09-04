using QTFK.Attributes;
using QTFK.Models;

namespace QTFK.Data.Tests.Models
{
    public interface ICurrency : IEntity
    {
        [Id]
        int Id { get; set; }
        string Name { get; set; }
    }
}