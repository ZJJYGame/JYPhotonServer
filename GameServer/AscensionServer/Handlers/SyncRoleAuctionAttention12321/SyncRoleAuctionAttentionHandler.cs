using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;

namespace AscensionServer
{
    public class SyncRoleAuctionAttentionHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRoleAuctionAttention;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRoleAuctionAttentionSubHandler>();
        }
    }
}
