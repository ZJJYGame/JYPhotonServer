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
using RedisDotNet;
namespace AscensionServer
{
    public class RemoveRolePetSubHandler : SyncRolePetSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Remove;
        public async override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string rpJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RolePet));
            string pJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Pet));

            var rpObj = Utility.Json.ToObject<RolePet>(rpJson);
            var pObj = Utility.Json.ToObject<Pet>(pJson);


            Dictionary<int, int> rolepetList;
            NHCriteria nHCriteriarolepet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rpObj.RoleID);
            var rolepet = NHibernateQuerier.CriteriaSelect<RolePet>(nHCriteriarolepet);
            if (rolepet != null)
            {
                if (!string.IsNullOrEmpty(rolepet.PetIDDict))
                {
                    rolepetList = new Dictionary<int, int>();
                    rolepetList = Utility.Json.ToObject<Dictionary<int, int>>(rolepet.PetIDDict);
                    //AscensionServer._Log.Info("删除宠物进来了》》》》》》》》》》》》》》》》》》》》》》"+ rolepetList.Count);
                    if (rolepetList.ContainsKey(pObj.ID))
                    {
                        NHCriteria nHCriteriapet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", pObj.ID);
                        var pet = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriapet);

                        if (pet != null)
                        {
                            NHibernateQuerier.Delete<Pet>(pet);
                            //AscensionServer._Log.Info("删除宠物成功》》》》》》》》》》》》》》》》》》》》》》" + pet);
                        }

                        NHCriteria nHCriteriapetstatus = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("PetID", pObj.ID);
                        var petstatus = NHibernateQuerier.CriteriaSelect<PetStatus>(nHCriteriapetstatus);
                        if (petstatus != null)
                        {
                            NHibernateQuerier.Delete<PetStatus>(petstatus);
                            //AscensionServer._Log.Info("删除宠物数据成功》》》》》》》》》》》》》》》》》》》》》》");
                        }
                        rolepetList.Remove(pObj.ID);
                        NHibernateQuerier.Update<RolePet>(new RolePet() { RoleID = rpObj.RoleID, PetIDDict = Utility.Json.ToJson(rolepetList) });
                        Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
                        GameManager.ReferencePoolManager.Despawns(nHCriteriapet, nHCriteriapetstatus);
                    }
                    if (RedisHelper.Hash.HashExistAsync("RolePet", rpObj.RoleID.ToString()).Result)
                    {
                        #region Redis模块
                        RolePet rolePetTemp = RedisHelper.Hash.HashGetAsync<RolePet>("RolePet", rpObj.RoleID.ToString()).Result;
                        Utility.Debug.LogError("测试读取到了redis的数据" + RedisHelper.Hash.HashExistAsync("RolePet", rpObj.RoleID.ToString()).Result);

                        if (!string.IsNullOrEmpty(rolePetTemp.PetIDDict))
                        {
                            rolepetList = new Dictionary<int, int>();
                            rolepetList = Utility.Json.ToObject<Dictionary<int, int>>(rolePetTemp.PetIDDict);
                            if (rolepetList.ContainsKey(pObj.ID))
                            {

                              await   RedisHelper.Hash.HashDeleteAsync("PET", pObj.ID.ToString());
                              await   RedisHelper.Hash.HashDeleteAsync("PetStatus", pObj.ID.ToString());

                                rolepetList.Remove(pObj.ID);
                              await   RedisHelper.Hash.HashSetAsync<RolePet>("RolePet", rpObj.RoleID.ToString(), new RolePet() { RoleID = rpObj.RoleID, PetIDDict = Utility.Json.ToJson(rolepetList) });

                                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;

                            }
                        }
                        #endregion
                    }
                    else
                    {

                    }

                }
                else
                    Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriarolepet);

            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);

        }
    }
}
