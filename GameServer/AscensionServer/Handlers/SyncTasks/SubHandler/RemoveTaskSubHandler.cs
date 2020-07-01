using System;
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
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Task));
            AscensionServer._Log.Info(">>>>>>>>>>>>>删除任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.Json.ToObject<RoleTaskProgressDTO>(roletask);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleTaskProgress>(nHCriteriaRoleID);
            Dictionary<int, RoleTaskItemDTO> Dic;
            if (exist)
            {
                var roleTaskInfo = Singleton<NHManager>.Instance.CriteriaSelect<RoleTaskProgress>(nHCriteriaRoleID);
                Dic = Utility.Json.ToObject<Dictionary<int, RoleTaskItemDTO>>(roleTaskInfo.RoleTaskInfoDic);
                if (roleTaskInfo.RoleTaskInfoDic != null)
                {
                    foreach (var client_p in roletaskobj.RoleTaskInfoDic)
                    {
                        if (Dic.ContainsKey(client_p.Key))// && client_p.Value.RoleTaskAchieveState == "AchieveTask"
                        {
                            Dic.Remove(client_p.Key);
                            Singleton<NHManager>.Instance.Update(new RoleTaskProgress() { RoleID = roletaskobj.RoleID, RoleTaskInfoDic = Utility.Json.ToJson(Dic) });
                            Owner.OpResponse.Parameters = Owner.ResponseData;
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        }
                    }
                }
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
    }
}