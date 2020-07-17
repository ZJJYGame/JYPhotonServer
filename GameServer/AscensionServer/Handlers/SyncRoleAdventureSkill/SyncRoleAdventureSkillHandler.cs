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
        HashSet<RoleAdventureSkillDTO> roleSet = new HashSet<RoleAdventureSkillDTO>();
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRoleAdventureSkill;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResponseData.Clear();
            var roleAdventureSkillJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleAdventureSkill));
            peer.PeerCache.RoleAdventureSkill = Utility.Json.ToObject<RoleAdventureSkillDTO>(roleAdventureSkillJson);

            roleSet.Clear();
            var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            int peerSetLength = peerSet.Count;
            for (int i = 0; i < peerSetLength; i++)
            {
                roleSet.Add(peerSet[i].PeerCache.RoleAdventureSkill);
            }
            var roleSetJson = Utility.Json.ToJson(roleSet);

        }
    }
}
