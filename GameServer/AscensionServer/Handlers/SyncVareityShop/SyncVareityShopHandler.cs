using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;

namespace AscensionServer
{
  public  class SyncVareityShopHandler: Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncVareityShop; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncVareityShopSubHandler>();
        }
    }
}
