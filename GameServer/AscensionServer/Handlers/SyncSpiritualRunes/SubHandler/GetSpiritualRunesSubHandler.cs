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
    public class GetSpiritualRunesSubHandler : SyncSpiritualRuneSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string spiritualrunesJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobSpiritualRunes));
            var spiritualrunesObj = Utility.Json.ToObject<SpiritualRunesDTO>(spiritualrunesJson);
            NHCriteria nHCriteriaspiritualrunes = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", spiritualrunesObj.RoleID);
            Utility.Debug.LogInfo("得到的制符配方");
            var spiritualrunestemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<SpiritualRunes>(nHCriteriaspiritualrunes);
            if (spiritualrunestemp != null)
            {
                if (!string.IsNullOrEmpty(spiritualrunestemp.Recipe_Array))
                {
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.JobSpiritualRunes, Utility.Json.ToJson(spiritualrunestemp));

                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                    //AscensionServer._Log.Info("得到的锻造配方"+ Utility.Json.ToJson(Frogetemp));
                }
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                SubDict.Add((byte)ParameterCode.JobSpiritualRunes, Utility.Json.ToJson(new List<string>()));
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaspiritualrunes);
        }
    }
}
