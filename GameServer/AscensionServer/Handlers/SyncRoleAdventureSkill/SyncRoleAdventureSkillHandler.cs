using AscensionProtocol;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
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
            ResponseData.Add((byte)ParameterCode.RoleAdventureSkill, roleSetJson);
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (short)ReturnCode.Success;
            OpResponse.Parameters = ResponseData;
            peer.SendOperationResponse(OpResponse, sendParameters);
            //广播事件
            threadEventParameter.Clear();
            threadEventParameter.Add((byte)ParameterCode.RoleAdventureSkill, roleAdventureSkillJson);
            QueueThreadEvent(peerSet, EventCode.RoleAdventureSkill, threadEventParameter);

        }

        async void MethodAsync()
        {


        }

        Task CDIntervalMethod(int cd)
        {
            return Task.Run(() =>
            {
              
                Thread.Sleep(5000);


            });
        }

        Task BuffeIntervalxMethod(int cd)
        {
            return Task.Run(() =>
            {

                Thread.Sleep(5000);


            });
        }
    }

}
