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
            NHCriteria nHCriteriaTreasureattic = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", treasureatticObj.ID);
            NHCriteria nHCriteriaschool = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            var treasureatticTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
            var schoolTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaschool);
            var exit = ConcurrentSingleton<NHManager>.Instance.Verify<Treasureattic>(nHCriteriaTreasureattic);
            int contribution = 0;

            Dictionary<int, int> itemDict = new Dictionary<int, int>();
            Dictionary<string, string> DOdict = new Dictionary<string, string>();
            if (schoolTemp != null)
            {
                if (schoolTemp.ContributionNow > schoolObj.ContributionNow)
                {
                    contribution = schoolTemp.ContributionNow - schoolObj.ContributionNow;
                    ConcurrentSingleton<NHManager>.Instance.Update<School>(new School() { ID = schoolTemp.ID, SchoolID = schoolTemp.SchoolID, SchoolJob = schoolTemp.SchoolJob, TreasureAtticID = schoolTemp.TreasureAtticID, SutrasAtticID= schoolTemp.SutrasAtticID, ContributionNow = contribution });
                    if (exit)
                    {
                        if (!string.IsNullOrEmpty(treasureatticTemp.ItemRedeemedDict))
                        {

                            itemDict = Utility.Json.ToObject<Dictionary<int, int>>(treasureatticTemp.ItemRedeemedDict);
                            foreach (var item in treasureatticObj.ItemRedeemedDict)
                            {
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
                        ConcurrentSingleton<NHManager>.Instance.Update<Treasureattic>(new Treasureattic() { ID = treasureatticTemp.ID, ItemAmountDict = treasureatticTemp.ItemAmountDict, ItemRedeemedDict = Utility.Json.ToJson(itemDict) });
                    }
                    var taSendObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
                    var sSendObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaschool);
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
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }

            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaTreasureattic, nHCriteriaschool);
        }

    }
}
