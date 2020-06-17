using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    public class RemoveRolePetSubHandler : SyncRoleStatusSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string rpJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.RolePet));
            string rpsJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.PetStatus));
            string pJson = Convert.ToString(Utility.GetValue(dict, (byte)ObjectParameterCode.Pet));

            var rpObj = Utility.Json.ToObject<RolePet>(rpJson);
            var rpsObj = Utility.Json.ToObject<PetStatus>(rpsJson);
            var pObj = Utility.Json.ToObject<Pet>(pJson);

            NHCriteria nHCriteriarolepet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rpObj.RoleID);
            var rolepet = Singleton<NHManager>.Instance.CriteriaSelect<RolePet>(nHCriteriarolepet);
            Dictionary<int, int> rolepetList;
            if (rolepet!=null)
            {
                int petid;
                if (!string.IsNullOrEmpty(rolepet.PetIDDict))
                {
                    rolepetList = new Dictionary<int, int>();
                    rolepetList = Utility.Json.ToObject<Dictionary<int, int>>(rolepet.PetIDDict);
                    AscensionServer._Log.Info("删除宠物进来了》》》》》》》》》》》》》》》》》》》》》》");
                    if (rolepetList.TryGetValue(rpObj.RoleID,out petid))
                    {
                        NHCriteria nHCriteriapet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", petid);
                        var pet = Singleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriapet);
                        if (pet!=null)
                        {
                            Singleton<NHManager>.Instance.Delete<Pet>(pet);
                            AscensionServer._Log.Info("删除宠物成功》》》》》》》》》》》》》》》》》》》》》》");
                        }

                        NHCriteria nHCriteriapetstatus = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", petid);
                        var petstatus = Singleton<NHManager>.Instance.CriteriaSelect<PetStatus>(nHCriteriapetstatus);
                        if (petstatus!=null)
                        {
                            Singleton<NHManager>.Instance.Delete<PetStatus>(petstatus);
                            AscensionServer._Log.Info("删除宠物数据成功》》》》》》》》》》》》》》》》》》》》》》");
                        }
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                        Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriapet, nHCriteriapetstatus);
                    }
                }else
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;

                peer.SendOperationResponse(Owner.OpResponse, sendParameters);
                Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriarolepet);
            }
        }
    }
}
