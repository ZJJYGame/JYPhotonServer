using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    public class UpdateVareityShopSubHandler : SyncVareityShopSubHandler
    {

        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string rolepurchaseJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.VareityPurchase));
            var rolepurchaseObj = Utility.Json.ToObject<VareityPurchaseRecordDTO>(rolepurchaseJson);
            NHCriteria nHCriteriarolepurchase = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolepurchaseObj.RoleID);
            AscensionServer._Log.Info("传过来的杂货铺购买数据" + rolepurchaseJson);
            var rolepurchasetemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<VareityPurchaseRecord>(nHCriteriarolepurchase);
            if (rolepurchasetemp != null)
            {
                if (rolepurchasetemp.VareityPurchasedCount.Equals("{}"))
                {
                    rolepurchasetemp.VareityPurchasedCount = Utility.Json.ToJson(rolepurchaseObj.VareityPurchasedCount);
                    ConcurrentSingleton<NHManager>.Instance.Update(rolepurchasetemp);
                }
                else
                {
                    var rolepurchaseDict = Utility.Json.ToObject<Dictionary<int, int>>(rolepurchasetemp.VareityPurchasedCount);
                    foreach (var item in rolepurchaseObj.VareityPurchasedCount)
                    {
                        if (rolepurchaseDict.ContainsKey(item.Key))
                        {
                            rolepurchaseDict[item.Key] += item.Value;
                        }
                        else
                            rolepurchaseDict.Add(item.Key, item.Value);
                    }
                    rolepurchasetemp.VareityPurchasedCount = Utility.Json.ToJson(rolepurchaseDict);
                    ConcurrentSingleton<NHManager>.Instance.Update(rolepurchasetemp);
                }
                VareityPurchaseRecordDTO vareityPurchaseRecordDTO = new VareityPurchaseRecordDTO() { RoleID = rolepurchasetemp.RoleID, VareityPurchasedCount = Utility.Json.ToObject<Dictionary<int, int>>(rolepurchasetemp.VareityPurchasedCount) };
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.VareityPurchase, Utility.Json.ToJson(vareityPurchaseRecordDTO));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarolepurchase);
        }
    }
}
