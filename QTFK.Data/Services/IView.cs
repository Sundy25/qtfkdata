﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTFK.Services
{
    public interface IView<T> : IEnumerable<T>
    {
        IPageView<T> paginate(int pageSize, int page);
        int Count { get; }
    }
}
