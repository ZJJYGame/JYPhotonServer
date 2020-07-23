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
    public class UpdateMiShuSubHandler : SyncMiShuSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var receivedRoleData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.Role));
            var receivedData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.MiShu));
            AscensionServer._Log.Info(">>>>>>>>>>>>接收秘术数据：" + receivedRoleData + ">>>>>>>>>>>>>>>>>>>>>>");
            AscensionServer._Log.Info(">>>>>>>>>>>>接收秘术数据：" + receivedData + ">>>>>>>>>>>>>>>>>>>>>>");
            var receivedRoleObj = Utility.Json.ToObject<RoleMiShu>(receivedRoleData);
            var receivedObj = Utility.Json.ToObject<MiShu>(receivedData);
            NHCriteria nHCriteriaRoleID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", receivedRoleObj.RoleID);
            bool exist = ConcurrentSingleton<NHManager>.Instance.Verify<RoleMiShu>(nHCriteriaRoleID);
            int intInfoObj = 0;
            int intLevel = 0;
            if (exist)
            {
                RoleMiShu MishuInfo = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RoleMiShu>(nHCriteriaRoleID);
                NHCriteria nHCriteriaMiShuID = ConcurrentSingleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", receivedObj.ID);
                bool existMiShu = ConcurrentSingleton<NHManager>.Instance.Verify<MiShu>(nHCriteriaMiShuID);
                if (existMiShu)
                {
                    MiShu MishuInfoExp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<MiShu>(nHCriteriaMiShuID);
                    foreach (var item in Utility.Json.ToObject<Dictionary<int,int>>(MishuInfo.MiShuIDArray))
                    {
                        if (item.Key == receivedObj.ID)
                        {
                            if (receivedObj.MiShuLevel != 0)
                            {
                                MishuInfoExp.MiShuExp = 0;
                                intInfoObj = MishuInfoExp.MiShuExp + receivedObj.MiShuExp;
                                intLevel = MishuInfoExp.MiShuLevel + receivedObj.MiShuLevel;
                                ConcurrentSingleton<NHManager>.Instance.Update(new MiShu() { ID = MishuInfoExp.ID, MiShuID = MishuInfoExp.MiShuID, MiShuLevel = (short)intLevel, MiShuSkillArry = MishuInfoExp.MiShuSkillArry, MiShuExp = intInfoObj });
                            }
                            else
                            {
                                intInfoObj = MishuInfoExp.MiShuExp + receivedObj.MiShuExp;
                                ConcurrentSingleton<NHManager>.Instance.Update(new MiShu() { ID = MishuInfoExp.ID, MiShuID = MishuInfoExp.MiShuID, MiShuLevel = MishuInfoExp.MiShuLevel, MiShuSkillArry = MishuInfoExp.MiShuSkillArry, MiShuExp = intInfoObj });

                            }
                        }
                    }
                }

                Owner.OpResponse.Parameters = Owner.ResponseData;
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
            }
            else
            {
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            ConcurrentSingleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
    }
}

