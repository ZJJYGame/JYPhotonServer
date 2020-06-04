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
    public class AddGongFaSubHandler : SyncGongFaSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Add;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleJsonTemp = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.Account));
            GongFa roleGongFaTmp = Utility.ToObject<GongFa>(roleJsonTemp);
            NHCriteria nHCriteriaGongFa = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleGongFaTmp.GongFaID);
            var isExisted = Singleton<NHManager>.Instance.Verify<GongFa>(nHCriteriaGongFa);
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>角色ID" + roleGongFaTmp.GongFaID + 
                ">>>>>>>>>>>>>>>");
            if (!isExisted)
            {
                roleGongFaTmp = Singleton<NHManager>.Instance.Add(roleGongFaTmp);
                NHCriteria nHCriteriaID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID",roleGongFaTmp.ID);
                bool rolegongfaID = Singleton<NHManager>.Instance.Verify<RoleGongFa>(nHCriteriaGongFa);
                if (!rolegongfaID)
                {
                    var rolegongfa = new RoleGongFa() { GongFaIDArray= roleGongFaTmp.ID.ToString()};
                    Singleton<NHManager>.Instance.Add(rolegongfa);
                }
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaID);

            }else
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;

            peer.SendOperationResponse(Owner.OpResponse,sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaGongFa);
               }
        
    }
}
