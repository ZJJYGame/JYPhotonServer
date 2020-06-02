using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
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
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.ToObject<RoleTaskProgress>(roletask);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            Owner.ResponseData.Clear();
            Owner.OpResponse.OperationCode = operationRequest.OperationCode;
            if (exist)
            {
                Singleton<NHManager>.Instance.Update(roletaskobj);
                var roleTaskProgress = Singleton<NHManager>.Instance.CriteriaGet<RoleTaskProgress>(nHCriteriaRoleID);
                AscensionServer._Log.Info(">>>>>>>>>>>>传回去的" + roleTaskProgress + ">>>>>>>>>>>>");
                Owner.ResponseData.Add((byte)ObjectParameterCode.Role, roleTaskProgress);
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
