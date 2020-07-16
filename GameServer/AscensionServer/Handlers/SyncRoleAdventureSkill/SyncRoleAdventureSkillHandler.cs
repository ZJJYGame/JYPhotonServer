using AscensionProtocol;
using Photon.SocketServer;
using System;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using System.Collections.Generic;

namespace AscensionServer
{
   public class SyncRoleAdventureSkillHandler:Handler
    {

        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRoleAdventureSkill;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            
        }
    }
}
