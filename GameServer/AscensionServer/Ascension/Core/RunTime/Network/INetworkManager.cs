using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public interface INetworkManager:IModuleManager
    {
        object EncodeMessage(object message);
    }
}


