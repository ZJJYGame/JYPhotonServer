﻿using System;
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
            var receivedObj = Utility.Json.ToObject<CultivationMethod>(receivedData);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", receivedRoleObj.RoleID);
            bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleGongFa>(nHCriteriaRoleID);
            int intInfoObj = 0;
            int intLevel = 0; 
            if (exist)
            {
                RoleGongFa GongfaInfo = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleGongFa>(nHCriteriaRoleID);
                NHCriteria nHCriteriaGongFaID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", receivedObj.ID);
                bool existGongFa = ConcurrentSingleton<NHManager>.Instance.Verify<CultivationMethod>(nHCriteriaGongFaID);
                if (existGongFa)
                {
                    CultivationMethod GongfaInfoExp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<CultivationMethod>(nHCriteriaGongFaID);
                   var dicGongFaId =   Utility.Json.ToObject<Dictionary<int,int>>(GongfaInfo.GongFaIDArray);

                    if (dicGongFaId.ContainsKey(receivedObj.ID))
                    {
                        if (receivedObj.CultivationMethodLevel != 0)
                        {
                            GongfaInfoExp.CultivationMethodExp = 0;
                            intInfoObj = GongfaInfoExp.CultivationMethodExp + receivedObj.CultivationMethodExp;
                            intLevel = GongfaInfoExp.CultivationMethodLevel + receivedObj.CultivationMethodLevel;
                            ConcurrentSingleton<NHManager>.Instance.Update(new CultivationMethod() { ID = GongfaInfoExp.ID, CultivationMethodID = GongfaInfoExp.CultivationMethodID, CultivationMethodLevel = (short)intLevel, CultivationMethodLevelSkillArray = GongfaInfoExp.CultivationMethodLevelSkillArray, CultivationMethodExp = intInfoObj });
                        }
                        else
                        {
                            intInfoObj = GongfaInfoExp.CultivationMethodExp + receivedObj.CultivationMethodExp;
                            ConcurrentSingleton<NHManager>.Instance.Update(new CultivationMethod() { ID = GongfaInfoExp.ID, CultivationMethodID = GongfaInfoExp.CultivationMethodID, CultivationMethodLevel = GongfaInfoExp.CultivationMethodLevel, CultivationMethodLevelSkillArray = GongfaInfoExp.CultivationMethodLevelSkillArray, CultivationMethodExp = intInfoObj });

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
            GameManager.ReferencePoolManager.Despawn(nHCriteriaRoleID);
        }
    }
}
