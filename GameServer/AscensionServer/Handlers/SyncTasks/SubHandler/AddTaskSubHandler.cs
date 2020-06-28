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
    public class AddTaskSubHandler : SyncTaskSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Task));
            AscensionServer._Log.Info(">>>>>>>>>>>>>添加任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.Json.ToObject<RoleTaskProgressDTO>(roletask);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleTaskProgress>(nHCriteriaRoleID);
            Dictionary<int, RoleTaskItemDTO> Dic;
            if (exist)
            {
                var roleTaskInfo = Singleton<NHManager>.Instance.CriteriaSelect<RoleTaskProgress>(nHCriteriaRoleID);
                Dic = Utility.Json.ToObject<Dictionary<int, RoleTaskItemDTO>>(roleTaskInfo.RoleTaskInfoDic);

                if (string.IsNullOrEmpty(roleTaskInfo.RoleTaskInfoDic))
                {
                    foreach (var client_n in roletaskobj.RoleTaskInfoDic)
                    {
                        if (!Dic.ContainsKey(client_n.Key))
                        {
                            Dic.Add(client_n.Key, client_n.Value);
                            Singleton<NHManager>.Instance.Update(new RoleTaskProgress() { RoleID = roletaskobj.RoleID, RoleTaskInfoDic = Utility.Json.ToJson(Dic) });
                        }
                    }
                    #region 弃用
                    /*
             // var ServerDic = Utility.Json.ToObject<Dictionary<int, RoleTaskItemDTO>>(roleTaskInfo.RoleTaskInfoDic);
                  * 
                 foreach (var server_n in ServerDic)
                 {
                     if (roletaskobj.RoleTaskInfoDic.ContainsKey(server_n.Key))
                         continue;
                     Dic.Add(server_n.Key,server_n.Value);
                 }

                 foreach (var client_n in roletaskobj.RoleTaskInfoDic)
                 {
                     if (!ServerDic.ContainsKey(client_n.Key))
                     {
                         Dic.Add(client_n.Key, client_n.Value);
                         Singleton<NHManager>.Instance.Update(new RoleTaskProgress() { RoleID = roletaskobj.RoleID, RoleTaskInfoDic = Utility.Json.ToJson(Dic) });
                     }
                 }*/

                    #endregion
                    Owner.OpResponse.Parameters = Owner.ResponseData;
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                }
                else
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            /*
            else 
            {
                Singleton<NHManager>.Instance.Insert(new RoleTaskProgress() { RoleID = roletaskobj.RoleID, RoleTaskInfoDic = Utility.Json.ToJson(roletaskobj.RoleTaskInfoDic) });
                Owner.OpResponse.Parameters = Owner.ResponseData;
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }*/
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
    }
}
