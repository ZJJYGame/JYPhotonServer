﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Newtonsoft.Json;
using AscensionProtocol.DTO;
using Cosmos;
namespace AscensionServer
{
    public class RemoveTaskSubHandler : SyncTaskSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Task));
            Utility.Debug.LogInfo(">>>>>>>>>>>>>删除任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.Json.ToObject<RoleTaskProgressDTO>(roletask);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = NHibernateQuerier.Verify<RoleTaskProgress>(nHCriteriaRoleID);
            Dictionary<string, RoleTaskItemDTO> Dic;
            if (exist)
            {
                var roleTaskInfo = NHibernateQuerier.CriteriaSelect<RoleTaskProgress>(nHCriteriaRoleID);
                Dic = Utility.Json.ToObject<Dictionary<string, RoleTaskItemDTO>>(roleTaskInfo.RoleTaskInfoDic);
                if (roleTaskInfo.RoleTaskInfoDic != null)
                {
                    foreach (var client_p in roletaskobj.RoleTaskInfoDic)
                    {
                        if (Dic.ContainsKey(client_p.Key))// && client_p.Value.RoleTaskAchieveState == "AchieveTask"
                        {
                            Dic.Remove(client_p.Key);
                            NHibernateQuerier.Update(new RoleTaskProgress() { RoleID = roletaskobj.RoleID, RoleTaskInfoDic = Utility.Json.ToJson(Dic) });
                            Owner.OpResponseData.Parameters = Owner.ResponseData;
                            Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
                        }
                    }
                }
            }
            else
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
        }
    }
}