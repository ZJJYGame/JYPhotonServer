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
        public async void RolePetSetBattle(RolePetDTO rolePetDTO, RolePet rolePet, NHCriteria nHCriteriaRolePet, Pet pet)
        {

            var role = NHibernateQuerier.CriteriaSelect<Role>(nHCriteriaRolePet);
            if (role.RoleLevel >= pet.PetLevel)
            {
                Utility.Debug.LogInfo("yzqData对角色宠物的操作" + rolePet.PetIsBattle + ".,.,.,.,.><<><><><><<<<>" + rolePetDTO.PetIsBattle);
                if (rolePet.PetIsBattle == rolePetDTO.PetIsBattle)
                {
                    rolePet.PetIsBattle = 0;
                    rolePetDTO.PetIsBattle = 0;
                }
                else
                    rolePet.PetIsBattle = rolePetDTO.PetIsBattle;
                await NHibernateQuerier.UpdateAsync<RolePet>(rolePet);
                rolePetDTO.PetIDDict = Utility.Json.ToObject<Dictionary<int, int>>(rolePet.PetIDDict);
                rolePetDTO.RoleID = rolePet.RoleID;
                await RedisHelper.Hash.HashSetAsync<RolePetDTO>(RedisKeyDefine._RolePetPerfix, rolePetDTO.RoleID.ToString(), rolePetDTO);
                S2CPetSetBattle(rolePet.RoleID, Utility.Json.ToJson(rolePetDTO), ReturnCode.Success);
            }
            else
            {
                S2CPetSetBattle(rolePet.RoleID, null, ReturnCode.Fail);
            }

        }
        /// <summary>
        /// 获得角色所有宠物
        /// </summary>
        /// <param name="rolePet"></param>
        public async void GetRoleAllPet(RolePet rolePet, RolePetDTO rolePetDTO)
        {
            var petDict = Utility.Json.ToObject<Dictionary<int, int>>(rolePet.PetIDDict);
            List<NHCriteria> nHCriteriaList = new List<NHCriteria>();
            Dictionary<int, PetDTO> allPetDict = new Dictionary<int, PetDTO>();
            rolePetDTO.PetIsBattle = rolePet.PetIsBattle;
            rolePetDTO.PetIDDict = Utility.Json.ToObject<Dictionary<int, int>>(rolePet.PetIDDict);

            foreach (var item in petDict)
            {
                if (RedisHelper.Hash.HashExistAsync(RedisKeyDefine._PetPerfix, item.Key.ToString()).Result)
                {
                    var petDTOTemp = await RedisHelper.Hash.HashGetAsync<PetDTO>(RedisKeyDefine._PetPerfix, item.Key.ToString());
                    allPetDict.Add(petDTOTemp.ID, petDTOTemp);
                }
                else
                {
                    NHCriteria nHCriteriaPet = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", item.Key);
                    nHCriteriaList.Add(nHCriteriaPet);
                    var petTemp = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriaPet);
                    var petDtoTemp = CosmosEntry.ReferencePoolManager.Spawn<PetDTO>();

                    petDtoTemp.ID = petTemp.ID;
                    petDtoTemp.PetExp = petTemp.PetExp;
                    petDtoTemp.DemonicSoul = Utility.Json.ToObject<Dictionary<int, List<int>>>(petTemp.DemonicSoul);
                    petDtoTemp.PetID = petTemp.PetID;
                    petDtoTemp.PetLevel = petTemp.PetLevel;
                    petDtoTemp.PetName = petTemp.PetName;
                    Utility.Debug.LogInfo("YZQ技能列表"+Utility.Json.ToJson(petTemp.PetSkillArray));
                    try
                    {
                        petDtoTemp.PetSkillArray = Utility.Json.ToObject<List<int>>(petTemp.PetSkillArray);
                    }
                    catch (Exception)
                    {
                        petDtoTemp.PetSkillArray=null;
                    }
                    allPetDict.Add(petDtoTemp.ID, petDtoTemp);
                }
            }
            Dictionary<string, string> rolepetsDict = new Dictionary<string, string>();
            rolepetsDict.Add("RolePet", Utility.Json.ToJson(rolePetDTO));
            rolepetsDict.Add("Pet", Utility.Json.ToJson(allPetDict));
            S2CRolePetOperateSuccess(rolePet.RoleID, Utility.Json.ToJson(rolepetsDict));
        }
        /// <summary>
        /// 移除角色宠物
        /// </summary>
        /// <param name="rolePet"></param>
        /// <param name="rolePetDTO"></param>
        public async void RemoveRolePet(RolePet rolePet, RolePetDTO rolePetDTO, Pet pet)
        {
            var rolePetDict = Utility.Json.ToObject<Dictionary<int, int>>(rolePet.PetIDDict);
            if (rolePetDict.ContainsKey(pet.ID))
            {
                #region Redis逻辑
                rolePetDict.Remove(pet.ID);
                rolePet.PetIDDict = Utility.Json.ToJson(rolePetDict);
                rolePetDTO.PetIDDict = rolePetDict;
                rolePetDTO.PetIsBattle = rolePet.PetIsBattle;
                var result = RedisHelper.KeyExistsAsync(RedisKeyDefine._PetPerfix).Result;
                if (result)
                {
                    await RedisHelper.Hash.HashDeleteAsync(RedisKeyDefine._PetPerfix, pet.ID.ToString());
                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RolePetPerfix, rolePetDTO.RoleID.ToString(), rolePetDTO);
                }
                #endregion
                await NHibernateQuerier.DeleteAsync(pet);
                Utility.Debug.LogInfo("yzqData已经放生的寵物" + Utility.Json.ToJson(pet));
                Utility.Debug.LogInfo("yzqData已经放生的寵物" + Utility.Json.ToJson(pet));
                await NHibernateQuerier.UpdateAsync(rolePet);
                S2CRemoveRolePet(rolePet.RoleID, Utility.Json.ToJson(rolePetDTO), ReturnCode.Success);
            } else
                S2CRemoveRolePet(rolePet.RoleID, Utility.Json.ToJson(rolePetDTO), ReturnCode.Fail);
        }
        /// <summary>
        /// 获得宠物所有属性
        /// </summary>
        public void GetPetAllCompeleteStatus(int petid, NHCriteria nHCriteria, int roleid, NHCriteria nHCriteriapet)
        {
            PetCompleteDTO petCompleteDTO = CosmosEntry.ReferencePoolManager.Spawn<PetCompleteDTO>();
            if (RedisHelper.KeyExistsAsync(RedisKeyDefine._PetPerfix + petid).Result)
            {
                var petObj = RedisHelper.Hash.HashGet<PetDTO>(RedisKeyDefine._PetPerfix, petid.ToString());
                petCompleteDTO.PetDTO = petObj;
            }
            else
            {
                Pet pet = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteria);
                var petObj = CosmosEntry.ReferencePoolManager.Spawn<PetDTO>();
                petObj.ID = pet.ID;
                petObj.PetExp = pet.PetExp;
                petObj.DemonicSoul = Utility.Json.ToObject<Dictionary<int, List<int>>>(pet.DemonicSoul);
                petObj.PetLevel = pet.PetLevel;
                petObj.PetName = pet.PetName;
                petObj.PetSkillArray = Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
                petCompleteDTO.PetDTO = petObj;
            }
            if (RedisHelper.KeyExistsAsync(RedisKeyDefine._PetAbilityPointPerfix + petid).Result)
            {
                var petAbilityPointObj = RedisHelper.Hash.HashGet<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, petid.ToString());
                petCompleteDTO.PetAbilityPointDTO = petAbilityPointObj;
            }
            else
            {
                PetAbilityPoint petAbilityPoint = NHibernateQuerier.CriteriaSelect<PetAbilityPoint>(nHCriteria);
                var petAbilityPointObj = CosmosEntry.ReferencePoolManager.Spawn<PetAbilityPointDTO>();
                petAbilityPointObj.ID = petAbilityPoint.ID;
                petAbilityPointObj.IsUnlockSlnThree = petAbilityPoint.IsUnlockSlnThree;
                petAbilityPointObj.SlnNow = petAbilityPoint.SlnNow;
                if (!string.IsNullOrEmpty(petAbilityPoint.AbilityPointSln))
                {
                    petAbilityPointObj.AbilityPointSln = new Dictionary<int, AbilityDTO>();
                    petAbilityPointObj.AbilityPointSln = Utility.Json.ToObject<Dictionary<int, AbilityDTO>>(petAbilityPoint.AbilityPointSln);
                }
                petCompleteDTO.PetAbilityPointDTO = petAbilityPointObj;

            }
            if (RedisHelper.KeyExistsAsync(RedisKeyDefine._PetAptitudePerfix + petid).Result)
            {
                var petAptitudeObj = RedisHelper.Hash.HashGet<PetAptitudeDTO>(RedisKeyDefine._PetAptitudePerfix, petid.ToString());
                petCompleteDTO.PetAptitudeDTO = petAptitudeObj;
            }
            else
            {
                PetAptitude petAptitude = NHibernateQuerier.CriteriaSelect<PetAptitude>(nHCriteriapet);
                var petAptitudeObj = CosmosEntry.ReferencePoolManager.Spawn<PetAptitudeDTO>();
                petAptitudeObj.PetID = petAptitude.PetID;
                petAptitudeObj.AttackphysicalAptitude = petAptitude.AttackphysicalAptitude;
                petAptitudeObj.AttackpowerAptitude = petAptitude.AttackpowerAptitude;
                petAptitudeObj.AttackspeedAptitude = petAptitude.AttackspeedAptitude;
                petAptitudeObj.DefendphysicalAptitude = petAptitude.DefendphysicalAptitude;
                petAptitudeObj.DefendpowerAptitude = petAptitude.DefendpowerAptitude;
                petAptitudeObj.HPAptitude = petAptitude.HPAptitude;
                petAptitudeObj.Petaptitudecol = petAptitude.Petaptitudecol;
                if (!string.IsNullOrEmpty(petAptitude.PetAptitudeDrug))
                {
                    petAptitudeObj.PetAptitudeDrug = new Dictionary<int, int>();
                    petAptitudeObj.PetAptitudeDrug = Utility.Json.ToObject<Dictionary<int, int>>(petAptitude.PetAptitudeDrug);
                }
                petAptitudeObj.SoulAptitude = petAptitude.SoulAptitude;
                petCompleteDTO.PetAptitudeDTO = petAptitudeObj;
            }

            if (RedisHelper.KeyExistsAsync(RedisKeyDefine._PetStatusPerfix + petid).Result)
            {
                var petStatusObj = RedisHelper.Hash.HashGet<PetStatusDTO>(RedisKeyDefine._PetStatusPerfix, petid.ToString());
                petCompleteDTO.PetStatusDTO = petStatusObj;
            }
            else
            {
                PetStatus petAptitude = NHibernateQuerier.CriteriaSelect<PetStatus>(nHCriteriapet);
                var petStatusObj = CosmosEntry.ReferencePoolManager.Spawn<PetStatusDTO>();
                var petAptitudeJson = Utility.Json.ToJson(petAptitude);
                petStatusObj = Utility.Json.ToObject<PetStatusDTO>(petAptitudeJson);
                petCompleteDTO.PetStatusDTO = petStatusObj;
            }

            //var petDrug = RedisHelper.String.StringGetAsync<PetDrugRefreshDTO>(RedisKeyDefine._PetDrugRefreshPostfix + petid);
            //if (petDrug!=null)
            //{
            //    S2CPetAllStatus(roleid, Utility.Json.ToJson(petDrug));
            //}
            petCompleteDTO.PetOrderType = PetCompleteDTO.PetOperationalOrder.PetGetStatus;
            S2CPetAllStatus(roleid, Utility.Json.ToJson(petCompleteDTO));
        }

        #region 洗练宠物
        public void ResetPetAllStatus(int petID, string petName, RolePet rolePet)
        {
            GameEntry. DataManager.TryGetValue<Dictionary<int, PetAptitudeData>>(out var petLevelDataDict);
            #region Pet
            var pet = CosmosEntry.ReferencePoolManager.Spawn<Pet>();
            pet.PetID = petID;
            pet.PetName = petName;
            pet.PetLevel = 1;
            pet.PetSkillArray = Utility.Json.ToJson(RestPetSkill(petLevelDataDict[petID].SkillArray));
            pet = NHibernateQuerier.Insert<Pet>(pet);
            var petObj = CosmosEntry.ReferencePoolManager.Spawn<PetDTO>();
            petObj.ID = pet.ID;
            petObj.PetExp = pet.PetExp;
            petObj.DemonicSoul = Utility.Json.ToObject<Dictionary<int, List<int>>>(pet.DemonicSoul);
            petObj.PetID = pet.PetID;
            petObj.PetLevel = pet.PetLevel;
            petObj.PetName = pet.PetName;
            petObj.PetSkillArray = Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
            RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, pet.ID.ToString(), petObj);
            #endregion
            #region PetAbilityPointDTO
            var petAbilityPointObj = CosmosEntry.ReferencePoolManager.Spawn<PetAbilityPointDTO>();
            var petAbilityPoint = CosmosEntry.ReferencePoolManager.Spawn<PetAbilityPoint>();
            petAbilityPoint.ID = pet.ID;
            petAbilityPoint.AbilityPointSln = Utility.Json.ToJson(petAbilityPointObj.AbilityPointSln);
            Utility.Debug.LogInfo("yzqData宠物加点方案" + Utility.Json.ToJson(petAbilityPointObj.AbilityPointSln));
            NHibernateQuerier.SaveOrUpdateAsync<PetAbilityPoint>(petAbilityPoint);
            petAbilityPointObj.ID = petAbilityPoint.ID;
            RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, pet.ID.ToString(), petAbilityPointObj);
            #endregion
            #region PetAptitudeDTO
            var petAptitude = CosmosEntry.ReferencePoolManager.Spawn<PetAptitude>();
            ResetPetAptitude(petID, out petAptitude);
            petAptitude.PetID = pet.ID;
            var petAptitudeObj = CosmosEntry.ReferencePoolManager.Spawn<PetAptitudeDTO>();
            petAptitudeObj.PetID = petAptitude.PetID;
            petAptitudeObj.AttackphysicalAptitude = petAptitude.AttackphysicalAptitude;
            petAptitudeObj.AttackpowerAptitude = petAptitude.AttackpowerAptitude;
            petAptitudeObj.AttackspeedAptitude = petAptitude.AttackspeedAptitude;
            petAptitudeObj.DefendphysicalAptitude = petAptitude.DefendphysicalAptitude;
            petAptitudeObj.DefendpowerAptitude = petAptitude.DefendpowerAptitude;
            petAptitudeObj.HPAptitude = petAptitude.HPAptitude;
            petAptitudeObj.Petaptitudecol = petAptitude.Petaptitudecol;
            petAptitudeObj.PetAptitudeDrug = Utility.Json.ToObject<Dictionary<int, int>>(petAptitude.PetAptitudeDrug);
            petAptitudeObj.SoulAptitude = petAptitude.SoulAptitude;

            RedisHelper.Hash.HashSetAsync<PetAptitudeDTO>(RedisKeyDefine._PetAptitudePerfix, pet.ID.ToString(), petAptitudeObj);
            NHibernateQuerier.SaveOrUpdateAsync<PetAptitude>(petAptitude);
            #endregion
            #region PetStatusDTO
            var petStatus = CosmosEntry.ReferencePoolManager.Spawn<PetStatus>();
            ResetPetStatus(pet, petAptitude, out petStatus);
            petStatus.PetID = pet.ID;
            RedisHelper.Hash.HashSetAsync<PetStatus>(RedisKeyDefine._PetStatusPerfix, pet.ID.ToString(), petStatus);

            NHibernateQuerier.SaveOrUpdateAsync<PetStatus>(petStatus);
            #endregion
            #region RolePetDTO
            var petDict = Utility.Json.ToObject<Dictionary<int, int>>(rolePet.PetIDDict);
            petDict.Add(pet.ID, pet.PetID);
            rolePet.PetIDDict = Utility.Json.ToJson(petDict);
            NHibernateQuerier.SaveOrUpdateAsync<RolePet>(rolePet);
            var RolepetObj = CosmosEntry.ReferencePoolManager.Spawn<RolePetDTO>();
            RolepetObj.RoleID = rolePet.RoleID;
            RolepetObj.PetIDDict = petDict;
            RolepetObj.PetIsBattle = rolePet.PetIsBattle;
            RedisHelper.Hash.HashSetAsync<RolePetDTO>(RedisKeyDefine._RolePetPerfix, rolePet.RoleID.ToString(), RolepetObj);
            #endregion

            S2CRoleAddPetSuccess(rolePet.RoleID);
            CosmosEntry.ReferencePoolManager.Despawns(pet, petStatus, petAptitude, petAbilityPoint, RolepetObj, petAptitudeObj, petAbilityPointObj);
        }
        #endregion

        #region 宠物加点
        /// <summary>
        /// 更新宠物加点方案
        /// </summary>
        public void UpdataPetAbilityPoint(PetAbilityPoint petAbilityPoint, PetCompleteDTO petCompleteDTO, NHCriteria nHCriteria)
        {
            switch (petCompleteDTO.PetAbilityPointDTO.AddPointType)
            {
                case PetAbilityPointDTO.AbilityPointType.Reset:
                    break;
                case PetAbilityPointDTO.AbilityPointType.Update:
                    Utility.Debug.LogInfo("yzqData更新宠物加点");
                    UpdatePointSln(petAbilityPoint, petCompleteDTO);
                    break;
                case PetAbilityPointDTO.AbilityPointType.Unlock:
                    UlockPointSln(petAbilityPoint, nHCriteria, petCompleteDTO);
                    break;
                case PetAbilityPointDTO.AbilityPointType.Rename:
                    RenamePointSln(petAbilityPoint, petCompleteDTO);
                    break;
                default:
                    break;
            }
        }

        public async void RenamePointSln(PetAbilityPoint petAbilityPoint, PetCompleteDTO petCompleteDTO)
        {
            var pointSlnDict = Utility.Json.ToObject<Dictionary<int, AbilityDTO>>(petAbilityPoint.AbilityPointSln);
            if (pointSlnDict.TryGetValue(petCompleteDTO.PetAbilityPointDTO.SlnNow, out var AbilityDTO))
            {
                AbilityDTO.SlnName = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].SlnName;
                pointSlnDict[petCompleteDTO.PetAbilityPointDTO.SlnNow] = AbilityDTO;

                petCompleteDTO.PetAbilityPointDTO.AbilityPointSln = pointSlnDict;
                petCompleteDTO.PetAbilityPointDTO.IsUnlockSlnThree = petAbilityPoint.IsUnlockSlnThree;
                petAbilityPoint.AbilityPointSln = Utility.Json.ToJson(pointSlnDict);
                await NHibernateQuerier.UpdateAsync<PetAbilityPoint>(petAbilityPoint);
                await RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, petCompleteDTO.RoleID.ToString(), petCompleteDTO.PetAbilityPointDTO);
                S2CPetAbilityPoint(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);
            }
        }
        public async void UpdatePointSln(PetAbilityPoint petAbilityPoint, PetCompleteDTO petCompleteDTO)
        {
            var pointSlnDict = Utility.Json.ToObject<Dictionary<int, AbilityDTO>>(petAbilityPoint.AbilityPointSln);
            if (pointSlnDict.TryGetValue(petCompleteDTO.PetAbilityPointDTO.SlnNow, out var AbilityDTO))
            {
                Utility.Debug.LogInfo("yzqData更新宠物加点" + Utility.Json.ToJson(pointSlnDict));
                Utility.Debug.LogInfo("yzqData更新宠物加点");
                AbilityDTO.Agility = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Agility;
                AbilityDTO.Power = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Power;
                AbilityDTO.Strength = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Strength;
                AbilityDTO.Soul = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Soul;
                AbilityDTO.Corporeity = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Corporeity;
                AbilityDTO.Stamina = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Stamina;
                AbilityDTO.SurplusAptitudePoint = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].SurplusAptitudePoint;
                AbilityDTO.IsSet = true;

                pointSlnDict[petCompleteDTO.PetAbilityPointDTO.SlnNow] = AbilityDTO;

                petCompleteDTO.PetAbilityPointDTO.AbilityPointSln = pointSlnDict;
                petAbilityPoint.AbilityPointSln = Utility.Json.ToJson(pointSlnDict);

                await NHibernateQuerier.UpdateAsync<PetAbilityPoint>(petAbilityPoint);
                await RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, petCompleteDTO.RoleID.ToString(), petCompleteDTO.PetAbilityPointDTO);
                S2CPetAbilityPoint(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);
            }
        }
        public async void UlockPointSln(PetAbilityPoint petAbilityPoint, NHCriteria nHCriteria, PetCompleteDTO petCompleteDTO)
        {
            var pointSlnDict = Utility.Json.ToObject<Dictionary<int, AbilityDTO>>(petAbilityPoint.AbilityPointSln);

            var roleassets = NHibernateQuerier.CriteriaSelectAsync<RoleAssets>(nHCriteria).Result;
            if (roleassets.SpiritStonesLow >= 0)
            {
                //if (pointSlnDict.TryGetValue(petCompleteDTO.PetAbilityPointDTO.SlnNow, out var AbilityDTO))
                //{                  
                //}
                pointSlnDict.Add(petCompleteDTO.PetAbilityPointDTO.SlnNow, new AbilityDTO() { SlnName = "方案三" });
                petAbilityPoint.IsUnlockSlnThree = true;
                petAbilityPoint.AbilityPointSln = Utility.Json.ToJson(pointSlnDict);
                roleassets.SpiritStonesLow -= 0;

                petCompleteDTO.PetAbilityPointDTO.IsUnlockSlnThree = true;
                await NHibernateQuerier.UpdateAsync<RoleAssets>(roleassets);
                await RedisHelper.Hash.HashSetAsync<RoleAssets>(RedisKeyDefine._RoleAssetsPerfix, roleassets.RoleID.ToString(), roleassets);

                petCompleteDTO.PetAbilityPointDTO.AbilityPointSln = pointSlnDict;

                petCompleteDTO.PetAbilityPointDTO.IsUnlockSlnThree = true;
                await NHibernateQuerier.UpdateAsync<PetAbilityPoint>(petAbilityPoint);
                await RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, petCompleteDTO.RoleID.ToString(), petCompleteDTO.PetAbilityPointDTO);

                S2CPetAbilityPoint(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);
            }
        }

        public void ResetAbilityPoint(PetAbilityPoint petAbilityPoint)
        {

        }

        #endregion

        #region 使用丹药
        /// <summary>
        /// 宠物培养丹药区分
        /// </summary>
        /// <param name="drugID"></param>
        /// <param name="nHCriteria"></param>
        /// <param name="pet"></param>
        public void PetCultivate(int drugID, NHCriteria nHCriteria, Pet pet, PetCompleteDTO petCompleteDTO)
        {
            var ringObj = CosmosEntry.ReferencePoolManager.Spawn<RingDTO>();
            ringObj.RingItems = new Dictionary<int, RingItemsDTO>();
            ringObj.RingItems.Add(drugID, new RingItemsDTO());
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            var nHCriteriaRingID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
            if (InventoryManager.VerifyIsExist(drugID, nHCriteriaRingID))
            {
                if (VerifyDrugEffect(drugID, pet, petCompleteDTO)) { }
                else
                    S2CPetCultivate(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Fail);
            }
            else
                S2CPetCultivate(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Fail);
        }
        /// <summary>
        /// 验证丹药作用
        /// </summary>
        public bool VerifyDrugEffect(int drugid, Pet pet, PetCompleteDTO petCompleteDTO)
        {
            GameEntry. DataManager.TryGetValue<Dictionary<int, DrugData>>(out var DrugDataDict);
            if (DrugDataDict.TryGetValue(InventoryManager.ConvertInt32(drugid), out var drugDatatemp))
            {
                if (drugDatatemp.Drug_Type == DrugType.PetExp)
                {
                    PetExpDrug(drugDatatemp, drugid, pet, petCompleteDTO);
                    return true;
                }
                else if (drugDatatemp.Drug_Type == DrugType.AddPetTalent)
                {
                    PetAtitudeDrug(drugDatatemp, drugid, pet, petCompleteDTO);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 增加经验
        /// </summary>
        /// <param name="drugData"></param>
        /// <param name="pet"></param>
        public async void PetExpDrug(DrugData drugData, int itemid, Pet pet, PetCompleteDTO petCompleteDTO)
        {
            GameEntry. DataManager.TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);

            var petDrugRefreshJson = RedisHelper.String.StringGetAsync(RedisKeyDefine._PetDrugRefreshPostfix + pet.ID.ToString()).Result;
            PetDrugRefreshDTO petDrugRefreshDTO = new PetDrugRefreshDTO();
            petDrugRefreshDTO.PetUsedDrug = new Dictionary<int, int>();
            if (petDrugRefreshJson != null)
            {
                petDrugRefreshDTO = Utility.Json.ToObject<PetDrugRefreshDTO>(petDrugRefreshJson);
                if (petDrugRefreshDTO.PetUsedDrug.TryGetValue(petCompleteDTO.UseItemID, out int count))
                {
                    if (count >= 100)
                    {
                        S2CPetCultivate(petCompleteDTO.RoleID, null, ReturnCode.Fail);
                        return;
                    }
                }
            }
            if (drugData.Need_Level_ID <= pet.PetLevel && drugData.Max_Level_ID >= pet.PetLevel)
            {
                pet.PetExp += drugData.Drug_Value;
                Utility.Debug.LogInfo("yzqData宠物即将使用加经验丹药" + pet.PetExp);
                if (petLevelDataDict[pet.PetLevel].ExpLevelUp <= pet.PetExp)
                {
                    if (petLevelDataDict[pet.PetLevel].IsFinalLevel)
                    {
                        pet.PetExp = petLevelDataDict[pet.PetLevel].ExpLevelUp;
                        Utility.Debug.LogInfo("yzqData使用加经验丹药即将进阶");
                    }
                    else
                    {
                        pet.PetExp = pet.PetExp - petLevelDataDict[pet.PetLevel].ExpLevelUp;
                        pet.PetLevel += 1;
                        //TODO刷新寵物所有屬性
                        Utility.Debug.LogInfo("yzqData使用加经验丹药" + pet.PetExp);
                    }
                }
                await NHibernateQuerier.UpdateAsync(pet);
                var petAbility = await RedisHelper.Hash.HashGetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, pet.ID.ToString());

                var petAptitude = await RedisHelper.Hash.HashGetAsync<PetAptitudeDTO>(RedisKeyDefine._PetAptitudePerfix, pet.ID.ToString());

                var petStatus = await RedisHelper.Hash.HashGetAsync<PetStatusDTO>(RedisKeyDefine._PetStatusPerfix, pet.ID.ToString());

                var petObj = await RedisHelper.Hash.HashGetAsync<PetDTO>(RedisKeyDefine._PetPerfix, pet.ID.ToString());
                petObj.PetLevel = pet.PetLevel;
                petObj.PetExp = pet.PetExp;
                await RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, pet.ID.ToString(), petObj);
                Utility.Debug.LogInfo("yzqData宠物加经验后1" + Utility.Json.ToJson(petStatus));
                PetStatus petStatuObj = new PetStatus();
                petStatuObj = Utility.Assembly.AssignSameFieldValue<PetStatusDTO, PetStatus>(petStatus, petStatuObj);
                Utility.Debug.LogInfo("yzqData宠物加经验后" + Utility.Json.ToJson(petStatuObj));
                PetAptitude petAptitudeObj = new PetAptitude();
                petAptitudeObj = AssignSameFieldValue(petAptitudeObj, petAptitude);


                petCompleteDTO.PetDTO = petObj;
                petCompleteDTO.PetAbilityPointDTO = petAbility;
                petCompleteDTO.PetAptitudeDTO = petAptitude;
                petCompleteDTO.PetStatusDTO = VerifyPetAllStatus(petAbility, petAptitudeObj, petStatuObj, petCompleteDTO, pet);
                petCompleteDTO.PetStatusDTO.PetID = pet.ID;
                await RedisHelper.Hash.HashSetAsync<PetStatusDTO>(RedisKeyDefine._PetStatusPerfix, pet.ID.ToString(), petCompleteDTO.PetStatusDTO);

                petStatuObj = Utility.Assembly.AssignSameFieldValue<PetStatusDTO, PetStatus>(petStatus, petStatuObj);

                await NHibernateQuerier.UpdateAsync(petStatuObj);

                S2CPetCultivate(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);
                if (petDrugRefreshDTO.PetUsedDrug.ContainsKey(drugData.Drug_ID))
                {
                    petDrugRefreshDTO.PetUsedDrug[drugData.Drug_ID]++;
                }
                else
                {
                    petDrugRefreshDTO.PetUsedDrug.Add(drugData.Drug_ID, 1);
                }
                petDrugRefreshDTO.PetID = pet.ID;
                await RedisHelper.Hash.HashSetAsync<PetDrugRefreshDTO>(RedisKeyDefine._PetDrugRefreshPostfix, pet.ID.ToString(), petDrugRefreshDTO);

                S2CPetDrugRefresh(petCompleteDTO.RoleID, Utility.Json.ToJson(petDrugRefreshDTO));

                InventoryManager.UpdateNewItem(petCompleteDTO.RoleID, itemid, 1);

            }

        }
        /// <summary>
        /// 增加资质
        /// </summary>
        public async void PetAtitudeDrug(DrugData drugData, int itemid, Pet pet, PetCompleteDTO petCompleteDTO)
        {
            var petAptitude = await RedisHelper.Hash.HashGetAsync<PetAptitudeDTO>(RedisKeyDefine._PetAptitudePerfix, pet.ID.ToString());
            GameEntry. DataManager.TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);
            if (petAptitude.PetAptitudeDrug.TryGetValue(drugData.Drug_ID, out var num))
            {
                if (num >= 10)
                {
                    S2CPetCultivate(petCompleteDTO.RoleID, null, ReturnCode.Fail);
                    return;
                }
            }

            if (drugData.Need_Level_ID <= pet.PetLevel && drugData.Max_Level_ID >= pet.PetLevel)
            {
                switch ((GrowUpDrugType)drugData.Drug_ID)
                {
                    case GrowUpDrugType.HPGrowUp:
                        petAptitude.HPAptitude += drugData.Drug_Value;
                        break;
                    case GrowUpDrugType.SoulGrowUp:
                        petAptitude.SoulAptitude += drugData.Drug_Value;
                        break;
                    case GrowUpDrugType.SpeedGrowUp:
                        petAptitude.AttackspeedAptitude += drugData.Drug_Value;
                        break;
                    case GrowUpDrugType.AttackDamageGrowUp:
                        petAptitude.AttackphysicalAptitude += drugData.Drug_Value;
                        break;
                    case GrowUpDrugType.ResistanceAttackGrowUp:
                        petAptitude.DefendphysicalAptitude += drugData.Drug_Value;
                        break;
                    case GrowUpDrugType.PowerGrowUp:
                        petAptitude.AttackpowerAptitude += drugData.Drug_Value;
                        break;
                    case GrowUpDrugType.ResistancePowerGrowUp:
                        petAptitude.DefendpowerAptitude += drugData.Drug_Value;
                        break;
                    case GrowUpDrugType.GrowUp:
                        petAptitude.Petaptitudecol += drugData.Drug_Value;
                        break;
                    default:
                        break;
                }
                if (petAptitude.PetAptitudeDrug.ContainsKey(drugData.Drug_ID))
                {
                    petAptitude.PetAptitudeDrug[drugData.Drug_ID]++;
                }
                else
                {
                    petAptitude.PetAptitudeDrug.Add(drugData.Drug_ID, 1);
                }
                await RedisHelper.Hash.HashSetAsync<PetAptitudeDTO>(RedisKeyDefine._PetAptitudePerfix, pet.ID.ToString(), petAptitude);

                var petAbility = await RedisHelper.Hash.HashGetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, pet.ID.ToString());

                var petStatus = await RedisHelper.Hash.HashGetAsync<PetStatusDTO>(RedisKeyDefine._PetStatusPerfix, pet.ID.ToString());

                var petObj = await RedisHelper.Hash.HashGetAsync<PetDTO>(RedisKeyDefine._PetPerfix, pet.ID.ToString());



                PetStatus petStatuObj = new PetStatus();
                petStatuObj = Utility.Assembly.AssignSameFieldValue<PetStatusDTO, PetStatus>(petStatus, petStatuObj);
                Utility.Debug.LogInfo("yzqData宠物加经验后" + Utility.Json.ToJson(petStatuObj));
                PetAptitude petAptitudeObj = new PetAptitude();
                petAptitudeObj = AssignSameFieldValue(petAptitudeObj, petAptitude);

                petCompleteDTO.PetDTO = petObj;
                petCompleteDTO.PetAbilityPointDTO = petAbility;
                petCompleteDTO.PetAptitudeDTO = petAptitude;
                petCompleteDTO.PetStatusDTO = VerifyPetAllStatus(petAbility, petAptitudeObj, petStatuObj, petCompleteDTO, pet);
                petCompleteDTO.PetStatusDTO.PetID = pet.ID;
                await RedisHelper.Hash.HashSetAsync<PetStatusDTO>(RedisKeyDefine._PetStatusPerfix, pet.ID.ToString(), petCompleteDTO.PetStatusDTO);

                await NHibernateQuerier.UpdateAsync(petStatuObj);
                await NHibernateQuerier.UpdateAsync(petAptitudeObj);
                S2CPetCultivate(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);

                InventoryManager.UpdateNewItem(petCompleteDTO.RoleID, itemid, 1);

            }
        }
        #endregion

        #region 宠物学习技能
        public async void PetStudySkill(int bookid, NHCriteria nHCriteria, Pet pet, PetCompleteDTO petCompleteDTO)
        {
            GameEntry. DataManager.TryGetValue<Dictionary<int, PetSkillBookData>>(out var petSkillBookDataDict);
            var ringObj = CosmosEntry.ReferencePoolManager.Spawn<RingDTO>();
            ringObj.RingItems = new Dictionary<int, RingItemsDTO>();
            ringObj.RingItems.Add(bookid, new RingItemsDTO());
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            var skillList = Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
            var nHCriteriaRingID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
            Utility.Debug.LogInfo("yzqData学习技能");
            if (InventoryManager.VerifyIsExist(bookid, nHCriteriaRingID))
            {
                if (!skillList.Contains(petSkillBookDataDict[bookid].PetSkillID))
                {
                    Utility.Debug.LogInfo("yzqData学习技能存在");
                    skillList = RandomSkillRemoveAdd(skillList, petSkillBookDataDict[bookid].PetSkillID);
                    pet.PetSkillArray = Utility.Json.ToJson(skillList);
                    await NHibernateQuerier.UpdateAsync<Pet>(pet);
                    petCompleteDTO.PetDTO.PetSkillArray = skillList;
                    petCompleteDTO.PetDTO.DemonicSoul = Utility.Json.ToObject<Dictionary<int,List<int>>>(pet.DemonicSoul);
                    S2CPetStudySkill(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);
                    await RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, petCompleteDTO.RoleID.ToString(), petCompleteDTO.PetDTO);
                    InventoryManager.RemoveCmd(bookid, ringObj, nHCriteriaRingID);
                }
                else
                {
                    S2CPetStudySkill(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO.PetDTO), ReturnCode.Fail);
                }
            }
            else
            {
                S2CPetStudySkill(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO.PetDTO), ReturnCode.Fail);
            }


        }
        #endregion

        #region 妖灵精魄
        public void DemonicSoulHandler(PetCompleteDTO petCompleteDTO, Pet pet, NHCriteria nHCriteriarole)
        {
            switch (petCompleteDTO.PetDTO.OperateType)
            {
                case PetDTO.PetOperateType.Rename:
                    PetRename(pet, petCompleteDTO);
                    break;
                case PetDTO.PetOperateType.Equip:
                    EquipDemonicSoul(petCompleteDTO.RoleID, nHCriteriarole, petCompleteDTO, pet);
                    break;
                case PetDTO.PetOperateType.Unequip:
                    UnEquipDemonicSoul(petCompleteDTO.UseItemID, pet, petCompleteDTO.RoleID, petCompleteDTO);
                    break;
                default:
                    break;
            }
        }
        public async void UnEquipDemonicSoul(int soulid, Pet pet, int roleid, PetCompleteDTO petCompleteDTO)
        {
            var soulDict = Utility.Json.ToObject<Dictionary<int, List<int>>>(pet.DemonicSoul);
            Utility.Debug.LogInfo("yzqData卸载的妖灵精魄ID" + soulid);

            InventoryManager.AddNewItem(roleid, soulid, 1);

            soulDict.Remove(soulid);

            pet.DemonicSoul = Utility.Json.ToJson(soulDict);
            await NHibernateQuerier.UpdateAsync(pet);

            petCompleteDTO.PetDTO.ID = pet.ID;
            petCompleteDTO.PetDTO.PetExp = pet.PetExp;
            petCompleteDTO.PetDTO.PetLevel = pet.PetLevel;
            petCompleteDTO.PetDTO.PetID = pet.PetID;
            petCompleteDTO.PetDTO.PetName = pet.PetName;
            petCompleteDTO.PetDTO.PetSkillArray = Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
            petCompleteDTO.PetDTO.DemonicSoul = soulDict;

            await RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, pet.ID.ToString(), petCompleteDTO.PetDTO);

            S2CPetRename(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);
        }

        public void AddDemonicSoul(int soulid, out List<int> getSkillList)
        {
            GameEntry. DataManager.TryGetValue<Dictionary<int, DemonicSoulData>>(out var DemonicSoulDict);

            GameEntry. DataManager.TryGetValue<Dictionary<int, DemonicSoulSkillPool>>(out var DemonicSoulSkillPoolDict);

            var skillPoolList = DemonicSoulDict[soulid].DemonicSoulSkill;
            Random random = new Random();
            int num = random.Next(0, skillPoolList.Count);
            var skillList = skillPoolList[num];
            getSkillList = new List<int>();
            for (int i = 0; i < skillList[0]; i++)
            {
                num = random.Next(0, DemonicSoulSkillPoolDict[0].PetSoulSkillPool.Count);
                if (!getSkillList.Contains(DemonicSoulSkillPoolDict[0].PetSoulSkillPool[num]))
                {
                    getSkillList.Add(DemonicSoulSkillPoolDict[0].PetSoulSkillPool[num]);
                }
            }
            for (int i = 0; i < skillList[1]; i++)
            {
                num = random.Next(0, DemonicSoulSkillPoolDict[1].PetSoulSkillPool.Count);
                if (!getSkillList.Contains(DemonicSoulSkillPoolDict[1].PetSoulSkillPool[num]))
                {
                    getSkillList.Add(DemonicSoulSkillPoolDict[1].PetSoulSkillPool[num]);
                }
            }
            for (int i = 0; i < skillList[2]; i++)
            {
                num = random.Next(0, DemonicSoulSkillPoolDict[2].PetSoulSkillPool.Count);
                if (!getSkillList.Contains(DemonicSoulSkillPoolDict[2].PetSoulSkillPool[num]))
                {
                    getSkillList.Add(DemonicSoulSkillPoolDict[2].PetSoulSkillPool[num]);
                }
            }
            //petCompleteDTO.PetDTO.DemonicSoul.Add();
        }

        public async void PetRename(Pet pet, PetCompleteDTO petCompleteDTO)
        {
            pet.PetName = petCompleteDTO.PetDTO.PetName;
            await NHibernateQuerier.UpdateAsync<Pet>(pet);
            petCompleteDTO.PetDTO.ID = pet.ID;
            petCompleteDTO.PetDTO.PetID = pet.PetID;
            petCompleteDTO.PetDTO.PetExp = pet.PetExp;
            petCompleteDTO.PetDTO.PetLevel = pet.PetLevel;
            petCompleteDTO.PetDTO.PetSkillArray = Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
            petCompleteDTO.PetDTO.DemonicSoul = Utility.Json.ToObject<Dictionary<int, List<int>>>(pet.DemonicSoul);
            await RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, petCompleteDTO.RoleID.ToString(), petCompleteDTO.PetDTO);
            Utility.Debug.LogInfo("yzqData宠物更改名字完成");
            S2CPetRename(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);
        }

        public async void EquipDemonicSoul(int roleid, NHCriteria nHCriteriarole, PetCompleteDTO petCompleteDTO, Pet pet)
        {
            var demonicSoul = NHibernateQuerier.CriteriaSelectAsync<DemonicSoul>(nHCriteriarole).Result;
            if (demonicSoul != null)
            {
                var soulDict = Utility.Json.ToObject<Dictionary<int, DemonicSoulEntity>>(demonicSoul.DemonicSouls);

                var result = soulDict.TryGetValue(petCompleteDTO.UseItemID, out var demonicSoulEntity);


                if (result)
                {
                    petCompleteDTO.PetDTO.DemonicSoul = new Dictionary<int, List<int>>();
                    Utility.Debug.LogInfo("yzqData" + Utility.Json.ToJson(pet));

                    petCompleteDTO.PetDTO.DemonicSoul.Add(demonicSoulEntity.UniqueID, demonicSoulEntity.Skills);
                    pet.DemonicSoul = Utility.Json.ToJson(petCompleteDTO.PetDTO.DemonicSoul);
                    await NHibernateQuerier.UpdateAsync(pet);

                    petCompleteDTO.PetDTO.ID = pet.ID;
                    petCompleteDTO.PetDTO.PetID = pet.PetID;
                    petCompleteDTO.PetDTO.PetExp = pet.PetExp;
                    petCompleteDTO.PetDTO.PetLevel = pet.PetLevel;
                    petCompleteDTO.PetDTO.PetName = pet.PetName;
                    petCompleteDTO.PetDTO.PetSkillArray = Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
                    await RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, pet.ID.ToString(), petCompleteDTO.PetDTO);

                    InventoryManager.Remove(roleid, petCompleteDTO.UseItemID);
                    S2CPetEquipDemonic(roleid, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);
                }
                else
                    S2CPetEquipDemonic(roleid, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Fail);
            }
            else
                S2CPetEquipDemonic(roleid, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Fail);

        }


        #endregion

        #region 宠物进阶

        public async void PetEvolution(PetCompleteDTO petCompleteDTO, Pet pet, int itemid, NHCriteria nHCriteriarole)
        {
            GameEntry. DataManager.TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);
            var demonicSoul = NHibernateQuerier.CriteriaSelectAsync<DemonicSoul>(nHCriteriarole).Result;
            var roleDemonic = Utility.Json.ToObject<Dictionary<int, DemonicSoulEntity>>(demonicSoul.DemonicSouls);
            if (roleDemonic.ContainsKey(itemid))
            {
                if (petLevelDataDict[pet.PetLevel].IsFinalLevel)
                {
                    var demonicDict = Utility.Json.ToObject<Dictionary<int, List<int>>>(pet.DemonicSoul);
                    var skillList = Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
                    if (demonicDict.TryGetValue(itemid, out var skills))
                    {
                        skillList.AddRange(skills);
                        demonicDict.Remove(itemid);
                        pet.PetLevel += 1;
                        pet.PetExp = 0;
                        pet.PetSkillArray = Utility.Json.ToJson(skillList);
                        pet.DemonicSoul = Utility.Json.ToJson(demonicDict);

                        await NHibernateQuerier.UpdateAsync(pet);
                        var petAbility = await RedisHelper.Hash.HashGetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, pet.ID.ToString());

                        var petAptitude = await RedisHelper.Hash.HashGetAsync<PetAptitudeDTO>(RedisKeyDefine._PetAptitudePerfix, pet.ID.ToString());

                        var petStatus = await RedisHelper.Hash.HashGetAsync<PetStatusDTO>(RedisKeyDefine._PetStatusPerfix, pet.ID.ToString());

                        var petObj = await RedisHelper.Hash.HashGetAsync<PetDTO>(RedisKeyDefine._PetPerfix, pet.ID.ToString());
                        petObj.PetLevel = pet.PetLevel;
                        petObj.PetExp = pet.PetExp;
                        petObj.PetSkillArray = skillList;
                        petObj.DemonicSoul = demonicDict;
                        await RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, pet.ID.ToString(), petObj);
                        Utility.Debug.LogInfo("yzqData宠物加经验后1" + Utility.Json.ToJson(petStatus));
                        PetStatus petStatuObj = new PetStatus();
                        petStatuObj = Utility.Assembly.AssignSameFieldValue<PetStatusDTO, PetStatus>(petStatus, petStatuObj);
                        Utility.Debug.LogInfo("yzqData宠物加经验后" + Utility.Json.ToJson(petStatuObj));
                        PetAptitude petAptitudeObj = new PetAptitude();
                        petAptitudeObj = AssignSameFieldValue(petAptitudeObj, petAptitude);


                        petCompleteDTO.PetDTO = petObj;
                        petCompleteDTO.PetAbilityPointDTO = petAbility;
                        petCompleteDTO.PetAptitudeDTO = petAptitude;
                        petCompleteDTO.PetStatusDTO = VerifyPetAllStatus(petAbility, petAptitudeObj, petStatuObj, petCompleteDTO, pet);
                        petCompleteDTO.PetStatusDTO.PetID = pet.ID;
                        await RedisHelper.Hash.HashSetAsync<PetStatusDTO>(RedisKeyDefine._PetStatusPerfix, pet.ID.ToString(), petCompleteDTO.PetStatusDTO);

                        petStatuObj = Utility.Assembly.AssignSameFieldValue<PetStatusDTO, PetStatus>(petStatus, petStatuObj);

                        await NHibernateQuerier.UpdateAsync(petStatuObj);

                        S2CPetCultivate(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);

                        roleDemonic.Remove(itemid);
                        demonicSoul.DemonicSouls = Utility.Json.ToJson(roleDemonic);
                        await NHibernateQuerier.UpdateAsync(demonicSoul);
                        DemonicSoulDTO demonicSoulDTO = new DemonicSoulDTO();
                        demonicSoulDTO.RoleID = demonicSoul.RoleID;
                        demonicSoulDTO.DemonicSouls = roleDemonic;
                        demonicSoulDTO.OperateType = DemonicSoulOperateType.Get;
                        GameEntry. DemonicSoulManager.S2CDemonicalMessage(petCompleteDTO.RoleID, Utility.Json.ToJson(demonicSoulDTO), ReturnCode.Success);

                    }
                    else
                    {
                        S2CPetCultivate(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Fail);
                        return;
                    }
                }
            }
            else
            {
                S2CPetCultivate(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Fail);
                return;
            }
        }

        #endregion

        #region 宠物洗练
        public async void PetStatusRestoreDefault(int itemid,int roleid,PetCompleteDTO petCompleteDTO,Pet pet,NHCriteria nHCriteria)
        {
            GameEntry. DataManager.TryGetValue<Dictionary<int, PetAptitudeData>>(out var petLevelDataDict);
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            var nHCriteriaRingID = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
            if (!InventoryManager.VerifyIsExist(itemid, nHCriteriaRingID))
            {
                //发送失败
                return;
            }
            //TODO判断物品是否为金柳露
            var petObj = CosmosEntry.ReferencePoolManager.Spawn<PetDTO>();
            petObj.PetLevel = 1;
            petObj.PetExp = 0;
            petObj.ID = pet.ID;
            petObj.PetID = pet.PetID;
            petObj.PetName = pet.PetName;
            petObj.DemonicSoul = Utility.Json.ToObject<Dictionary<int, List<int>>>(pet.DemonicSoul);
            petObj.PetSkillArray = RestPetSkill(petLevelDataDict[pet.PetID].SkillArray);
            await RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, petObj.ID.ToString(), petObj);

            pet.PetLevel =1;
            pet.PetExp = 0;
            pet.PetSkillArray = Utility.Json.ToJson(petObj.PetSkillArray);
            await NHibernateQuerier.UpdateAsync(pet);

            var petAbitilyObj= CosmosEntry.ReferencePoolManager.Spawn<PetAbilityPointDTO>();
            petAbitilyObj.ID = pet.ID;
            await RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, petObj.ID.ToString(), petAbitilyObj);
            var petAbitily= CosmosEntry.ReferencePoolManager.Spawn<PetAbilityPoint>();
            petAbitily.ID = pet.ID;
            await NHibernateQuerier.UpdateAsync(petAbitily);

            var petAptitudeObj= CosmosEntry.ReferencePoolManager.Spawn<PetAptitudeDTO>();
            var petAptitude = CosmosEntry.ReferencePoolManager.Spawn<PetAptitude>();
            ResetPetAptitude(pet.PetID,out petAptitude);
            petAptitude.PetID = pet.ID;
            petAptitudeObj =AssignSameFieldValue(petAptitudeObj, petAptitude);
            petAptitudeObj.PetID = pet.ID;
            await RedisHelper.Hash.HashSetAsync<PetAptitudeDTO>(RedisKeyDefine._PetAptitudePerfix, petObj.ID.ToString(), petAptitudeObj);
            await NHibernateQuerier.UpdateAsync(petAptitude);

            var petstatusObj = CosmosEntry.ReferencePoolManager.Spawn<PetStatusDTO>();
            var petstatus = CosmosEntry.ReferencePoolManager.Spawn<PetStatus>();
            ResetPetStatus(pet, petAptitude,out petstatus);
            petstatus.PetID = pet.ID;
            petstatusObj = Utility.Assembly.AssignSameFieldValue<PetStatus, PetStatusDTO>(petstatus, petstatusObj);
            await RedisHelper.Hash.HashSetAsync<PetStatusDTO>(RedisKeyDefine._PetStatusPerfix, petObj.ID.ToString(), petstatusObj);
            await NHibernateQuerier.UpdateAsync(petstatus);


            petCompleteDTO.PetDTO = petObj;
            petCompleteDTO.PetAptitudeDTO = petAptitudeObj;
            petCompleteDTO.PetAbilityPointDTO = petAbitilyObj;
            petCompleteDTO.PetStatusDTO = petstatusObj;
            S2CPetCultivate(roleid,Utility.Json.ToJson(petCompleteDTO),ReturnCode.Success);
        }
        #endregion
    }
}


