using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    public class UpdateSchoolHandler : SyncSchoolSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {


            var dict = ParseSubDict(operationRequest);
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.School));
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);

            string treasureatticJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.TreasureAttic));
            var treasureatticObj = Utility.Json.ToObject<Treasureattic>(treasureatticJson);

            NHCriteria nHCriteriaSchool = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            NHCriteria nHCriteriaTreasureattic = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", treasureatticObj.ID);

            var schoolTemp = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaSchool);
            var treasureatticTemp = Singleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
            if (schoolTemp != null)
            {
                //if (treasureatticTemp!=null)
                //{
                //    treasureatticTemp.ID = treasureatticObj.ID;
                //    treasureatticTemp.ItemAmountDict = Utility.Json.ToJson(treasureatticObj.ItemAmountDict);
                //    Singleton<NHManager>.Instance.Update(treasureatticTemp);
                //}
                schoolTemp.SchoolID = schoolObj.SchoolID;
                schoolTemp.SchoolJob = schoolObj.SchoolJob;
                Singleton<NHManager>.Instance.Update(schoolTemp);
                SetResponseData(() =>
                {

                    SubDict.Add((byte)ParameterCode.School, Utility.Json.ToJson(schoolTemp));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            else
            {
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaSchool);
        }
    }
}
