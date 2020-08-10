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
            NHCriteria nHCriteriashoppingmall = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", shoppingmallObj.ID);

            var shoppingmalltemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<ShoppingMall>(nHCriteriashoppingmall);
            //List<ShoppingGoods> MaterialsList=new List<ShoppingGoods>();
            List<ShoppingGoods> NewArrivalList = new List<ShoppingGoods>();
            List<ShoppingGoods> QualifiedToBuyList = new List<ShoppingGoods>();

            if (shoppingmalltemp!=null)
            {
                AscensionServer._Log.Info("得到的商城數據" + shoppingmalltemp.Materials);
                var  MaterialsList = Utility.Json.ToObject<List<ShoppingGoods>>(shoppingmalltemp.Materials);
                AscensionServer._Log.Info("得到的商城數據" + MaterialsList.Count);
                NewArrivalList = Utility.Json.ToObject<List<ShoppingGoods>>(shoppingmalltemp.NewArrival);
                AscensionServer._Log.Info("2發送回去的商城數據" + shoppingmalltemp.Materials);
                QualifiedToBuyList = Utility.Json.ToObject<List<ShoppingGoods>>(shoppingmalltemp.QualifiedToBuy);

                ShoppingMallDTO shoppingMallDTO = new ShoppingMallDTO() {ID= shoppingmalltemp.ID,Materials= MaterialsList,NewArrival= NewArrivalList,QualifiedToBuy= QualifiedToBuyList };
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.ShoppingMall, Utility.Json.ToJson(shoppingMallDTO));
                    AscensionServer._Log.Info("發送回去的商城數據"+Utility.Json.ToJson(shoppingMallDTO));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            //else
            //    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriashoppingmall);
        }
    }
}
