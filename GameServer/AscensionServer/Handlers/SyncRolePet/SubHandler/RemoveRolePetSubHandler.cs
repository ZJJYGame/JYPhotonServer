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
    public class RemoveRolePetSubHandler : SyncRolePetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string rpJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RolePet));
            string pJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));

            var rpObj = Utility.Json.ToObject<RolePet>(rpJson);
            var pObj = Utility.Json.ToObject<Pet>(pJson);

            NHCriteria nHCriteriarolepet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", rpObj.RoleID);
            var rolepet = Singleton<NHManager>.Instance.CriteriaSelect<RolePet>(nHCriteriarolepet);
            Dictionary<int, int> rolepetList;
            if (rolepet!=null)
            {
                if (!string.IsNullOrEmpty(rolepet.PetIDDict))
                {
                    rolepetList = new Dictionary<int, int>();
                    rolepetList = Utility.Json.ToObject<Dictionary<int, int>>(rolepet.PetIDDict);
                    //AscensionServer._Log.Info("删除宠物进来了》》》》》》》》》》》》》》》》》》》》》》"+ rolepetList.Count);
                    if (rolepetList.ContainsKey(pObj.ID))
                    {
                        NHCriteria nHCriteriapet = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", pObj.ID);
                        var pet = Singleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriapet);

                        if (pet!=null)
                        {
                            Singleton<NHManager>.Instance.Delete<Pet>(pet);
                            //AscensionServer._Log.Info("删除宠物成功》》》》》》》》》》》》》》》》》》》》》》" + pet);
                        }

                        NHCriteria nHCriteriapetstatus = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("PetID", pObj.ID);
                        var petstatus = Singleton<NHManager>.Instance.CriteriaSelect<PetStatus>(nHCriteriapetstatus);
                        if (petstatus!=null)
                        {
                            Singleton<NHManager>.Instance.Delete<PetStatus>(petstatus);
                            //AscensionServer._Log.Info("删除宠物数据成功》》》》》》》》》》》》》》》》》》》》》》");
                        }
                        rolepetList.Remove(pObj.ID);
                        Singleton<NHManager>.Instance.Update<RolePet>(new RolePet() { RoleID= rpObj.RoleID, PetIDDict=Utility.Json.ToJson(rolepetList) });
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
