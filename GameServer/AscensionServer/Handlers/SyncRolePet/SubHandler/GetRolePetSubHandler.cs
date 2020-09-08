using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using RedisDotNet;
namespace AscensionServer
{
    public class GetRolePetSubHandler : SyncRolePetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Get;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {

            var dict = ParseSubDict(operationRequest);
            string rolepet = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RolePet));

            var rolepetObj = Utility.Json.ToObject<RolePet>(rolepet);
            NHCriteria nHCriteriaRolePet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);

            var rolepets = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RolePet>(nHCriteriaRolePet);
            List<Pet> petlist = new List<Pet>();
            Dictionary<string, string> DODict = new Dictionary<string, string>();
            if (RedisHelper.Hash.HashExistAsync("RolePet", rolepetObj.RoleID.ToString()).Result)
            {
                #region Redis模块
                RolePet rolePetTemp = RedisHelper.Hash.HashGetAsync<RolePet>("RolePet", rolepetObj.RoleID.ToString()).Result;
                Dictionary<int, int> petIDList;

                List<NHCriteria> nHCriteriasList = new List<NHCriteria>();
                if (!string.IsNullOrEmpty(rolePetTemp.PetIDDict))
                {
                    petIDList = new Dictionary<int, int>();
                    petIDList = Utility.Json.ToObject<Dictionary<int, int>>(rolePetTemp.PetIDDict);
                    foreach (var petid in petIDList)
                    {
                        NHCriteria nHCriteriapet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petid.Key);
                        Pet petObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriapet);
                        petlist.Add(petObj);
                        nHCriteriasList.Add(nHCriteriapet);
                    }
                }
                DODict.Add("Pet", Utility.Json.ToJson(petlist));        
                DODict.Add("RolePet", Utility.Json.ToJson(rolePetTemp));
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(DODict));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
                GameManager.ReferencePoolManager.Despawns(nHCriteriasList);
                #endregion
            }

            else
            {
                Utility.Debug.LogError("测试读取到了MySql的数据" + RedisHelper.Hash.HashExistAsync("RolePet", rolepetObj.RoleID.ToString()).Result);
                #region MySql逻辑
                if (rolepets != null)
                {
                    var rpetObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<RolePet>(nHCriteriaRolePet);
                    string RolePetList = rpetObj.PetIDDict;
                    Dictionary<int, int> petIDList;
                    //List<Pet> petlist = new List<Pet>();
                    List<NHCriteria> nHCriteriasList = new List<NHCriteria>();
                    if (!string.IsNullOrEmpty(RolePetList))
                    {
                        petIDList = new Dictionary<int, int>();
                        petIDList = Utility.Json.ToObject<Dictionary<int, int>>(RolePetList);
                        foreach (var petid in petIDList)
                        {
                            NHCriteria nHCriteriapet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petid.Key);
                            Pet petObj = ConcurrentSingleton<NHManager>.Instance.CriteriaSelect<Pet>(nHCriteriapet);
                            petlist.Add(petObj);
                            nHCriteriasList.Add(nHCriteriapet);
                        }
                    }
                    DODict.Add("Pet", Utility.Json.ToJson(petlist));
                    DODict.Add("RolePet", Utility.Json.ToJson(rpetObj));
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(DODict));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                    GameManager.ReferencePoolManager.Despawns(nHCriteriasList);
                }
                else
                {
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(new List<string>()));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
                #endregion
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRolePet);
        }
    }
}
