﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class UpdateRoleStatusSubHandler : SyncRoleStatusSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            Utility.Debug.LogInfo(">>>>>>>>>>>>VerifyRoleStatusHandler\n进来更新的战斗数据:VerifyRoleStatusHandler\n<<<<<<<<<<<");
            var dict = ParseSubDict(operationRequest);
            string rolestatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleStatus));
            Utility.Debug.LogInfo(">>>>>>>>>>>>VerifyRoleStatusHandler\n传输过来更新的战斗数据:" + rolestatusJson + "VerifyRoleStatusHandler\n<<<<<<<<<<<");
            var rolestatusObj = Utility.Json.ToObject<RoleStatus>(rolestatusJson);
            NHCriteria nHCriteriaRoleStatue = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rolestatusObj.RoleID);
            var result =NHibernateQuerier.Verify<RoleStatus>(nHCriteriaRoleStatue);
            if (result)
            {
                NHibernateQuerier.Update(rolestatusObj);
                SetResponseData(() => Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success);
            }
            else
            {
                SetResponseData(() => Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail);
            }
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
        }
    }
}
