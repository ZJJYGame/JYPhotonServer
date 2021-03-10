using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Cosmos;
using Protocol;
using AscensionProtocol.DTO;
namespace AscensionServer
{
    [Module]
    public class RoleStatusManager:Cosmos.Module, IRoleStatusManager
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncPractice, ProcessHandlerC2S);
        }
        void ProcessHandlerC2S(int seeionid, OperationData packet)
        {
            switch (packet.SubOperationCode)
            {
                default:
                    break;
            }
        }
    }
}
