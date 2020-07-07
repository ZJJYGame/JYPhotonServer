using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    public class VerifyTreasureAtticHandler : SyncTreasureatticSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Verify;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.School));
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);
            NHCriteria nHCriteriaSchool = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("", schoolObj.ID);
            var schoolTemp = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaSchool);
            if (schoolTemp!=null)
            {
                if (schoolTemp.Contribution> schoolObj.Contribution)
                {
                    SetResponseData(() =>
                    {

                        SubDict.Add((byte)ParameterCode.School, Utility.Json.ToJson(true));
                        Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                    });
                }
                else
                {
                    SubDict.Add((byte)ParameterCode.School, Utility.Json.ToJson(false));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                }
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaSchool);
        }
    }
}
