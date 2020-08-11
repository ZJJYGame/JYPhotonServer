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
  public   class UpdateShoppingMallSubHandler: SyncShoppingMallSubHandler
    {

        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string rolepurchaseJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RolePurchase));
            var rolepurchaseObj = Utility.Json.ToObject<RolePurchaseRecordDTO>(rolepurchaseJson);
            NHCriteria nHCriteriarolepurchase = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolepurchaseObj.RoleID);

            var rolepurchasetemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RolePurchaseRecord>(nHCriteriarolepurchase);
            if (rolepurchasetemp != null)
            {
                if (rolepurchasetemp.GoodsPurchasedCount.Equals("[]"))
                {
                    rolepurchasetemp.GoodsPurchasedCount = Utility.Json.ToJson(rolepurchaseObj.GoodsPurchasedCount);
                    ConcurrentSingleton<NHManager>.Instance.Update(rolepurchasetemp);
                }
                else
                {
                    var rolepurchaseDict = Utility.Json.ToObject<Dictionary<int, int>>(rolepurchasetemp.GoodsPurchasedCount);
                    foreach (var item in rolepurchaseObj.GoodsPurchasedCount)
                    {
                        if (rolepurchaseDict.ContainsKey(item.Key))
                        {

                        }
                    }
                }
            }

        }

    }
}
