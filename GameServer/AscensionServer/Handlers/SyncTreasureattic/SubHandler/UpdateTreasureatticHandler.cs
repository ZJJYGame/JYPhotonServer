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
    public class UpdateTreasureatticHandler : SyncTreasureatticSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string treasureatticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.TreasureAttic));
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.School));

            var treasureatticObj = Utility.Json.ToObject<TreasureatticDTO>(treasureatticJson);
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);
           NHCriteria nHCriteriaTreasureattic = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", treasureatticObj.ID);
           
            NHCriteria nHCriteriaschool = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
           var treasureatticTemp = Singleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
            var schoolTemp = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaschool);
            var exit = Singleton<NHManager>.Instance.Verify<Treasureattic>(nHCriteriaTreasureattic);
            int contribution=0;
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>更新藏宝阁" + treasureatticTemp.ID);
            Dictionary<int, int> itemDict = new Dictionary<int, int>();
            Dictionary<string, string> DOdict = new Dictionary<string, string>();
            if (schoolTemp != null)
            {
               // AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>更新藏宝阁2");
                contribution = schoolTemp.Contribution - schoolObj.Contribution;
                Singleton<NHManager>.Instance.Update<School>(new School() { ID = schoolTemp.ID, SchoolID = schoolTemp.SchoolID, SchoolJob = schoolTemp.SchoolJob, TreasureAtticID = schoolTemp.TreasureAtticID, Contribution = contribution });
                if (exit)
                {
                    itemDict = Utility.Json.ToObject<Dictionary<int, int>>(treasureatticTemp.ItemRedeemedDict);
                   // AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>更新藏宝阁key" + Utility.Json.ToJson(treasureatticObj.ItemRedeemedDict));
                    foreach (var item in treasureatticObj.ItemRedeemedDict)
                    {
                       // AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>更新藏宝阁key" + item.Key);
                        if (itemDict.ContainsKey(item.Key))
                        {
                            itemDict[item.Key] += treasureatticObj.ItemRedeemedDict[item.Key];
                        }
                        else
                        { itemDict.Add(item.Key, treasureatticObj.ItemRedeemedDict[item.Key]); }

                    }

                    Singleton<NHManager>.Instance.Update<Treasureattic>(new Treasureattic() { ID = treasureatticTemp.ID, ItemAmountDict = Utility.Json.ToJson(treasureatticObj), ItemRedeemedDict = Utility.Json.ToJson(itemDict) });
                }
                var taSendObj = Singleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
                var sSendObj = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaschool);
                DOdict.Add("Treasureattic", Utility.Json.ToJson(taSendObj));
                DOdict.Add("School", Utility.Json.ToJson(sSendObj));
                SetResponseData(() =>
                {

                    SubDict.Add((byte)ParameterCode.TreasureAttic, Utility.Json.ToJson(DOdict));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
                });
            }
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>传回到的藏宝阁"+ Utility.Json.ToJson(DOdict));
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaTreasureattic, nHCriteriaschool);
        }
    }
}