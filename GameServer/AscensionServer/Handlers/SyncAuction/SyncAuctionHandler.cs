using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;

namespace AscensionServer
{
    public class SyncAuctionHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncAuction;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncAuctionSubHandler>();
        }
    }
}
