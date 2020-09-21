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
    public class GetTaskSubHandler : SyncTaskSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Task));
            Utility.Debug.LogInfo(">>>>>>>>>>>>>获得任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.Json.ToObject<RoleTaskProgressDTO>(roletask);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = NHibernateQuerier.Verify<Role>(nHCriteriaRoleID);
            if (exist)
            {
                var roleTaskInfo = NHibernateQuerier.CriteriaSelect<RoleTaskProgress>(nHCriteriaRoleID);
                if (roleTaskInfo.RoleTaskInfoDic != null)
                    Owner.ResponseData.Add((byte)ParameterCode.Task, roleTaskInfo.RoleTaskInfoDic);
                Owner.OpResponseData.Parameters = Owner.ResponseData;
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
            }
            else
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
        }
    }
}