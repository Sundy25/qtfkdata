using QTFK.Attributes;
using QTFK.Services;
using System;

namespace SimpleDB1
{
    public interface IUser : IEntity
    {
        [Id]
        int Id { get; }

        string Name { get; set; }
        DateTime BirthDate { get; set; }
        bool IsEnabled { get; set; }
    }
}