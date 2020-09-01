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
    public class GetShoppingMallSubHandler : SyncShoppingMallSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string shoppingmallJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ShoppingMall));
            var shoppingmallObj = Utility.Json.ToObject<ShoppingMallDTO>(shoppingmallJson);
            string rolepurchaseJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RolePurchase));
            var rolepurchaseObj = Utility.Json.ToObject<RolePurchaseRecordDTO>(rolepurchaseJson);
            Utility.Debug.LogInfo("得到的商店數據"+ rolepurchaseObj.RoleID);
            NHCriteria nHCriteriarolepurchase = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rolepurchaseObj.RoleID);
            NHCriteria nHCriteriashoppingmall = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", shoppingmallObj.ID);


            var rolepurchasetemp= ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RolePurchaseRecord>(nHCriteriarolepurchase);
            var shoppingmalltemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<ShoppingMall>(nHCriteriashoppingmall);
            //记录传输的总商店物品及已经兑换的商店物品
            Dictionary<string, string> shopDIct = new Dictionary<string, string>();
            if (shoppingmalltemp!=null)
            {
                var MaterialsList = Utility.Json.ToObject<List<ShoppingGoods>>(shoppingmalltemp.Materials);
                var NewArrivalList = Utility.Json.ToObject<List<ShoppingGoods>>(shoppingmalltemp.NewArrival);
                var QualifiedToBuyList = Utility.Json.ToObject<List<ShoppingGoods>>(shoppingmalltemp.QualifiedToBuy);
                var RechargeStoreList = Utility.Json.ToObject<List<RechargeGoods>>(shoppingmalltemp.RechargeStore);
                ShoppingMallDTO shoppingMallDTO = new ShoppingMallDTO() { ID = shoppingmalltemp.ID, Materials = MaterialsList, NewArrival = NewArrivalList, QualifiedToBuy = QualifiedToBuyList,RechargeStore= RechargeStoreList };
                shopDIct.Add("ShoppingMall", Utility.Json.ToJson(shoppingMallDTO));

                if (rolepurchasetemp!=null&& !rolepurchasetemp.GoodsPurchasedCount.Equals("{}"))
                {
                    RolePurchaseRecordDTO rolePurchaseRecordDTO = new RolePurchaseRecordDTO() { GoodsPurchasedCount= Utility.Json.ToObject<Dictionary<int,int>>(rolepurchasetemp.GoodsPurchasedCount),RoleID= rolepurchasetemp .RoleID};
                    shopDIct.Add("RolePurchaseRecord", Utility.Json.ToJson(rolePurchaseRecordDTO));
                }
                else
                {
                    RolePurchaseRecordDTO rolePurchaseRecordDTO = new RolePurchaseRecordDTO() { RoleID = rolepurchasetemp.RoleID };
                    shopDIct.Add("RolePurchaseRecord", Utility.Json.ToJson(rolePurchaseRecordDTO));
                }
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.ShoppingMall, Utility.Json.ToJson(shopDIct));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriashoppingmall);
        }
    }
}
