using System;
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
    public class GetRoleStatusSubHandler : SyncRoleStatusSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

            var dict = ParseSubDict(operationRequest);
            string roleJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role));
            
            var roleObj = Utility.Json.ToObject<Role>(roleJson);
            NHCriteria nHCriteriaRoleId = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleObj.RoleID);
            bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleId);
            if (exist)
            {
                AscensionServer._Log.Info("------------------------------------" + "获取人物数据  : " + roleJson + "---------------------------------------");
                AscensionServer.Instance.Online(peer, roleObj);
                RoleStatus roleStatus = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleStatus>(nHCriteriaRoleId);
                AscensionServer._Log.Info("------------------------------------GetRoleStatusSubHandler\n" + "RoleStatus  : " + roleStatus + "\nGetRoleStatusSubHandler---------------------------------------");
                string roleStatusJson = Utility.Json.ToJson(roleStatus);
                RoleRing roleRing =  ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleRing>(nHCriteriaRoleId);
                SetResponseData(() => {SubDict.Add((byte)ParameterCode.RoleStatus, roleStatusJson); SubDict.Add((byte)ParameterCode.Inventory, Utility.Json.ToObject<Dictionary<int,int>>(roleRing.RingIdArray)); });
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleId);
        }
    }
}
