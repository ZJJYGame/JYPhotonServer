using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using NHibernate.Linq.Clauses;
using AscensionProtocol.DTO;
using Renci.SshNet.Security;

namespace AscensionServer
{
    public class AddInventorySubHandler : SyncInventorySubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var InventoryData = Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Inventory) as string;
            AscensionServer._Log.Info(">>>>>接收背包的数据" + InventoryData + ">>>>>>>>>>>>>");
            var InventoryRoleObj = Utility.ToObject<RoleRing>(InventoryData);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", InventoryRoleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
            if (exist)
            {
                var ringIdArray = Utility.ToObject<Dictionary<int, List<string>>>(InventoryRoleObj.RingIdArray);
                var ringArray = Singleton<NHManager>.Instance.CriteriaGet<RoleRing>(nHCriteriaRoleID);
                string str = "";
                foreach (var item in Utility.ToObject<List<int>>(ringArray.RingIdArray))
                {
                    foreach (var n in ringIdArray)
                    {
                        if (item == n.Key)
                        {
                            NHCriteria nHCriteriaId = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", n.Key);
                            bool existId = Singleton<NHManager>.Instance.Verify<Ring>(nHCriteriaId);
                            if (existId)
                            {
                                var ringData = Singleton<NHManager>.Instance.CriteriaGet<Ring>(nHCriteriaId);
                                str = ringData.ItemArray + Utility.ToJson(n.Value);
                                foreach (var p in Utility.ToObject<List<string>>(ringData.ItemArray))
                                {
                                    AscensionServer._Log.Info(">>>>>接收背包的数据" + Utility.ToObject<List<string>>(ringData.ItemArray).Count);
                                }
                                //Singleton<NHManager>.Instance.Update(new Ring() {  ID = ringData.ID, RingID = ringData.RingID, ItemArray = str });
                            }
                        }
                    }
                }
            }
        }
    }
}

//{"1001":[10,"1"],"1002":[11,"0"],"1003":[12,"1"]}