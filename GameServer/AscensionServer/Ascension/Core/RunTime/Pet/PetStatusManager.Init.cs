using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;
using RedisDotNet;
using Cosmos;
namespace AscensionServer
{
    public partial class PetStatusManager
    {
        /// <summary>
        /// 宠物设置出战
        /// </summary>
        /// <param name="rolePetDTO"></param>
        /// <param name="rolePet"></param>
        public async void RolePetSetBattle(RolePetDTO rolePetDTO,RolePet rolePet)
        {
            rolePet.PetIsBattle = rolePetDTO.PetIsBattle;
            await NHibernateQuerier.UpdateAsync<RolePet>(rolePet);
            rolePetDTO.PetIDDict = Utility.Json.ToObject<Dictionary<int, int>>(rolePet.PetIDDict);
            await RedisHelper.Hash.HashSetAsync<RolePetDTO>(RedisKeyDefine._RolePetPerfix + rolePetDTO.RoleID, rolePetDTO.RoleID.ToString(), rolePetDTO);
        }

        /// <summary>
        /// 获得角色所有宠物
        /// </summary>
        /// <param name="rolePet"></param>
        public async  void GetRoleAllPet(RolePet rolePet,RolePetDTO rolePetDTO)
        {
           var petDict= Utility.Json.ToObject<Dictionary<int, int>>(rolePet.PetIDDict);
            List<NHCriteria> nHCriteriaList = new List<NHCriteria>();
            Dictionary<int, PetDTO> allPetDict = new Dictionary<int, PetDTO>();
            List<PetDTO>  petList = new List<PetDTO>();
            rolePetDTO.PetIsBattle = rolePet.PetIsBattle;
            rolePetDTO.PetIDDict = Utility.Json.ToObject<Dictionary<int,int>>(rolePet.PetIDDict);
            Utility.Debug.LogInfo("yzqData获取所有宠物进来了" );
            foreach (var item in petDict)
            {
                if (RedisHelper.KeyExistsAsync(RedisKeyDefine._PetPerfix + item.Key).Result)
                {
                    var petDTOTemp = await RedisHelper.Hash.HashGetAsync<PetDTO>(RedisKeyDefine._PetPerfix + item.Key, item.Key.ToString());
                    allPetDict.Add(petDTOTemp.ID, petDTOTemp);
                }
                else
                {
                    NHCriteria nHCriteriaPet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                    nHCriteriaList.Add(nHCriteriaPet);
                    var petTemp = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriaPet);

                   var petDtoTemp= GameManager.ReferencePoolManager.Spawn<PetDTO>();

                    petDtoTemp.ID = petTemp.ID;
                    petDtoTemp.PetExp = petTemp.PetExp;
                    petDtoTemp.PetExtraSkill = Utility.Json.ToObject<Dictionary<int,int>>(petTemp.PetExtraSkill);
                    petDtoTemp.PetID = petTemp.PetID;
                    petDtoTemp.PetLevel = petTemp.PetLevel;
                    petDtoTemp.PetName = petTemp.PetName;
                    petDtoTemp.PetSkillArray = Utility.Json.ToObject<List<int>>(petTemp.PetSkillArray);
                    allPetDict.Add(petDtoTemp.ID, petDtoTemp);
                }
            }
            Dictionary<string, string> rolepetsDict = new Dictionary<string, string>();
            rolepetsDict.Add("RolePet", Utility.Json.ToJson(rolePetDTO));
            rolepetsDict.Add("Pet", Utility.Json.ToJson(petList));
            S2CRolePetOperateSuccess(rolePet.RoleID,Utility.Json.ToJson(rolepetsDict));
        }
        /// <summary>
        /// 移除角色宠物
        /// </summary>
        /// <param name="rolePet"></param>
        /// <param name="rolePetDTO"></param>
        public async static void RemoveRolePet(RolePet rolePet,RolePetDTO rolePetDTO )
        {
            var rolePetDict = Utility.Json.ToObject<Dictionary<int,int>>(rolePet.PetIDDict);

            if (rolePetDict.ContainsKey(rolePetDTO.RoleID))
            {
                rolePetDict.Remove(rolePetDTO.RoleID);
                var result = RedisHelper.KeyExistsAsync(RedisKeyDefine._PetPerfix + rolePetDTO.RoleID).Result;
                if (result)
                {
                  await RedisHelper.Hash.HashDeleteAsync(RedisKeyDefine._PetPerfix + rolePetDTO.RoleID);
                }
                await NHibernateQuerier.DeleteAsync<Pet>(default);

            }
        }



        /// <summary>
        /// 宠物培养丹药区分
        /// </summary>
        /// <param name="drugID"></param>
        /// <param name="nHCriteria"></param>
        /// <param name="pet"></param>
        public  void PetCultivate(int drugID,NHCriteria nHCriteria,Pet pet)
        {
            var ringObj = GameManager.ReferencePoolManager.Spawn<RingDTO>();
            ringObj.RingItems = new Dictionary<int, RingItemsDTO>();
            ringObj.RingItems.Add(drugID, new RingItemsDTO());
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            var nHCriteriaRingID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
            if (VerifyDrugEffect(drugID, out DrugData drugData))
            {
                if (InventoryManager.VerifyIsExist(drugID, nHCriteriaRingID))
                {
                }
            }
        }
        /// <summary>
        /// 验证丹药作用
        /// </summary>
        public  bool VerifyDrugEffect(int drugid,out DrugData drugData)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, DrugData>>(out var DrugDataDict);
            if (DrugDataDict.TryGetValue(drugid ,out var drugDatatemp))
            {
                if (drugDatatemp.Drug_Type == DrugType.PetExp && drugDatatemp.Drug_Type == DrugType.AddPetTalent)
                {
                    drugData = drugDatatemp;
                    return true;
                }
                else
                {
                    drugData = null;
                    return false;
                }
            }
            else
            {
                drugData = null;
                return false;
            }
        }
        /// <summary>
        /// 增加经验
        /// </summary>
        /// <param name="drugData"></param>
        /// <param name="pet"></param>
        public  void PetExpDrug(DrugData drugData,Pet pet)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);
            if (drugData.Need_Level_ID<=pet.PetLevel&& drugData.Max_Level_ID >=pet.PetLevel)
            {
                pet.PetExp += drugData.Drug_Value;
                if (petLevelDataDict[pet.PetLevel].ExpLevelUp<= pet.PetExp)
                {
                    if (petLevelDataDict[pet.PetLevel].IsFinalLevel)
                    {
                        pet.PetExp = petLevelDataDict[pet.PetLevel].ExpLevelUp;
                    }
                    else
                    {
                        pet.PetLevel += 1;
                        pet.PetExp = pet.PetExp-petLevelDataDict[pet.PetLevel].ExpLevelUp;
                    }
                }

            }
        }
        /// <summary>
        /// 增加资质
        /// </summary>
        public  void PetAtitudeDrug(DrugData drugData, Pet pet)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);
            //var drugDict = Utility.Json.ToObject<Dictionary<int,int>>();
            //if (drugData.Need_Level_ID <= pet.PetLevel && drugData.Max_Level_ID >= pet.PetLevel)
            //{

            //}
        }
    }
}
