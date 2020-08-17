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
using RedisDotNet;

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
            Dictionary<int, List< GoodsStatus>> AllGoodsList = new Dictionary<int, List<GoodsStatus>>();
            if (vareityTemp!=null)
            {
                if (!string.IsNullOrEmpty(vareityTemp.AllGoods))
                {
                    
                    //RedisTest("ISme", vareityTemp.AllGoods);
                  
                    AllGoodsList = Utility.Json.ToObject<Dictionary<int, List<GoodsStatus>>>(vareityTemp.AllGoods);
                    SetResponseData(() =>
                    {
                        VareityShopDTO vareityShopDTO = new VareityShopDTO() { VareityshopID = vareityTemp.VareityshopID, AllGoods = AllGoodsList };

                        SubDict.Add((byte)ParameterCode.VareityShop, Utility.Json.ToJson(vareityShopDTO));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriavareity);
            //string str = RedisHelper.String.StringGetAsync<string>("ISme").Result;
            //AscensionServer._Log.Info("获得的redis的数据为" + str);
        }


        void RedisTest(string name ,string content)
        {
            RedisManager.Instance.OnInitialization();
            RedisHelper.String.StringGetSetAsync(name, content);
        }

    }
}
