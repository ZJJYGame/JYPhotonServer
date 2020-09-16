using AscensionProtocol;
using Photon.SocketServer;
using System;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Model;

namespace AscensionServer
{
   public class SyncAllianceNameHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRoleAdventureSkill;
            base.OnInitialization();
        }

        public async override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResponseData.Clear();
            var allianceJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.AllianceName));
            var roleAssetsJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleAssets));

            var  allianceStatusObj = Utility.Json.ToObject<AllianceStatusDTO>(allianceJson);
            var roleAssetsObj = Utility.Json.ToObject<RoleAssetsDTO>(roleAssetsJson);

            NHCriteria nHCriteriaAlliance = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceName", allianceStatusObj.AllianceName);
            var allianceNameTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceStatus>(nHCriteriaAlliance);

            var allianceStatusTemp= AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", allianceStatusObj.ID);
            var roleAssetsTemp = AlliancelogicManager.Instance.GetNHCriteria<RoleAssets>("RoleID", roleAssetsObj.RoleID);

            if (roleAssetsTemp != null)
            {
                if (roleAssetsTemp.SpiritStonesLow <= roleAssetsObj.SpiritStonesLow)
                {
                    if (allianceStatusTemp != null && allianceNameTemp == null)
                    {
                        allianceStatusTemp.AllianceName = allianceStatusObj.AllianceName;
                        roleAssetsTemp.SpiritStonesLow -= roleAssetsObj.SpiritStonesLow;
                        await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceStatusTemp);
                        await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleAssetsTemp);

                        ResponseData.Add((byte)ParameterCode.AllianceName, Utility.Json.ToJson(allianceStatusTemp));
                        OpResponse.Parameters = ResponseData;
                    }
                    else
                        OpResponse.ReturnCode = (short)ReturnCode.Fail;
                }
                else
                    OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            else
                OpResponse.ReturnCode = (short)ReturnCode.Fail;

            peer.SendOperationResponse(OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaAlliance);
        }
    }
}
