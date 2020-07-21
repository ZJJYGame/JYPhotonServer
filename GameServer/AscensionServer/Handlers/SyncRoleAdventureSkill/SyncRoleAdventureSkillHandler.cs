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
            var roleAdventureSkillJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleAdventureStartSkill));
            peer.PeerCache.RoleAdventureSkill = Utility.Json.ToObject<RoleAdventureSkillDTO>(roleAdventureSkillJson);
            AscensionServer._Log.Info("历练技能使用成功");
            roleSet.Clear();
            var peerSet = AscensionServer.Instance.AdventureScenePeerCache.GetValuesList();
            int peerSetLength = peerSet.Count;
            for (int i = 0; i < peerSetLength; i++)
            {
                roleSet.Add(peerSet[i].PeerCache.RoleAdventureSkill);
            }
            var roleSetJson = Utility.Json.ToJson(roleSet);
            ResponseData.Add((byte)ParameterCode.RoleAdventureStartSkill, roleSetJson);
            OpResponse.OperationCode = operationRequest.OperationCode;
            OpResponse.ReturnCode = (short)ReturnCode.Success;
            OpResponse.Parameters = ResponseData;
            peer.SendOperationResponse(OpResponse, sendParameters);
            //广播事件
            threadEventParameter.Clear();
            threadEventParameter.Add((byte)ParameterCode.RoleAdventureStartSkill, roleAdventureSkillJson);
            QueueThreadEvent(peerSet, EventCode.RoleAdventureStartSkill, threadEventParameter);

        }

        #region 删除
        //    void funtion(RoleAdventureSkillDTO roleAdventureSkillDTO)
        //    {
        //        switch (roleAdventureSkillDTO.featureSkillTypeEnum)
        //        {
        //            case RoleAdventureSkillDTO.FeatureSkillTypeEnum.Zero:
        //                break;
        //            case RoleAdventureSkillDTO.FeatureSkillTypeEnum.Tp:
        //                break;
        //            case RoleAdventureSkillDTO.FeatureSkillTypeEnum.SpeetMove:
        //                break;
        //            case RoleAdventureSkillDTO.FeatureSkillTypeEnum.Stealth:
        //                break;
        //            case RoleAdventureSkillDTO.FeatureSkillTypeEnum.Visible:
        //                break;
        //            case RoleAdventureSkillDTO.FeatureSkillTypeEnum.Eradicate:
        //                break;
        //            case RoleAdventureSkillDTO.FeatureSkillTypeEnum.FlashBeforeOne:
        //                break;
        //            default:
        //                break;
        //        }
        //    }



        //    Task CDIntervalMethod(int cd)
        //    {
        //        return Task.Run(() =>
        //        {
        //            Thread.Sleep(cd*1000);
        //        });
        //    }

        //    Task BuffeIntervalxMethod(int cd)
        //    {
        //        return Task.Run(() =>
        //        {
        //            Thread.Sleep(5000);
        //        });
        //    }
        #endregion
    }
    }
