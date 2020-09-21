using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public interface INetworkMessageHelper
    {
        object EncodeMessage(object message);
    }
}
