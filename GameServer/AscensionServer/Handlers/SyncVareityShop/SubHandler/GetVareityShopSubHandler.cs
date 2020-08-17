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
    public class GetVareityShopSubHandler : SyncVareityShopSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string vareityJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.VareityShop));
            var vareityObj = Utility.Json.ToObject<VareityShopDTO>(vareityJson);
            NHCriteria nHCriteriavareity = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("VareityshopID", vareityObj.VareityshopID);
            var vareityTemp= ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<VareityShop>(nHCriteriavareity);
            Dictionary<int, Dictionary<int, GoodsStatus>> AllGoodsDict = new Dictionary<int, Dictionary<int, GoodsStatus>>();
            if (vareityTemp!=null)
            {
                if (string.IsNullOrEmpty(vareityTemp.AllGoods))
                {
                    var allgoodsDict = Utility.Json.ToObject<Dictionary<int, string>>(vareityTemp.AllGoods);
                    foreach (var item in allgoodsDict)
                    {
                        var goodsDict = Utility.Json.ToObject<Dictionary<int, GoodsStatus>>(item.Value);
                        AllGoodsDict.Add(item.Key, goodsDict);
                    }
                    SetResponseData(() =>
                    {
                        VareityShopDTO vareityShopDTO = new VareityShopDTO() { VareityshopID= vareityTemp .VareityshopID,AllGoods= AllGoodsDict };
                        SubDict.Add((byte)ParameterCode.VareityShop, Utility.Json.ToJson(vareityShopDTO));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriavareity);
        }
    }
}
