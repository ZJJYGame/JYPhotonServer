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
    public class GetSchoolSubHandler : SyncSchoolSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.School));
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);
            NHCriteria nHCriteriaSchool = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            var schoolTemp = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaSchool);
            List<string > DTOList = new List<string>(); ;
            if (schoolTemp!=null)
            {
                DTOList.Clear();
                DTOList.Add(Utility.Json.ToJson( schoolTemp));
                NHCriteria nHCriteriaTreasureattic = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolTemp.TreasureAtticID);
                var TreasureatticTemp = Singleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
                if (TreasureatticTemp!=null)
                {
                    DTOList.Add(Utility.Json.ToJson(new TreasureatticDTO() { ID = TreasureatticTemp.ID, ItemAmountDict = Utility.Json.ToObject<Dictionary<int, int>>(TreasureatticTemp.ItemAmountDict), ItemRedeemedDict = Utility.Json.ToObject<Dictionary<int, int>>(TreasureatticTemp.ItemRedeemedDict) }));
                }
                NHCriteria nHCriteriaSutrasAttic = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolTemp.SutrasAtticID);
                var SutrasAtticTemp = Singleton<NHManager>.Instance.CriteriaSelect<SutrasAttic>(nHCriteriaSutrasAttic);

                if (SutrasAtticTemp != null)
                {
                    DTOList.Add(Utility.Json.ToJson(new SutrasAtticDTO() { ID = SutrasAtticTemp.ID, SutrasAmountDict = Utility.Json.ToObject<Dictionary<int, int>>(SutrasAtticTemp.SutrasAmountDict), SutrasRedeemedDictl = Utility.Json.ToObject<Dictionary<int, int>>(SutrasAtticTemp.SutrasRedeemedDictl) }));
                }
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaTreasureattic, nHCriteriaSutrasAttic);
            }
            else     
                SetResponseData(() => {Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail; });

            if (DTOList.Count >0)
            {
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.School, Utility.Json.ToJson(DTOList));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;

                });

            }
            else
            {
                SetResponseData(() => { Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail; });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaSchool);

        }
    }
}
