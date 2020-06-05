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
            string roleJsonTemp = Convert.ToString(Utility.GetValue(dict,(byte)ObjectParameterCode.GongFa));

            GongFa roleGongFaTmp = Utility.ToObject<GongFa>(roleJsonTemp);
            NHCriteria nHCriteriaGongFa = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("GongFaID", roleGongFaTmp.GongFaID);
            var obj = Singleton<NHManager>.Instance.CriteriaGet<GongFa>(nHCriteriaGongFa);
            AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>角色ID>>>>>>" + roleJsonTemp + 
                ">>>>>>>>>>>>>>>");
            NHCriteria nHCriteriaID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", roleGongFaTmp.ID);

            Utility.Assert.NotNull(obj, () =>
            {
                RoleGongFa roleGongFa = new RoleGongFa() ;
                NHCriteria rolegongfaobj = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roleGongFa.RoleID);
                var result = Singleton<NHManager>.Instance.CriteriaGet<RoleGongFa>(rolegongfaobj);
                Utility.Assert.IsNull(result, () =>
                 {
                     result = Singleton<NHManager>.Instance.Add(new RoleGongFa() { GongFaIDArray = roleGongFaTmp.ID.ToString() });
                     AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>角色ID>>>>>>" + roleGongFaTmp.ID +
                ">>>>>>>>>>>>>>>");
                 },() => { Singleton<NHManager>.Instance.Update(result);AscensionServer._Log.Info("AddGongFaSubHandler \n成功"); });
            });
            //peer.SendOperationResponse(Owner.OpResponse,sendParameters);
            //Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaGongFa);
        }
        
    }
}
