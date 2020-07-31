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
    public class GetTacticFormationSubHandler : SyncTacticFormationSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string tacticformationJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobTacticFormation));
            var tacticformationObj = Utility.Json.ToObject<TacticFormationDTO>(tacticformationJson);
            NHCriteria nHCriteriatacticformation = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", tacticformationObj.RoleID);
            AscensionServer._Log.Info("得到的阵法配方" + tacticformationJson);
            var tacticformationtemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<TacticFormation>(nHCriteriatacticformation);
            if (tacticformationtemp != null)
            {
                if (!string.IsNullOrEmpty(tacticformationtemp.Recipe_Array))
                {
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.JobTacticFormation, Utility.Json.ToJson(tacticformationtemp));

                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                    AscensionServer._Log.Info("得到的阵法配方" + Utility.Json.ToJson(tacticformationtemp));
                }
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                SubDict.Add((byte)ParameterCode.JobTacticFormation, Utility.Json.ToJson(new List<string>()));
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawns(nHCriteriatacticformation);
        }
    }
}
