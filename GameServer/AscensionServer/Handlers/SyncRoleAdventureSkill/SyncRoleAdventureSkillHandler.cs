using AscensionProtocol;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol.DTO;
using EventData = Photon.SocketServer.EventData;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
namespace AscensionServer
{
   public class SyncRoleAdventureSkillHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncRoleAdventureSkill; } }
        HashSet<RoleAdventureSkillDTO> roleSet = new HashSet<RoleAdventureSkillDTO>();
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            responseParameters.Clear();
            var roleAdventureSkillJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleAdventureStartSkill));
            //peer.PeerCache.RoleAdventureSkill = Utility.Json.ToObject<RoleAdventureSkillDTO>(roleAdventureSkillJson);
            Utility.Debug.LogInfo("历练技能使用成功");
            roleSet.Clear();
            //var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            //int peerSetLength = peerSet.Count;
            //for (int i = 0; i < peerSetLength; i++)
            //{
            //    roleSet.Add(peerSet[i].PeerCache.RoleAdventureSkill);
            //}
            var roleSetJson = Utility.Json.ToJson(roleSet);
            responseParameters.Add((byte)ParameterCode.RoleAdventureStartSkill, roleAdventureSkillJson);
            operationResponse.OperationCode = operationRequest.OperationCode;
            operationResponse.ReturnCode = (short)ReturnCode.Success;
            operationResponse.Parameters = responseParameters;
            //广播事件
            //threadEventParameter.Clear();
            //threadEventParameter.Add((byte)ParameterCode.RoleAdventureStartSkill, roleAdventureSkillJson);
            //QueueThreadEvent(peerSet, EventCode.RoleAdventureStartSkill, threadEventParameter);
            //SkillBuffEnd(peer.PeerCache.RoleAdventureSkill.BuffeInterval, peer);
            //CDIntervalMethod(peer.PeerCache.RoleAdventureSkill.CDInterval, peer, peer.PeerCache.RoleAdventureSkill.SkillID);
            return operationResponse;
        }

        #region 删除
        async void SkillCDEnd(int cd, AscensionPeer peer, int skillid)
        {
            await CDIntervalMethod(cd, peer, skillid);
        }
        async void SkillBuffEnd(int cd, AscensionPeer peer)
        {
            await BuffIntervalMethod(cd, peer);
        }

        Task BuffIntervalMethod(int cd, AscensionPeer peer)
        {
            return Task.Run(() =>
            {
                //AscensionServer._Log.Info("进入技能buff线程》》》》》》》》》》》》》》》》》》》》"+ peer.PeerCache.RoleAdventureSkill.IsInUse);
                Thread.Sleep(cd * 1000);
                //AscensionServer._Log.Info("技能buff线程已经开始》》》》》》》》》》》》》》》》》》》》");
                //peer.PeerCache.RoleAdventureSkill.IsInUse =false ;
                //var syncAdventureSkillEvent = new SyncAdventureSkillEvent();
                //syncAdventureSkillEvent.OnInitialization();
                //ThreadPool.QueueUserWorkItem(syncAdventureSkillEvent.Handler);
            });
        }
        Task CDIntervalMethod(int cd, AscensionPeer peer,int skillid)
        {
            return Task.Run(() =>
            {
                Utility.Debug.LogInfo("技能cd线程已经开始》》》》》》》》》》》》》》》》》》》》");
                Thread.Sleep(cd * 1000);
                Utility.Debug.LogInfo("进入技能buff线程》》》》》》》》》》》》》》》》》》》》");
                var data = new Dictionary<byte, object>();
                data.Add((byte)ParameterCode.RoleAdventureSkillCD, Utility.Json.ToJson(skillid));
                EventData eventData = new EventData();
                eventData.Code= (byte)EventCode.RoleAdventureSkillCD;
                eventData.Parameters = data;
                peer.SendEvent(eventData, new SendParameters());
            });

            }
        #endregion
    }
    }


