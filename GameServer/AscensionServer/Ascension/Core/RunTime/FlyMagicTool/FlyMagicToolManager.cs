using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
using AscensionProtocol.DTO;
using AscensionProtocol;
namespace AscensionServer
{
    [Module]
    public class FlyMagicToolManager: Cosmos.Module,IFlyMagicToolManager
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncRoleFlyMagicTool, null);
        }

        void ProcessHandlerC2S(int seeionid, OperationData packet)
        {


        }

    }
}
