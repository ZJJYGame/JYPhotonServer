using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionServer
{
    [ConfigData]
    public abstract class Data
    {
        public abstract void SetData(object data);
    }
}
