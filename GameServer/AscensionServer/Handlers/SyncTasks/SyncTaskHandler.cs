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
    public class SyncTaskHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncTask;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncTaskSubHandler>();
        }

        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            //2020-06-02 11:28:54,659 [15] INFO  AscensionServer.AscensionServer - >>>>>>>>>>>>>鎺ュ彈鍒扮殑浠诲姟鐩稿叧淇℃伅锛歿"RoleID":61,"RoleTaskDict":{"0001":{"TaskState":"1","TaskResult":"0002","TaskType":"0"}}}>>>>>>>>>>>>>>>>>>>>>>


            var roletaskobj = Utility.ToObject<RoleTaskProgress>(roletask);
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务：" + roletaskobj.RoleID + ">>>>>>>>>>>>>>>>>>>>>>");
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            ResponseData.Clear();
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务strString：" + roletaskobj.RoleTaskDict + ">>>>>>>>>>>>>>>>>>>>>>");
            OpResponse.OperationCode = operationRequest.OperationCode;
            if (exist)
            {

                Singleton<NHManager>.Instance.Add( new RoleTaskProgress() { RoleTaskDict = roletaskobj.RoleTaskDict });
                var roleTaskProgress = Singleton<NHManager>.Instance.CriteriaGet<RoleTaskProgress>(nHCriteriaRoleID);
                AscensionServer._Log.Info(">>>>>>>>>>>>传回去的" + roleTaskProgress + ">>>>>>>>>>>>");
                ResponseData.Add((byte)ObjectParameterCode.Role, roleTaskProgress);
                OpResponse.Parameters = ResponseData;
                OpResponse.ReturnCode = (short)ReturnCode.Success;

            }
            else
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
    }
}
