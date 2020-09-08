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
    public class UpdateRoleAllianceSkillSubHandler : SyncRoleAllianceSkillSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }


        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleallianceskillJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAllianceSkill));

            var roleallianceskillObj = Utility.Json.ToObject<RoleAllianceSkilltransferDTO>(roleallianceskillJson);
            NHCriteria nHCriteriroleallianceskill = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleallianceskillObj.RoleID);

            var roleallianceskillTemp= ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<RoleAllianceSkill>(nHCriteriroleallianceskill).Result;

            var rolealliancesTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<RoleAlliance>(nHCriteriroleallianceskill).Result;
            var roleAssetsTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<RoleAssets>(nHCriteriroleallianceskill).Result;

            if (roleallianceskillTemp!=null&& rolealliancesTemp != null && roleAssetsTemp != null)
            {
                if (roleAssetsTemp.SpiritStonesLow >= roleallianceskillObj.RoleAssets && rolealliancesTemp.Reputation >= roleallianceskillObj.Contribution)
                {
                    roleallianceskillTemp.SkillInsight += roleallianceskillObj.SkillInsight;
                    roleallianceskillTemp.SkillMeditation += roleallianceskillObj.SkillMeditation;
                    roleallianceskillTemp.SkillRapid += roleallianceskillObj.SkillRapid;
                    roleallianceskillTemp.SkillStrong += roleallianceskillObj.SkillStrong;
                    roleAssetsTemp.SpiritStonesLow -= roleallianceskillObj.RoleAssets;
                    rolealliancesTemp.Reputation -= roleallianceskillObj.Contribution;

                  await  ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleallianceskillTemp);
                  await  ConcurrentSingleton<NHManager>.Instance.UpdateAsync(rolealliancesTemp);
                   await ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleAssetsTemp);

                    await RedisHelper.Hash.HashSetAsync<RoleAssets>("RoleAssets", roleAssetsTemp.RoleID.ToString(), roleAssetsTemp);
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.RoleAllianceSkill, Utility.Json.ToJson(roleallianceskillTemp));
                        SubDict.Add((byte)ParameterCode.RoleAssets, Utility.Json.ToJson(roleAssetsTemp));
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
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriroleallianceskill);
        }
        }
    }
