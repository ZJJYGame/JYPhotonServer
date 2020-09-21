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
    public class AddApplyForAllianceSubHandler : SyncApplyForAllianceSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Add;
        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleAllianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ApplyForAlliance));
            var roleAllianceObj = Utility.Json.ToObject<RoleAllianceDTO>(roleAllianceJson);
            Utility.Debug.LogInfo("收到的加入仙盟的请求"+ roleAllianceJson);
            NHCriteria nHCriteriaroleAlliance = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleAllianceObj.RoleID);
            var roleAllianceTemp = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriaroleAlliance).Result;
            List<int> applyList = new List<int>();
            List<NHCriteria> NHCriterias = new List<NHCriteria>();
            NHCriterias.Add(nHCriteriaroleAlliance);
            if (!string.IsNullOrEmpty(roleAllianceTemp.ApplyForAlliance))
            {
                applyList = Utility.Json.ToObject<List<int>>(roleAllianceTemp.ApplyForAlliance);
                if (!roleAllianceTemp.ApplyForAlliance.Equals(roleAllianceObj.RoleID.ToString()))
                {
                    for (int i = 0; i < roleAllianceObj.ApplyForAlliance.Count; i++)
                    {
                        #region 监听解散的请求
                        //AllianceEvent.Instance.AddEventListener(roleAllianceObj.RoleID,AllianceEvent.Instance.RemoveAllianceStatus);
                        #endregion

                        applyList.Add(roleAllianceObj.ApplyForAlliance[i]);
                        roleAllianceTemp.ApplyForAlliance = Utility.Json.ToJson(applyList);
                     await   NHibernateQuerier.UpdateAsync(roleAllianceTemp);
                        NHCriteria nHCriteriaroleAllianceMember = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", roleAllianceObj.ApplyForAlliance[i]);
                        NHCriterias.Add(nHCriteriaroleAllianceMember);
                        var allianceMemberTemp = NHibernateQuerier.CriteriaSelectAsync<AllianceMember>(nHCriteriaroleAllianceMember).Result;
                        var applyer = Utility.Json.ToObject<List<int>>(allianceMemberTemp.ApplyforMember);
                        applyer.Add(roleAllianceObj.RoleID);
                        allianceMemberTemp.ApplyforMember = Utility.Json.ToJson(applyer);
                      await  NHibernateQuerier.UpdateAsync(allianceMemberTemp);
                    }


                    SetResponseData(() =>
                    {
                        roleAllianceObj.ApplyForAlliance = applyList;
                        SubDict.Add((byte)ParameterCode.ApplyForAlliance, Utility.Json.ToJson(roleAllianceObj));
                        Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
                    });
                }
            }
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters); 
            GameManager.ReferencePoolManager.Despawns(NHCriterias);
        }
    }
}
