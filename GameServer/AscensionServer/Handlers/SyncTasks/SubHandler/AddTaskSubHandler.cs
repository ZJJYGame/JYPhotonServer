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
    public class AddTaskSubHandler : SyncTaskSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.ToObject<RoleTaskProgress>(roletask);
            var rolejson = Utility.ToJson(roletaskobj.RoleTaskDict);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            Owner.ResponseData.Clear();
            Owner.OpResponse.OperationCode = operationRequest.OperationCode;
            Owner.ResponseData.Add((byte)OperationCode.SubOperationCode, (byte)SubOpCode);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            if (exist)
            {
                Singleton<NHManager>.Instance.Add(new RoleTaskProgress() { RoleID = roletaskobj.RoleID, RoleTaskInfo = Utility.ToJson(roletaskobj.RoleTaskDict)});
                var roleTaskProgress = Singleton<NHManager>.Instance.CriteriaGet<RoleTaskProgress>(nHCriteriaRoleID);
                AscensionServer._Log.Info(">>>>>>>>>>>>传回去的" + roleTaskProgress.RoleTaskInfo + ">>>>>>>>>>>>");
                Owner.ResponseData.Add((byte)ObjectParameterCode.Role, roleTaskProgress.RoleTaskInfo);
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
