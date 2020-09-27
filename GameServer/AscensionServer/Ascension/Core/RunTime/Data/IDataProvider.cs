using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionServer
{
    public interface IDataProvider
    {
        object LoadData();
        object ParseData();
    }
}
