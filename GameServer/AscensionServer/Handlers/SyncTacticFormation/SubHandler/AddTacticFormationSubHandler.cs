using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using AscensionProtocol.DTO;

namespace AscensionServer
{
    public class AddTacticFormationSubHandler : SyncTacticFormationSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string tacticFormationJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobTacticFormation));
            var tacticFormationObj = Utility.Json.ToObject<TacticFormationDTO>(tacticFormationJson);
            NHCriteria nHCriteriatacticFormation = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", tacticFormationObj.RoleID);
            var tacticFormatioTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<TacticFormation>(nHCriteriatacticFormation);
            HashSet<int> tacticFormationHash = new HashSet<int>();
            if (true)
            {

            }




        }
    }
}
