using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class UpdateGongFaSubHandler : SyncGongFaSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            //base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var receivedRoleData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role));
            var receivedData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.GongFa));
            AscensionServer._Log.Info(">>>>>>>>>>>>接收功法数据：" + receivedRoleData + ">>>>>>>>>>>>>>>>>>>>>>");
            AscensionServer._Log.Info(">>>>>>>>>>>>接收功法数据：" + receivedData + ">>>>>>>>>>>>>>>>>>>>>>");
            var receivedRoleObj = Utility.Json.ToObject<RoleGongFa>(receivedRoleData);
            var receivedObj = Utility.Json.ToObject<GongFa>(receivedData);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", receivedRoleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleGongFa>(nHCriteriaRoleID);
            int intInfoObj = 0;
            int intLevel = 0; 
            if (exist)
            {
                RoleGongFa GongfaInfo = Singleton<NHManager>.Instance.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
                NHCriteria nHCriteriaGongFaID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", receivedObj.ID);
                bool existGongFa = Singleton<NHManager>.Instance.Verify<GongFa>(nHCriteriaGongFaID);
                if (existGongFa)
                {
                    GongFa GongfaInfoExp = Singleton<NHManager>.Instance.CriteriaSelect<GongFa>(nHCriteriaGongFaID);
                    foreach (var item in Utility.Json.ToObject<List<string>>(GongfaInfo.GongFaIDArray))
                    {
                        if (int.Parse(item) == receivedObj.ID)
                        {
                            if (receivedObj.GongFaLevel != 0)
                            {
                                GongfaInfoExp.GongFaExp = 0;
                                intInfoObj = GongfaInfoExp.GongFaExp + receivedObj.GongFaExp;
                                intLevel = GongfaInfoExp.GongFaLevel + receivedObj.GongFaLevel;
                                Singleton<NHManager>.Instance.Update(new GongFa() { ID = GongfaInfoExp.ID, GongFaID = GongfaInfoExp.GongFaID, GongFaLevel = (short)intLevel, GongFaSkillArray = GongfaInfoExp.GongFaSkillArray, GongFaExp = intInfoObj });
                            }
                            else
                            {
                                intInfoObj = GongfaInfoExp.GongFaExp + receivedObj.GongFaExp;
                                Singleton<NHManager>.Instance.Update(new GongFa() { ID = GongfaInfoExp.ID, GongFaID = GongfaInfoExp.GongFaID, GongFaLevel = GongfaInfoExp.GongFaLevel, GongFaSkillArray = GongfaInfoExp.GongFaSkillArray, GongFaExp = intInfoObj });

                            }
                        }
                    }
                }
               
                Owner.OpResponse.Parameters = Owner.ResponseData;
                Owner. OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                Owner. OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner. OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
    }
}
