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
            var receivedRoleData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            var receivedData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.MiShu));
            AscensionServer._Log.Info(">>>>>>>>>>>>接收秘术数据：" + receivedRoleData + ">>>>>>>>>>>>>>>>>>>>>>");
            AscensionServer._Log.Info(">>>>>>>>>>>>接收秘术数据：" + receivedData + ">>>>>>>>>>>>>>>>>>>>>>");
            var receivedRoleObj = Utility.Json.ToObject<RoleMiShu>(receivedRoleData);
            var receivedObj = Utility.Json.ToObject<MiShu>(receivedData);
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", receivedRoleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleMiShu>(nHCriteriaRoleID);
            int intInfoObj = 0;
            int intLevel = 0;
            if (exist)
            {
                RoleMiShu MishuInfo = Singleton<NHManager>.Instance.CriteriaGet<RoleMiShu>(nHCriteriaRoleID);
                NHCriteria nHCriteriaMiShuID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", receivedObj.ID);
                bool existMiShu = Singleton<NHManager>.Instance.Verify<MiShu>(nHCriteriaMiShuID);
                if (existMiShu)
                {
                    MiShu MishuInfoExp = Singleton<NHManager>.Instance.CriteriaGet<MiShu>(nHCriteriaMiShuID);
                    foreach (var item in Utility.Json.ToObject<List<string>>(MishuInfo.MiShuIDArray))
                    {
                        if (int.Parse(item) == receivedObj.ID)
                        {
                            if (receivedObj.MiShuLevel != 0)
                            {
                                MishuInfoExp.MiShuExp = 0;
                                intInfoObj = MishuInfoExp.MiShuExp + receivedObj.MiShuExp;
                                intLevel = MishuInfoExp.MiShuLevel + receivedObj.MiShuLevel;
                                Singleton<NHManager>.Instance.Update(new MiShu() { ID = MishuInfoExp.ID, MiShuID = MishuInfoExp.MiShuID, MiShuLevel = (short)intLevel, MiShuSkillArry = MishuInfoExp.MiShuSkillArry, MiShuExp = intInfoObj });
                            }
                            else
                            {
                                intInfoObj = MishuInfoExp.MiShuExp + receivedObj.MiShuExp;
                                Singleton<NHManager>.Instance.Update(new MiShu() { ID = MishuInfoExp.ID, MiShuID = MishuInfoExp.MiShuID, MiShuLevel = MishuInfoExp.MiShuLevel, MiShuSkillArry = MishuInfoExp.MiShuSkillArry, MiShuExp = intInfoObj });

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
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }
    }
}

