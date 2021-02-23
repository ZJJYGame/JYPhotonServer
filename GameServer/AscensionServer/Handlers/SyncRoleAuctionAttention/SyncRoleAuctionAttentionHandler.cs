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
        public override byte OpCode { get { return (byte)OperationCode.SyncRoleAuctionAttention; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRoleAuctionAttentionSubHandler>();
        }
    }
}


