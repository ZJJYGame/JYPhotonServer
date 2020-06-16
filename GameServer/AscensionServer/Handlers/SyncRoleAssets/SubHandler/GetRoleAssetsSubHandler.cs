﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class GetRoleAssetsSubHandler : SyncRoleAssetsSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>>GetRoleAssetsSubHandler\n" + roleJson + "\n GetRoleAssetsSubHandler >>>>>>>>>>>>>>>>>>>>>>");
            var roleObj = Utility.Json.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            var obj =Singleton<NHManager>.Instance.CriteriaSelect<Role>(nHCriteriaRoleID);
            if (obj != null)
            {
                var result = Singleton<NHManager>.Instance.CriteriaSelect<RoleAssets>(nHCriteriaRoleID);
                if (result == null)
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>\n GetRoleAssetsSubHandler  " + roleObj.RoleID + "  GetRoleAssetsSubHandler  \n >>>>>>>>>>>>>>>>>>>>>>");
                    result = Singleton<NHManager>.Instance.Insert(new RoleAssets() { RoleID = roleObj.RoleID });
                }
                string roleAssetsJson = Utility.Json.ToJson(result);
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ObjectParameterCode.RoleAssets, roleAssetsJson);
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaRoleID);
        }
    }
}
