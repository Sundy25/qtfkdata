﻿using QTFK.Services;

namespace SimpleDB1.DataBases.Sample1
{
    public interface IReadonlyUsersDB : IDB
    {
        IView<IUser> Users { get; }
    }
}
