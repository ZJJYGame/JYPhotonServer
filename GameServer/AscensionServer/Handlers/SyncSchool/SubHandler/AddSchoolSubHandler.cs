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
    public class AddSchoolSubHandler : SyncSchoolSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }
        

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

            var dict = ParseSubDict(operationRequest);
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.School));
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);

            AscensionServer._Log.Info(">>>>>>>加入1宗门的请求收到了"+ schoolJson);
            string treasureatticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.TreasureAttic));
            var treasureatticObj = Utility.Json.ToObject<TreasureatticDTO>(treasureatticJson);

            string sutrasAtticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.SutrasAtticm));
            var sutrasAtticObj = Utility.Json.ToObject<SutrasAtticDTO>(sutrasAtticJson);

            AscensionServer._Log.Info(">>>>>>>加入2宗门的请求收到了"+ treasureatticJson);
            NHCriteria nHCriteriaSchool = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            NHCriteria nHCriteriaTreasureattic = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", treasureatticObj.ID);
            NHCriteria nHCriteriasutrasAttic = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", sutrasAtticObj.ID);

            var schoolTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaSchool);
            var treasureatticTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
            var sutrasAtticTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<SutrasAttic>(nHCriteriasutrasAttic);
            Dictionary<string, string> DTOdict = new Dictionary<string, string>();
            if (schoolTemp != null)
            {

                AscensionServer._Log.Info(">>>>>>>加入宗门的请求收到了"+ schoolTemp.SchoolID);
                if (treasureatticTemp != null)
                {
                    treasureatticTemp.ID = treasureatticObj.ID;
                    treasureatticTemp.ItemAmountDict = Utility.Json.ToJson(treasureatticObj.ItemAmountDict);
                    ConcurrentSingleton<NHManager>.Instance.Update(treasureatticTemp);
                }
                if (sutrasAtticTemp != null)
                    {
                    sutrasAtticTemp.ID = sutrasAtticObj.ID;
                    sutrasAtticTemp.SutrasAmountDict = Utility.Json.ToJson(sutrasAtticObj.SutrasAmountDict);
                        ConcurrentSingleton<NHManager>.Instance.Update(sutrasAtticTemp);
                }
                    schoolTemp.SchoolID = schoolObj.SchoolID;
                schoolTemp.SchoolJob = schoolObj.SchoolJob;
                ConcurrentSingleton<NHManager>.Instance.Update(schoolTemp);
                var schoolSendObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaSchool);
                SetResponseData(() =>
                {
                    AscensionServer._Log.Info(">>>>>>>返回加入宗门的数据" + Utility.Json.ToJson(schoolSendObj));
                    SubDict.Add((byte)ParameterCode.School, Utility.Json.ToJson(schoolSendObj));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
                SetResponseData(() =>{Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail; });
            AscensionServer._Log.Info(">>>>>>>加入宗门的请求收到了2" + schoolTemp.SchoolID);

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaSchool, nHCriteriaTreasureattic, nHCriteriasutrasAttic);
        }
    }
}
