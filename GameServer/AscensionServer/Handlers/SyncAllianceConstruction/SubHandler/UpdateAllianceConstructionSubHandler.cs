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

namespace AscensionServer.Handlers
{
    public class UpdateAllianceConstructionSubHandler : SyncAllianceConstructionSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }


        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string allianceConstructionJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceConstruction));

            Utility.Debug.LogError("仙盟升级数据接收成功为" + allianceConstructionJson);
            var allianceConstructionObj = Utility.Json.ToObject<AllianceConstructionDTO>(allianceConstructionJson);
            NHCriteria nHCriteriallianceConstruction = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", allianceConstructionObj.AllianceID);

            NHCriteria nHCriterialliance = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", allianceConstructionObj.AllianceID);

            var allianceConstructionTemp= ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<AllianceConstruction>(nHCriteriallianceConstruction).Result;
            var allianceStatusTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<AllianceStatus>(nHCriterialliance).Result;

            if (allianceConstructionTemp != null)
            {
                if (allianceConstructionObj.AllianceCave>0)
                {
                    if (allianceConstructionTemp .AllianceAssets> allianceConstructionObj.AllianceAssets)
                    {
                        allianceConstructionTemp.AllianceCave += allianceConstructionObj.AllianceCave;
                        allianceConstructionTemp.AllianceAssets -= allianceConstructionObj.AllianceAssets;
                        await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceConstructionTemp);
                        SetResponseData(() =>
                        {
                            Utility.Debug.LogError("发送的升级仙盟数据为"+ Utility.Json.ToJson(allianceConstructionTemp));
                            SubDict.Add((byte)ParameterCode.AllianceConstruction, Utility.Json.ToJson(allianceConstructionTemp));
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        });
                    }
                    else
                    {
                        SetResponseData(() =>
                        {
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                        });
                    }
                }
                if (allianceConstructionObj.AllianceAlchemyStorage > 0)
                {
                    if (allianceConstructionTemp.AllianceAssets > allianceConstructionObj.AllianceAssets)
                    {
                        allianceConstructionTemp.AllianceAlchemyStorage += allianceConstructionObj.AllianceAlchemyStorage;
                        allianceConstructionTemp.AllianceAssets -= allianceConstructionObj.AllianceAssets;
                        await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceConstructionTemp);
                        SetResponseData(() =>
                        {
                            Utility.Debug.LogError("发送的升级仙盟数据为" + Utility.Json.ToJson(allianceConstructionTemp));
                            SubDict.Add((byte)ParameterCode.AllianceConstruction, Utility.Json.ToJson(allianceConstructionTemp));
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        });
                    }
                    else
                    {
                        SetResponseData(() =>
                        {
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                        });
                    }
                }
                if (allianceConstructionObj.AllianceScripturesPlatform > 0)
                {
                    if (allianceConstructionTemp.AllianceAssets > allianceConstructionObj.AllianceAssets)
                    {
                        allianceConstructionTemp.AllianceScripturesPlatform += allianceConstructionObj.AllianceScripturesPlatform;
                        allianceConstructionTemp.AllianceAssets -= allianceConstructionObj.AllianceAssets;
                        await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceConstructionTemp);
                        SetResponseData(() =>
                        {
                            Utility.Debug.LogError("发送的升级仙盟数据为" + Utility.Json.ToJson(allianceConstructionTemp));
                            SubDict.Add((byte)ParameterCode.AllianceConstruction, Utility.Json.ToJson(allianceConstructionTemp));
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        });
                    }
                    else
                    {
                        SetResponseData(() =>
                        {
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                        });
                    }
                }
                if (allianceConstructionObj.AllianceChamber > 0)
                {
                    if (allianceConstructionTemp.AllianceChamber == allianceConstructionTemp.AllianceCave&& allianceConstructionTemp.AllianceChamber == allianceConstructionTemp.AllianceAlchemyStorage&& allianceConstructionTemp.AllianceChamber == allianceConstructionTemp.AllianceScripturesPlatform&& allianceConstructionTemp.AllianceAssets > allianceConstructionObj.AllianceAssets)
                    {
                        allianceConstructionTemp.AllianceChamber += allianceConstructionObj.AllianceChamber;
                        allianceConstructionTemp.AllianceAssets -= allianceConstructionObj.AllianceAssets;

                        await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceConstructionTemp);
                        allianceStatusTemp.AllianceLevel += 1;
                        await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceStatusTemp);
                        SetResponseData(() =>
                        {
                            Utility.Debug.LogError("发送的升级仙盟数据为" + Utility.Json.ToJson(allianceConstructionTemp));
                            SubDict.Add((byte)ParameterCode.AllianceConstruction, Utility.Json.ToJson(allianceConstructionTemp));
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        });
                    }
                    else
                    {
                        SetResponseData(() =>
                        {
                            Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                        });
                    }
                }
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriallianceConstruction, nHCriterialliance);

        }
    }
}
