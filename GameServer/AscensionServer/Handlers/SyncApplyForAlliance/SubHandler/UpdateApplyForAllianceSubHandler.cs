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
    public class UpdateApplyForAllianceSubHandler : SyncApplyForAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string allianceApplyJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.ApplyForAlliance));
            var allianceApplyObj = Utility.Json.ToObject<ApplyForAllianceDTO>(allianceApplyJson);
            string allianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.AllianceMember));
            var allianceObj = Utility.Json.ToObject<AllianceMember>(allianceJson);
            AscensionServer._Log.Info("收到的同意的仙盟成员数据" + allianceApplyJson+"lianmeng数据位 "+ allianceJson);

            NHCriteria nHCriteriallianceApplyFor = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", allianceApplyObj.RoleID);
            var allianceApplyForTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<RoleAlliance>(nHCriteriallianceApplyFor).Result;

            NHCriteria nHCriterialliancemember= ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("AllianceID", allianceObj.AllianceID);
            var alliancememberTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<AllianceMember>(nHCriterialliancemember).Result;



            List<int> applyList = new List<int>();
            List<int> memberList = new List<int>();
            if (allianceApplyForTemp!=null)
            {
                if (allianceApplyForTemp.AllianceID==0)
                {
                    if (alliancememberTemp!=null)
                    {
                        allianceApplyForTemp.AllianceID = alliancememberTemp.AllianceID;
                        allianceApplyForTemp.ApplyForAlliance = Utility.Json.ToJson(new List<int>());
                        applyList = Utility.Json.ToObject<List<int>>(alliancememberTemp.ApplyforMember);
                        applyList.Remove(allianceApplyObj.RoleID);
                        memberList = Utility.Json.ToObject<List<int>>(alliancememberTemp.Member);
                        memberList.Add(allianceApplyObj.RoleID);



                        AllianceMember allianceMember = new AllianceMember() { AllianceID= alliancememberTemp .AllianceID,ApplyforMember=Utility.Json.ToJson(applyList),Member= Utility.Json.ToJson(memberList) };
                        AscensionServer._Log.Info("修改后仙盟成员数据" + Utility.Json.ToJson(allianceMember));
                        ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceMember);
                        ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceApplyForTemp);

                    }
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.ApplyForAlliance, Utility.Json.ToJson(allianceApplyObj));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else
                {
                    applyList = Utility.Json.ToObject<List<int>>(alliancememberTemp.ApplyforMember);
                    applyList.Remove(allianceApplyObj.RoleID);

                    AllianceMember allianceMember = new AllianceMember() { AllianceID = alliancememberTemp.AllianceID, ApplyforMember = Utility.Json.ToJson(applyList), Member = alliancememberTemp.Member };
                    ConcurrentSingleton<NHManager>.Instance.UpdateAsync(allianceMember);
                    SetResponseData(() =>
                    {
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
                peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            }
        }
    }
}
