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
namespace AscensionServer
{
    public class UpdateRoleStatusSubHandler : SyncRoleStatusSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            AscensionServer._Log.Info(">>>>>>>>>>>>VerifyRoleStatusHandler\n进来更新的战斗数据:VerifyRoleStatusHandler\n<<<<<<<<<<<");
            var dict = ParseSubDict(operationRequest);
            string rolestatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.RoleStatus));
            AscensionServer._Log.Info(">>>>>>>>>>>>VerifyRoleStatusHandler\n传输过来更新的战斗数据:" + rolestatusJson + "VerifyRoleStatusHandler\n<<<<<<<<<<<");
            var rolestatusObj = Utility.ToObject<RoleStatus>(rolestatusJson);
            NHCriteria nHCriteriaRoleStatue = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolestatusObj.RoleID);
            Utility.Assert.Predicate(() => Singleton<NHManager>.Instance.Verify<RoleStatus>(nHCriteriaRoleStatue), () =>
            {
                Singleton<NHManager>.Instance.Update(rolestatusObj);
                SetResponseData(() => Owner.OpResponse.ReturnCode = (short)ReturnCode.Success);
            }, () => {
                SetResponseData(() => Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail);
            });
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}