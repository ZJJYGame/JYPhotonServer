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

namespace AscensionServer
{
    public class GetRoleAllianceSkillSubHandler : SyncRoleAllianceSkillSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleallianceskillJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAllianceSkill));

            var roleallianceskillObj = Utility.Json.ToObject<RoleAllianceSkillDTO>(roleallianceskillJson);
            NHCriteria nHCriteriroleallianceskill = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleallianceskillObj.RoleID);
            Utility.Debug.LogError("收到的获得技能仙盟数据" + roleallianceskillJson);

            var roleallianceskillTemp= NHibernateQuerier.CriteriaSelectAsync<RoleAllianceSkill>(nHCriteriroleallianceskill).Result;


            if (roleallianceskillTemp!=null)
            {
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.RoleAllianceSkill, Utility.Json.ToJson(roleallianceskillTemp));

                    Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriroleallianceskill);
        }
    }
}
