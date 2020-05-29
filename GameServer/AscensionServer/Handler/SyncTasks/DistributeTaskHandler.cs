/*
*Author : Yingduan_yu
*Since 	:2020-05-25
*Description  : 请求角色任务处理
*/
using System;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using System.Collections.Generic;

namespace AscensionServer
{
    public class DistributeTaskHandler : HandlerBase
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.DistributeTask;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters,(byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务相关信息："+roletask+">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.ToObject<RoleTaskProgress>(roletask);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            ResponseData.Clear();
            OpResponse.OperationCode = operationRequest.OperationCode;
            if (exist)
            {
               Singleton<NHManager>.Instance.Update(roletaskobj);
                var roleTaskProgress = Singleton<NHManager>.Instance.CriteriaGet<RoleTaskProgress>(nHCriteriaRoleID);
                AscensionServer._Log.Info(">>>>>>>>>>>>传回去的" + roleTaskProgress + ">>>>>>>>>>>>");
                ResponseData.Add((byte)ObjectParameterCode.Role, roleTaskProgress);
                OpResponse.Parameters = ResponseData;
                OpResponse.ReturnCode = (short)ReturnCode.Success;

            }
            else
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(OpResponse,sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
       

    }
}
