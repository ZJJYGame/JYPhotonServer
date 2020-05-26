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

namespace AscensionServer.Handler
{
    public class DistributeTaskHandler : BaseHandler
    {
        public DistributeTaskHandler()
        {
            opCode = OperationCode.DistributeTask;
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters,(byte)ObjectParameterCode.Role));
            AscensionServer.log.Info(">>>>>>>>>>>>>接受到的任务相关信息："+roletask+">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.ToObject<Model.RoleTaskProgress>(roletask);
            NHCriteria nHCriteriaTask = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaTask);
            OperationResponse operationResponse = new OperationResponse(operationRequest.OperationCode);
            if (exist)
            {
               Singleton<NHManager>.Instance.Update(roletaskobj);
                var roleTaskProgress = Singleton<NHManager>.Instance.CriteriaGet<Model.RoleTaskProgress>(nHCriteriaTask);
                AscensionServer.log.Info(">>>>>>>>>>>>传回去的" + roleTaskProgress + ">>>>>>>>>>>>");
                Dictionary<byte, object> data = new Dictionary<byte, object>();
                data.Add((byte)ObjectParameterCode.Role, roleTaskProgress);
                operationResponse.Parameters = data;
                operationResponse.ReturnCode = (short)ReturnCode.Success;

            }
            else
                operationResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(operationResponse,sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaTask);
        }
       

    }
}
