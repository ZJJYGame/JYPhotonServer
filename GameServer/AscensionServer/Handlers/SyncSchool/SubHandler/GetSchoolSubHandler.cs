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
            List<DataObject> DTOList = new List<DataObject>(); ;
            if (schoolTemp!=null)
            {
                DTOList.Add(schoolTemp);
                NHCriteria nHCriteriaTreasureattic = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolTemp.TreasureAtticID);
                var TreasureatticTemp = Singleton<NHManager>.Instance.CriteriaSelect<Treasureattic>(nHCriteriaTreasureattic);
                if (TreasureatticTemp!=null)
                {
                    DTOList.Add(TreasureatticTemp);
                }
                NHCriteria nHCriteriaSutrasAttic = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolTemp.SutrasAtticID);
                var SutrasAtticTemp = Singleton<NHManager>.Instance.CriteriaSelect<SutrasAttic>(nHCriteriaSutrasAttic);
                if (SutrasAtticTemp != null)
                {
                    DTOList.Add(SutrasAtticTemp);
                }
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaTreasureattic, nHCriteriaSutrasAttic);
            }
            else     
                SetResponseData(() => {Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail; });

            if (DTOList.Count < 3)
            {
                SetResponseData(() => { Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail; });
            }
            else
            {
                SetResponseData(() =>
                {

                    SubDict.Add((byte)ParameterCode.School, Utility.Json.ToJson(DTOList));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                });
            }
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaSchool);
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);

        }
    }
}
