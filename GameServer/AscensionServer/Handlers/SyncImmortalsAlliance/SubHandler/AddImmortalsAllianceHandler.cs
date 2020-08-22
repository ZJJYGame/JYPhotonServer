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
    public class AddImmortalsAllianceHandler : SyncImmortalsAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string alliancestatusJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAlliance));
            var alliancestatusObj = Utility.Json.ToObject<AllianceStatus>
                (alliancestatusJson);
            string roleAllianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ImmortalsAlliance));
            var roleAllianceObj = Utility.Json.ToObject<RoleAlliance> 
                (roleAllianceJson);

            AscensionServer._Log.Info("获得的发过来的仙盟数据" + alliancestatusJson+"及盟主信息"+ roleAllianceJson);
            NHCriteria nHCriteriaAllianceName = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("AllianceName", alliancestatusObj.AllianceName);
            var alliance = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<AllianceStatus>(nHCriteriaAllianceName);


            List<string> Alliancelist = new List<string>();
            NHCriteria nHCriteriaroleAlliance = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleAllianceObj.RoleID);
            var roleAllianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleAlliance>(nHCriteriaroleAlliance);

            if (alliance == null)
            {
                List<int> gangslist = new List<int>();
                NHCriteria nHCriteriaAllianceList = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", 1);
                var allianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Alliances>(nHCriteriaAllianceList);
                gangslist = Utility.Json.ToObject<List<int>>(allianceTemp.AllianceList);

                var allianceslIstObj = ConcurrentSingleton<NHManager>.Instance.Insert(alliancestatusObj);
                AscensionServer._Log.Info("添加的仙盟id为" + allianceslIstObj.ID);
                gangslist.Add(allianceslIstObj.ID);
                allianceTemp.AllianceList = Utility.Json.ToJson(gangslist);
                ConcurrentSingleton<NHManager>.Instance.Update(allianceTemp);

                Alliancelist.Add(Utility.Json.ToJson(allianceslIstObj));
                if (roleAllianceTemp != null)
                {
                    roleAllianceTemp.RoleID = roleAllianceObj.RoleID;
                    roleAllianceTemp.AllianceID = allianceslIstObj.ID;
                    roleAllianceTemp.AllianceJob = roleAllianceObj.AllianceJob;
                    roleAllianceTemp.Reputation = roleAllianceObj.Reputation;
                    ConcurrentSingleton<NHManager>.Instance.Update(roleAllianceTemp);
                }
                else
                {
                    roleAllianceTemp = new RoleAlliance()
                    {
                        RoleID = roleAllianceObj.RoleID,
                        AllianceID = allianceslIstObj.ID,
                        AllianceJob = roleAllianceObj.AllianceJob,
                        Reputation = roleAllianceObj.Reputation,
                    };
                    ConcurrentSingleton<NHManager>.Instance.Insert(roleAllianceTemp);
                }

                Alliancelist.Add(Utility.Json.ToJson(roleAllianceTemp));
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.ImmortalsAlliance, Utility.Json.ToJson(Alliancelist));
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
                peer.SendOperationResponse(Owner.OpResponse, sendParameters);

        }
    }
}
