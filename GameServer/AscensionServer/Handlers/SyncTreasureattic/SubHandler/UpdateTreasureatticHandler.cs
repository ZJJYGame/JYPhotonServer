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
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>" + Utility.Json.ToJson(schoolObj.TreasureAtticID) + "更新2藏宝阁" + Utility.Json.ToJson(treasureatticObj.ItemRedeemedDict));
            NHCriteria nHCriteriaschool = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            var treasureatticTemp = Singleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
            var schoolTemp = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaschool);
            var exit = Singleton<NHManager>.Instance.Verify<Treasureattic>(nHCriteriaTreasureattic);
            int contribution = 0;

            Dictionary<int, int> itemDict = new Dictionary<int, int>();
            Dictionary<string, string> DOdict = new Dictionary<string, string>();
            if (schoolTemp != null)
            {
                if (schoolTemp.Contribution > schoolObj.Contribution)
                {
                    contribution = schoolTemp.Contribution - schoolObj.Contribution;
                    Singleton<NHManager>.Instance.Update<School>(new School() { ID = schoolTemp.ID, SchoolID = schoolTemp.SchoolID, SchoolJob = schoolTemp.SchoolJob, TreasureAtticID = schoolTemp.TreasureAtticID, SutrasAtticID= schoolTemp.SutrasAtticID,Contribution = contribution });
                    if (exit)
                    {
                        if (!string.IsNullOrEmpty(treasureatticTemp.ItemRedeemedDict))
                        {

                            itemDict = Utility.Json.ToObject<Dictionary<int, int>>(treasureatticTemp.ItemRedeemedDict);
                            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>更新藏宝阁key" + Utility.Json.ToJson(itemDict));
                            foreach (var item in treasureatticObj.ItemRedeemedDict)
                            {
                                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>更新藏宝阁key" + item.Key);
                                if (itemDict.ContainsKey(item.Key))
                                {
                                    itemDict[item.Key] += treasureatticObj.ItemRedeemedDict[item.Key];
                                }
                                else
                                { itemDict.Add(item.Key, treasureatticObj.ItemRedeemedDict[item.Key]); }

                            }
                        }
                        else
                            itemDict = treasureatticObj.ItemRedeemedDict;
                        Singleton<NHManager>.Instance.Update<Treasureattic>(new Treasureattic() { ID = treasureatticTemp.ID, ItemAmountDict = treasureatticTemp.ItemAmountDict, ItemRedeemedDict = Utility.Json.ToJson(itemDict) });
                    }
                    var taSendObj = Singleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
                    var sSendObj = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaschool);
                    DOdict.Add("Treasureattic", Utility.Json.ToJson(new TreasureatticDTO() { ID = taSendObj.ID, ItemRedeemedDict = Utility.Json.ToObject<Dictionary<int, int>>(taSendObj.ItemRedeemedDict) }));
                    DOdict.Add("School", Utility.Json.ToJson(sSendObj));
                    SetResponseData(() =>
                    {

                        SubDict.Add((byte)ParameterCode.TreasureAttic, Utility.Json.ToJson(DOdict));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else
                {
                    SetResponseData(() =>
                    {
                        AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>传回到的藏宝阁s失败");
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }

            }
            else
            {
                SetResponseData(() =>
                {
                    AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>传回到的藏宝阁s失败");
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaTreasureattic, nHCriteriaschool);
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>传回到的藏宝阁" + Utility.Json.ToJson(DOdict));
        }

    }
}
