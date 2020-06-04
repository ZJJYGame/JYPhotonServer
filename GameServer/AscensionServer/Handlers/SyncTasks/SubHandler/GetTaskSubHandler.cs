using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Newtonsoft.Json;

namespace AscensionServer
{
    public class GetTaskSubHandler : SyncTaskSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.ToObject<RoleTaskProgress>(roletask);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            if (exist)
            {
                RoleTaskProgress roleTaskInfo = Singleton<NHManager>.Instance.CriteriaGet<RoleTaskProgress>(nHCriteriaRoleID);
                if (roleTaskInfo != null)
                    Owner.ResponseData.Add((byte)ObjectParameterCode.Role, roleTaskInfo.RoleTaskInfo);
                else
                    Owner.ResponseData.Add((byte)ObjectParameterCode.Role, "数据库不存在任务");
                Owner.OpResponse.Parameters = Owner.ResponseData;
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
    }
}
