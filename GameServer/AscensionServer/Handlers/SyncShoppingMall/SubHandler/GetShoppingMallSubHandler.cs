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
            NHCriteria nHCriteriashoppingmall = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", 1);
            AscensionServer._Log.Info("得到的商城數據"+ shoppingmallObj.ID);
            var shoppingmalltemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<ShoppingMall>(nHCriteriashoppingmall);
            AscensionServer._Log.Info("發送回去的商城數據" + shoppingmalltemp.Materials);
            List<ShoppingGoods> MaterialsList;
            List<ShoppingGoods> NewArrivalList;
            List<ShoppingGoods> QualifiedToBuyList;

            if (shoppingmalltemp!=null)
            {
               MaterialsList = Utility.Json.ToObject<List<ShoppingGoods>>(shoppingmalltemp.Materials);
                NewArrivalList = Utility.Json.ToObject<List<ShoppingGoods>>(shoppingmalltemp.NewArrival);
                QualifiedToBuyList = Utility.Json.ToObject<List<ShoppingGoods>>(shoppingmalltemp.QualifiedToBuy);

                ShoppingMallDTO shoppingMallDTO = new ShoppingMallDTO() {ID= shoppingmallObj.ID,Materials= MaterialsList,NewArrival= NewArrivalList,QualifiedToBuy= QualifiedToBuyList };
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.ShoppingMall, shoppingMallDTO);
                    AscensionServer._Log.Info("發送回去的商城數據"+Utility.Json.ToJson(shoppingMallDTO));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriashoppingmall);
        }
    }
}
