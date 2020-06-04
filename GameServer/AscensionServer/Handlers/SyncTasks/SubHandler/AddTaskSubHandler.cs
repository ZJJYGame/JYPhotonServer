﻿using System;
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
            ResetResponseData(operationRequest);
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            var roletaskobj = Utility.ToObject<RoleTaskProgress>(roletask);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            string strInfoObj = "";
            if (exist)
            {
                RoleTaskProgress roleTaskInfo = Singleton<NHManager>.Instance.CriteriaGet<RoleTaskProgress>(nHCriteriaRoleID);
                if (roleTaskInfo == null)
                    Singleton<NHManager>.Instance.Add(new RoleTaskProgress() { RoleID = roletaskobj.RoleID, RoleTaskInfo = roletaskobj.RoleTaskInfo });
                else
                {
                    strInfoObj = roleTaskInfo.RoleTaskInfo + roletaskobj.RoleTaskInfo;
                    AscensionServer._Log.Info("角色存在2" + strInfoObj);
                    Singleton<NHManager>.Instance.Update(new RoleTaskProgress() { RoleID = roletaskobj.RoleID, RoleTaskInfo = strInfoObj });
                }
                //AscensionServer._Log.Info(">>>>>>>>>>>>传回去的" + roleTaskProgress.RoleTaskInfo + ">>>>>>>>>>>>");
                //Owner.ResponseData.Add((byte)ObjectParameterCode.Role, roleTaskProgress.RoleTaskInfo);
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
