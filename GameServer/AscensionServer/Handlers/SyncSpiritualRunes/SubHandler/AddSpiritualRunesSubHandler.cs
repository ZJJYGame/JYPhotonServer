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
    public class AddSpiritualRunesSubHandler : SyncSpiritualRuneSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string spiritualRuneJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobSpiritualRunes));
            var spiritualRuneObj = Utility.Json.ToObject<SpiritualRunesDTO>(spiritualRuneJson);
            NHCriteria nHCriteriaspiritualRune = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", spiritualRuneObj.RoleID);
            var spiritualRuneTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<SpiritualRunes>(nHCriteriaspiritualRune);
            HashSet<int> spiritualRuneHash = new HashSet<int>();
            if (spiritualRuneTemp != null)
            {
                if (string.IsNullOrEmpty(spiritualRuneTemp.Recipe_Array))
                {
                    spiritualRuneTemp.Recipe_Array = Utility.Json.ToJson(spiritualRuneObj.Recipe_Array);
                    ConcurrentSingleton<NHManager>.Instance.Update(spiritualRuneTemp);
                }
                else
                {
                    spiritualRuneHash = Utility.Json.ToObject<HashSet<int>>(spiritualRuneTemp.Recipe_Array);
                    spiritualRuneHash.Add(spiritualRuneObj.Recipe_Array.First());
                    spiritualRuneTemp.Recipe_Array = Utility.Json.ToJson(spiritualRuneHash);
                    ConcurrentSingleton<NHManager>.Instance.Update(spiritualRuneTemp);
                }
                SetResponseData(() =>
                {
                    spiritualRuneObj = new SpiritualRunesDTO() { RoleID = spiritualRuneTemp.RoleID, JobLevel = spiritualRuneTemp.JobLevel, JobLevelExp = spiritualRuneTemp.JobLevelExp, Recipe_Array = spiritualRuneHash };

                    SubDict.Add((byte)ParameterCode.JobSpiritualRunes, spiritualRuneObj);
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaspiritualRune);
        }
    }
}
