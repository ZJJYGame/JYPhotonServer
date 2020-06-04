using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
namespace AscensionServer
{
    public class UpdateGongFaSubHandler : SyncGongFaSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            var receivedRoleData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            var receivedData = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.GongFaExp));
            AscensionServer._Log.Info(">>>>>>>>>>>>接收功法数据：" + receivedData + ">>>>>>>>>>>>>>>>>>>>>>");
            var receivedRoleObj = Utility.ToObject<RoleGongFa>(receivedRoleData);
            var receivedObj = Utility.ToObject<GongFa>(receivedData);

            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", receivedRoleObj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<RoleGongFa>(nHCriteriaRoleID);
            int intInfoObj = 0;
            if (exist)
            {
                RoleGongFa GongfaInfo = Singleton<NHManager>.Instance.CriteriaGet<RoleGongFa>(nHCriteriaRoleID);

                NHCriteria nHCriteriaGongFaID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", receivedObj.ID);
                bool existGongFa = Singleton<NHManager>.Instance.Verify<GongFa>(nHCriteriaGongFaID);

                if (existGongFa)
                {
                    GongFa GongfaInfoExp = Singleton<NHManager>.Instance.CriteriaGet<GongFa>(nHCriteriaGongFaID);
                    foreach (var item in Utility.ToObject<List<string>>(GongfaInfo.GongFaIDArray))
                    {
                        if (int.Parse(item) == receivedObj.ID)
                        {
                            intInfoObj = GongfaInfoExp.GongFaExp + receivedObj.GongFaExp;
                            Singleton<NHManager>.Instance.Update(new GongFa() { GongFaExp = intInfoObj });
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
