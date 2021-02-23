using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
namespace AscensionServer
{
    public class SyncRoleAllianceCaveHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncRoleAllianceCave; } }
        public override void OnInitialization()
        {
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRoleAllianceCaveSubHandler>();
        }
    }
}


