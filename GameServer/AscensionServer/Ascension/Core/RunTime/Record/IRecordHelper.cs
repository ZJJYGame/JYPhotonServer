﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface IRecordHelper
    {
        void RecordRole(int roleId,object data);
    }
}