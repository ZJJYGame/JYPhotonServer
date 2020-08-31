﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos.Log
{
    public interface ILogHelper
    {
        void Info(string msg);
        void Warring(string msg);
        void Error(Exception exception, string msg);
    }
}
