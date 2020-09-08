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
using RedisDotNet;
using StackExchange.Redis;

namespace AscensionServer
{
    public class VerifyImmortalsAllianceHandler : SyncImmortalsAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Verify;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string alliancestatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ImmortalsAlliance));
            var alliancestatusObj = Utility.Json.ToObject<AllianceStatusDTO>
                (alliancestatusJson);
            NHCriteria nHCriteriaAlliance = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", alliancestatusObj.ID);
            var allianceIDObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceStatus>(nHCriteriaAlliance);
            List<AllianceStatusDTO> allianceStatusDTOs = new List<AllianceStatusDTO>();

            if (allianceIDObj==null)
            {
                NHCriteria nHCriteriaAllianceName = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceName", alliancestatusObj.AllianceName);
                var allianceNameObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceStatus>(nHCriteriaAllianceName);
                if (allianceNameObj == null)
                {
                    SetResponseData(() =>
                    {
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
                else
                {
                    AllianceStatusDTO allianceStatusDTO = new AllianceStatusDTO() { ID = allianceNameObj.ID, AllianceLevel = allianceNameObj.AllianceLevel, AllianceMaster = allianceNameObj.AllianceMaster, AllianceName = allianceNameObj.AllianceName, AllianceNumberPeople = allianceNameObj.AllianceNumberPeople, AlliancePeopleMax = allianceNameObj.AlliancePeopleMax, Manifesto = allianceNameObj.Manifesto, Popularity = allianceNameObj.Popularity };
                    allianceStatusDTOs.Add(allianceStatusDTO);
                    SetResponseData(() =>
                    {
                        Utility.Debug.LogInfo("发送的所有仙盟列表" + Utility.Json.ToJson(allianceStatusDTOs));
                        SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(allianceStatusDTOs));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                    GameManager.ReferencePoolManager.Despawns(nHCriteriaAllianceName);
                }
            }
            else
            {
                AllianceStatusDTO allianceStatusDTO = new AllianceStatusDTO() { ID = allianceIDObj.ID, AllianceLevel = allianceIDObj.AllianceLevel, AllianceMaster = allianceIDObj.AllianceMaster, AllianceName = allianceIDObj.AllianceName, AllianceNumberPeople = allianceIDObj.AllianceNumberPeople, AlliancePeopleMax = allianceIDObj.AlliancePeopleMax, Manifesto = allianceIDObj.Manifesto, Popularity = allianceIDObj.Popularity };
                allianceStatusDTOs.Add(allianceStatusDTO);
                SetResponseData(() =>
                {
                    Utility.Debug.LogInfo("发送的所有仙盟列表" + Utility.Json.ToJson(allianceStatusDTOs));
                    SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(allianceStatusDTOs));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaAlliance);
        }
    }
}
