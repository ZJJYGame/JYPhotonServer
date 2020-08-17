using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using AscensionProtocol.DTO;
namespace AscensionServer
{
    public class UpdateTaskSubHandler : SyncTaskSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Task));
            AscensionServer._Log.Info(">>>>>>>>>>>>>更新任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.Json.ToObject<RoleTaskProgressDTO>(roletask);
            NHCriteria nHCriteriaRoleID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleTaskProgress>(nHCriteriaRoleID);
            if (exist)
            {
                var roleTaskInfo = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleTaskProgress>(nHCriteriaRoleID);
                var serverDict = Utility.Json.ToObject<Dictionary<string, RoleTaskItemDTO>>(roleTaskInfo.RoleTaskInfoDic);
                if (roleTaskInfo.RoleTaskInfoDic != null)
                {
                    foreach (var client_p in roletaskobj.RoleTaskInfoDic)
                    {
                        if (serverDict.ContainsKey(client_p.Key))
                        {
                            var serverDictValue = serverDict[client_p.Key];
                            if (serverDictValue.RoleTaskAcceptState != client_p.Value.RoleTaskAcceptState)
                                serverDictValue.RoleTaskAcceptState = client_p.Value.RoleTaskAcceptState;
                            if (serverDictValue.RoleTaskAbandonState != client_p.Value.RoleTaskAbandonState)
                                serverDictValue.RoleTaskAbandonState = client_p.Value.RoleTaskAbandonState;
                            if (serverDictValue.RoleTaskAchieveState != client_p.Value.RoleTaskAchieveState)
                                serverDictValue.RoleTaskAchieveState = client_p.Value.RoleTaskAchieveState;
                            ConcurrentSingleton<NHManager>.Instance.Update(new RoleTaskProgress() { RoleID = roletaskobj.RoleID, RoleTaskInfoDic = Utility.Json.ToJson(serverDict) });
                        }else
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    }
                    Owner.OpResponse.Parameters = Owner.ResponseData;
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
    }
}
