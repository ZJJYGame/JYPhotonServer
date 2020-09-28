using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface IRoleOperationHelper
    {
        void LoginHandler(object sender,object data);
        void LogoffHandler(object sender,object data);
    }
}
