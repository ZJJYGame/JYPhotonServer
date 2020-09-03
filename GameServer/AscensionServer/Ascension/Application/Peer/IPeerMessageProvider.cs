using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
  public  interface IPeerMessageProvider
    {
        byte SendEvent( object userData);
        byte SendMessage(object userData);
        byte SendOperationResponse(object userData);
    }
}
