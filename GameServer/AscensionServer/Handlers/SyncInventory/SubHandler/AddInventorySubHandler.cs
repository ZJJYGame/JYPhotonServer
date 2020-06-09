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
            var InventoryRoleObj = Utility.ToObject<Dictionary<int,HashSet<RingDTO>>>(InventoryData);

            foreach (var intRold in InventoryRoleObj)
            {
                NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", intRold.Key);
                bool exist = Singleton<NHManager>.Instance.Verify<RoleRing>(nHCriteriaRoleID);
                if (exist)
                {
                    var ringIdArray = intRold.Value;
                    AscensionServer._Log.Info("ringarray" + ringIdArray.Count);

                    var ringArray = Singleton<NHManager>.Instance.CriteriaGet<RoleRing>(nHCriteriaRoleID);
                    int ringCount = 0;
                    string ringitem = "";
                    foreach (var item in Utility.ToObject<List<int>>(ringArray.RingIdArray))
                    {
                        foreach (var n in ringIdArray)
                        {
                            if (item == n.ID)
                            {
                                NHCriteria nHCriteriaId = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", n.ID);
                                bool existId = Singleton<NHManager>.Instance.Verify<Ring>(nHCriteriaId);
                                if (existId)
                                {
                                    var ringData = Singleton<NHManager>.Instance.CriteriaGet<Ring>(nHCriteriaId);
                                    AscensionServer._Log.Info(ringData.RingItems + ">>>>>>>>>>>>>>>>>");
                                    var ClientObj = n.RingItems;
                                    Singleton<NHManager>.Instance.Update(new Ring() { ID = ringData.ID, RingId = ringData.RingId, RingItems = Utility.ToJson(n.RingItems) });
                                    AscensionServer._Log.Info(ringData.RingItems + ">>>>>>>>>>>>>>>>>");

                                    var ServerObj = ringData.RingItems;
                                    var objStr = Utility.ToObject<HashSet<RingItems>>(ringData.RingItems);
                                    //foreach (var key in ServerObj)
                                    //{
                                    //    AscensionServer._Log.Info(key.Value + ">>>>>>>>>>>>>>>>>");
                                    //}
                                    //AscensionServer._Log.Info(ServerObj.Count + ">>>>>>>>>>>>>>>>>");

                                    //if (ServerObj.RingItemId == ClientObj.RingItemId)
                                    //{
                                    //ringCount = ServerObj.RingItemCount + ClientObj.RingItemCount;
                                    //ServerObj.RingItemCount = ringCount;
                                    //Singleton<NHManager>.Instance.Update(new Ring() { ID = ringData.ID, RingId = ringData.RingId, RingItems = Utility.ToJson(n.RingItems) });
                                    //}
                                    //  else
                                    // {
                                    //ringitem = ServerObj.ToString() + ClientObj.ToString();
                                    //Singleton<NHManager>.Instance.Add(new Ring() { ID = ringData.ID, RingId = ringData.RingId, RingItems = Utility.ToJson(ringitem) });

                                    //  }
                                    //AscensionServer._Log.Info(Utility.ToJson(ClientObj) + ">>>>>>>>>>>>>>>>>");
                                    //var ServerObj = ringData.RingItems.ToList();
                                    //AscensionServer._Log.Info(ServerObj.Count+ ">>>>>>>>>>>>>>>>>");
                                    //foreach (var p in ServerObj)
                                    //{
                                    //AscensionServer._Log.Info(p.RingItemId + ">>>>>>>>>>>>>>>>>");

                                    //if (ClientObj.RingItemId == p.RingItemId)
                                    //{

                                    //}
                                    // }

                                    AscensionServer._Log.Info(ringData.RingItems+ ">>>>>>>>>>>>>>>>>");
                                }
                            }
                        }
                    }
                }

            }
        }
    }
}
//{100002,88,"1"}