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
            GongFa rolegongFatemp = Utility.ToObject<GongFa>(roleJsonTemp);
            NHCriteria nHCriteriaGongFa = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rolegongFatemp.GongFaID);
            var isExisted = Singleton<NHManager>.Instance.Verify<GongFa>(nHCriteriaGongFa);
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>角色ID" + rolegongFatemp.GongFaID + 
                ">>>>>>>>>>>>>>>");
            if (!isExisted)
            {
                rolegongFatemp = Singleton<NHManager>.Instance.Add(rolegongFatemp);
                NHCriteria nHCriteriaID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID",rolegongFatemp.ID);
                bool rolegongfaID = Singleton<NHManager>.Instance.Verify<RoleGongFa>(nHCriteriaGongFa);
                if (!rolegongfaID)
                {
                    var rolegongfa = new RoleGongFa() { GongFaIDArray= rolegongFatemp.ID.ToString()};
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
