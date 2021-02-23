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
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {

            var dict = operationRequest.Parameters;
            string rolepet = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RolePet));

            var rolepetObj = Utility.Json.ToObject<RolePetDTO>(rolepet);

            NHCriteria nHCriteriaRolePet = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);

            NHCriteria nHCriteriaPet = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", rolepetObj.PetIsBattle);

            NHCriteria nHCriteriaRomePet = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", rolepetObj.AddRemovePetID);

            var pet = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriaPet);

            var rolepets = NHibernateQuerier.CriteriaSelect<RolePet>(nHCriteriaRolePet);
            List<Pet> petlist = new List<Pet>();
            Dictionary<string, string> DODict = new Dictionary<string, string>();
            switch (rolepetObj.RolePetOrderType)
            {
                case RolePetDTO.RolePetOperationalOrder.Battle:
                   GameEntry. PetStatusManager.RolePetSetBattle(rolepetObj, rolepets,nHCriteriaRolePet, pet);
                    break;
                case RolePetDTO.RolePetOperationalOrder.GetAllPet:
                     GameEntry. PetStatusManager.GetRoleAllPet(rolepets, rolepetObj);
                    break;
                case RolePetDTO.RolePetOperationalOrder.RemovePet:
                    Utility.Debug.LogInfo("yzqData放生寵物"+ rolepetObj.AddRemovePetID);
                    var petRemove = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriaRomePet);
                     GameEntry. PetStatusManager.RemoveRolePet(rolepets, rolepetObj, petRemove);
                    break;
                case RolePetDTO.RolePetOperationalOrder.AddPet:
                     GameEntry. PetStatusManager.InitPet(rolepetObj.AddRemovePetID, rolepetObj.AddPetName,rolepets);
                    break;
                default:
                    break;
            }
            #region 待删
            //if (RedisHelper.Hash.HashExistAsync("RolePet", rolepetObj.RoleID.ToString()).Result)
            //{
            //    #region Redis模块
            //    RolePet rolePetTemp = RedisHelper.Hash.HashGetAsync<RolePet>("RolePet", rolepetObj.RoleID.ToString()).Result;
            //    Dictionary<int, int> petIDList;

            //    List<NHCriteria> nHCriteriasList = new List<NHCriteria>();
            //    if (!string.IsNullOrEmpty(rolePetTemp.PetIDDict))
            //    {
            //        petIDList = new Dictionary<int, int>();
            //        petIDList = Utility.Json.ToObject<Dictionary<int, int>>(rolePetTemp.PetIDDict);
            //        foreach (var petid in petIDList)
            //        {
            //            NHCriteria nHCriteriapet = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petid.Key);
            //            Pet petObj = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriapet);
            //            petlist.Add(petObj);
            //            nHCriteriasList.Add(nHCriteriapet);
            //        }
            //    }
            //    DODict.Add("Pet", Utility.Json.ToJson(petlist));
            //    DODict.Add("RolePet", Utility.Json.ToJson(rolePetTemp));
            //    SetResponseParamters(() =>
            //    {
            //        subResponseParameters.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(DODict));
            //        operationResponse.ReturnCode = (short)ReturnCode.Success;
            //    });
            //    CosmosEntry.ReferencePoolManager.Despawns(nHCriteriasList);
            //    #endregion
            //}
            //else
            //{
            //    Utility.Debug.LogError("测试读取到了MySql的数据" + RedisHelper.Hash.HashExistAsync("RolePet", rolepetObj.RoleID.ToString()).Result);
            #region MySql逻辑
            //    if (rolepets != null)
            //    {
            //        var rpetObj = NHibernateQuerier.CriteriaSelect<RolePet>(nHCriteriaRolePet);
            //        string RolePetList = rpetObj.PetIDDict;
            //        Dictionary<int, int> petIDList;
            //        //List<Pet> petlist = new List<Pet>();
            //        List<NHCriteria> nHCriteriasList = new List<NHCriteria>();
            //        if (!string.IsNullOrEmpty(RolePetList))
            //        {
            //            petIDList = new Dictionary<int, int>();
            //            petIDList = Utility.Json.ToObject<Dictionary<int, int>>(RolePetList);
            //            foreach (var petid in petIDList)
            //            {
            //                NHCriteria nHCriteriapet = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petid.Key);
            //                Pet petObj = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriapet);
            //                petlist.Add(petObj);
            //                nHCriteriasList.Add(nHCriteriapet);
            //            }
            //        }
            //        DODict.Add("Pet", Utility.Json.ToJson(petlist));
            //        DODict.Add("RolePet", Utility.Json.ToJson(rpetObj));
            //        SetResponseParamters(() =>
            //        {
            //            subResponseParameters.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(DODict));
            //            operationResponse.ReturnCode = (short)ReturnCode.Success;
            //        });
            //        CosmosEntry.ReferencePoolManager.Despawns(nHCriteriasList);
            //    }
            //    else
            //    {
            //        SetResponseParamters(() =>
            //        {
            //            subResponseParameters.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(new List<string>()));
            //            operationResponse.ReturnCode = (short)ReturnCode.Fail;
            //        });
            //    }
            #endregion
            //}
            #endregion
            CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaRolePet);
            return operationResponse;
        }
    }
}


