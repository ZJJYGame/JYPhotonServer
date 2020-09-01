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
    public class GetForgeSubHandler : SyncForgeSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string forgeJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobForge));
            var forgeObj = Utility.Json.ToObject<ForgeDTO>(forgeJson);
            NHCriteria nHCriteriaFroge = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", forgeObj.RoleID);
            //AscensionServer._Log.Info("得到的锻造配方" );
            var Frogetemp= ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Forge>(nHCriteriaFroge);
            if (Frogetemp != null)
            {
                if (!string.IsNullOrEmpty(Frogetemp.Recipe_Array))
                {
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.JobForge, Utility.Json.ToJson(Frogetemp));

                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                    //AscensionServer._Log.Info("得到的锻造配方"+ Utility.Json.ToJson(Frogetemp));
                }
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                SubDict.Add((byte)ParameterCode.JobForge, Utility.Json.ToJson(new List<string>()));
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaFroge);
        }
    }
}
