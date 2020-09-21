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
using StackExchange.Redis;

namespace AscensionServer
{
    public class GetVareityShopSubHandler : SyncVareityShopSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }


        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string vareityJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.VareityShop));
            var vareityObj = Utility.Json.ToObject<VareityShopDTO>(vareityJson);
            string vareitypurchaseJson= Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.VareityPurchase));
            var vareitypurchaseObj=Utility.Json.ToObject<VareityPurchaseRecordDTO>(vareitypurchaseJson);

            var name = RedisData.Initialize.InsertName("VAREITY_SHOP", vareityObj.VareityshopID);
            var content = RedisData.Initialize.GetData(name);
            var vareityname = RedisData.Initialize.InsertName("VAREITY_BUY_COUNT", vareitypurchaseObj.RoleID);
            var vareitycontent = RedisData.Initialize.GetData(vareityname);

            Dictionary<int, List<GoodsStatus>> AllGoodsList = new Dictionary<int, List<GoodsStatus>>();
            Dictionary<string, string> shopDIct = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(vareitycontent))
            {
               // Utility.Debug.LogInfo("储存进Redis成功了杂货铺名字是" + vareityname + "内容是" + vareitycontent);
                NHCriteria nHCriteriavareitycontent = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", vareitypurchaseObj.RoleID);
                var vareitycontentTemp = NHibernateQuerier.CriteriaSelect<VareityPurchaseRecord>(nHCriteriavareitycontent);
                if (vareitycontentTemp != null)
                {
                   
                    if (!string.IsNullOrEmpty(vareitycontentTemp.VareityPurchasedCount))
                    {
                        //Utility.Debug.LogInfo("储存进数据库成功了杂货铺" + vareityname + "内容是" + vareitycontentTemp.VareityPurchasedCount);
                    await   RedisHelper.String.StringSetAsync(vareityname, vareitycontentTemp.VareityPurchasedCount);
                        VareityPurchaseRecordDTO vareityPurchaseRecordDTO = new VareityPurchaseRecordDTO()
                        {
                            RoleID = vareitypurchaseObj.RoleID,
                            VareityPurchasedCount = Utility.Json.ToObject<Dictionary<int, int>>(vareitycontentTemp.VareityPurchasedCount)
                        };
                        shopDIct.Add("VareityPurchaseRecord",Utility.Json.ToJson(vareityPurchaseRecordDTO));
                    }
                }
            }
            else
            {
                VareityPurchaseRecordDTO vareityPurchaseRecordDTO = new VareityPurchaseRecordDTO()
                {
                    RoleID = vareitypurchaseObj.RoleID,
                    VareityPurchasedCount = Utility.Json.ToObject<Dictionary<int, int>>(vareitycontent)
                };
                shopDIct.Add("VareityPurchaseRecord", Utility.Json.ToJson(vareityPurchaseRecordDTO));
            }

            if (string.IsNullOrEmpty(content))
            {
                NHCriteria nHCriteriavareity = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("VareityshopID", vareityObj.VareityshopID);
                var vareityTemp = NHibernateQuerier.CriteriaSelect<VareityShop>(nHCriteriavareity);

                if (vareityTemp != null)
                {
                    if (!string.IsNullOrEmpty(vareityTemp.AllGoods))
                        {
                       await RedisHelper.String.StringSetAsync(name, vareityTemp.AllGoods);
                        AllGoodsList = Utility.Json.ToObject<Dictionary<int, List<GoodsStatus>>>(vareityTemp.AllGoods);
                        VareityShopDTO vareityShopDTO = new VareityShopDTO() { VareityshopID = vareityObj.VareityshopID, AllGoods = AllGoodsList };
                        shopDIct.Add("VareityShop", Utility.Json.ToJson(vareityShopDTO));
                        GameManager.ReferencePoolManager.Despawns(nHCriteriavareity);
                    }
                }
            }
            else
            {
                Utility.Debug.LogInfo("储存进Redis成功了");
                AllGoodsList = Utility.Json.ToObject<Dictionary<int, List<GoodsStatus>>>(content);
                VareityShopDTO vareityShopDTO = new VareityShopDTO() { VareityshopID = vareityObj.VareityshopID, AllGoods = AllGoodsList };
                shopDIct.Add("VareityShop", Utility.Json.ToJson(vareityShopDTO));
            }
            SetResponseData(() =>
            {
                SubDict.Add((byte)ParameterCode.VareityShop, Utility.Json.ToJson(shopDIct));
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            });
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            
        }
    
    }
}
