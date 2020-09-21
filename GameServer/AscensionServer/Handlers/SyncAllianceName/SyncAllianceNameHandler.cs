using AscensionProtocol;
using Photon.SocketServer;
using System;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using RedisDotNet;
using StackExchange.Redis;namespace AscensionServer
{
   public class SyncAllianceNameHandler:Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncAllianceName;
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
            var allianceNameTemp = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteriaAlliance);
            var allianceStatusTemp= AlliancelogicManager.Instance.GetNHCriteria<AllianceStatus>("ID", allianceStatusObj.ID);
            var roleAssetsTemp = AlliancelogicManager.Instance.GetNHCriteria<RoleAssets>("RoleID", roleAssetsObj.RoleID);
            OpResponse.OperationCode = operationRequest.OperationCode;
            if (roleAssetsTemp != null)
            {
                if (roleAssetsTemp.SpiritStonesLow >= roleAssetsObj.SpiritStonesLow)
                {

                    if (allianceStatusTemp != null && allianceNameTemp == null)
                    {
                        allianceStatusTemp.AllianceName = allianceStatusObj.AllianceName;
                        roleAssetsTemp.SpiritStonesLow -= roleAssetsObj.SpiritStonesLow;
                        await NHibernateQuerier.UpdateAsync(allianceStatusTemp);
                        await NHibernateQuerier.UpdateAsync(roleAssetsTemp);

                        await RedisHelper.Hash.HashSetAsync<RoleAssets>("RoleAssets", roleAssetsObj.RoleID.ToString(), new RoleAssets() { RoleID = roleAssetsObj.RoleID, SpiritStonesLow = roleAssetsTemp.SpiritStonesLow, XianYu = roleAssetsTemp.XianYu });
                        ResponseData.Add((byte)ParameterCode.AllianceName, Utility.Json.ToJson(allianceStatusTemp));
                        OpResponse.ReturnCode = (short)ReturnCode.Success;//返回成功
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
