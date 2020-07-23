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
    public class UpdateSutrasAtticmHandler : SyncSutrasAtticmSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string sutrasAtticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.SutrasAtticm));
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.School));

            var sutrasAtticObj = Utility.Json.ToObject<SutrasAtticDTO>(sutrasAtticJson);
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);
            NHCriteria nHCriteriasutrasAttic = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", sutrasAtticObj.ID);
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>" + Utility.Json.ToJson(schoolObj.SutrasAtticID) + "更新2藏宝阁" + Utility.Json.ToJson(sutrasAtticObj.SutrasRedeemedDictl));
            NHCriteria nHCriteriaschool = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            var sutrasAtticTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<SutrasAttic>(nHCriteriasutrasAttic);
            var schoolTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaschool);
            var exit = ConcurrentSingleton<NHManager>.Instance.Verify<SutrasAttic>(nHCriteriasutrasAttic);
            int contribution = 0;

            Dictionary<int, int> itemDict = new Dictionary<int, int>();
            Dictionary<string, string> DOdict = new Dictionary<string, string>();
            if (schoolTemp != null)
            {
                if (schoolTemp.ContributionNow > schoolObj.ContributionNow)
                {
                    contribution = schoolTemp.ContributionNow - schoolObj.ContributionNow;
                    ConcurrentSingleton<NHManager>.Instance.Update<School>(new School() { ID = schoolTemp.ID, SchoolID = schoolTemp.SchoolID, SchoolJob = schoolTemp.SchoolJob, TreasureAtticID = schoolTemp.TreasureAtticID, SutrasAtticID = schoolTemp.SutrasAtticID, ContributionNow = contribution });
                    if (exit)
                    {
                        if (!string.IsNullOrEmpty(sutrasAtticTemp.SutrasRedeemedDictl))
                        {

                            itemDict = Utility.Json.ToObject<Dictionary<int, int>>(sutrasAtticTemp.SutrasRedeemedDictl);
                            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>更新藏宝阁key" + Utility.Json.ToJson(itemDict));
                            foreach (var item in sutrasAtticObj.SutrasRedeemedDictl)
                            {
                                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>更新藏宝阁key" + item.Key);
                                if (itemDict.ContainsKey(item.Key))
                                {
                                    itemDict[item.Key] += sutrasAtticObj.SutrasRedeemedDictl[item.Key];
                                }
                                else
                                { itemDict.Add(item.Key, sutrasAtticObj.SutrasRedeemedDictl[item.Key]); }

                            }
                        }
                        else
                            itemDict = sutrasAtticObj.SutrasRedeemedDictl;
                        ConcurrentSingleton<NHManager>.Instance.Update<SutrasAttic>(new SutrasAttic() { ID = sutrasAtticTemp.ID, SutrasAmountDict= sutrasAtticTemp.SutrasAmountDict, SutrasRedeemedDictl = Utility.Json.ToJson(itemDict) });
                    }
                    var saSendObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<SutrasAttic>(nHCriteriasutrasAttic);
                    var sSendObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaschool);
                    DOdict.Add("SutrasAttic", Utility.Json.ToJson(new SutrasAtticDTO() { ID = saSendObj.ID, SutrasRedeemedDictl = Utility.Json.ToObject<Dictionary<int, int>>(saSendObj.SutrasRedeemedDictl) }));
                    DOdict.Add("School", Utility.Json.ToJson(sSendObj));
                    SetResponseData(() =>
                    {

                        SubDict.Add((byte)ParameterCode.SutrasAtticm, Utility.Json.ToJson(DOdict));
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
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriasutrasAttic, nHCriteriaschool);
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>传回到的藏宝阁" + Utility.Json.ToJson(DOdict));
        }
    }
}
