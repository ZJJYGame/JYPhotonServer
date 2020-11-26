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
        public async void RolePetSetBattle(RolePetDTO rolePetDTO,RolePet rolePet, NHCriteria nHCriteriaRolePet,Pet pet)
        {

            var role = NHibernateQuerier.CriteriaSelect<Role>(nHCriteriaRolePet);
            if (role.RoleLevel>= pet.PetLevel)
            {
                Utility.Debug.LogInfo("yzqData对角色宠物的操作" + rolePet.PetIsBattle +".,.,.,.,.><<><><><><<<<>"+ rolePetDTO.PetIsBattle);
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
                S2CPetSetBattle(rolePet.RoleID,null, ReturnCode.Fail);
            }

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
            rolePetDTO.PetIsBattle = rolePet.PetIsBattle;
            rolePetDTO.PetIDDict = Utility.Json.ToObject<Dictionary<int,int>>(rolePet.PetIDDict);

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
                    petDtoTemp.DemonicSoul = Utility.Json.ToObject<Dictionary<int,List<int>>>(petTemp.DemonicSoul);
                    petDtoTemp.PetID = petTemp.PetID;
                    petDtoTemp.PetLevel = petTemp.PetLevel;
                    petDtoTemp.PetName = petTemp.PetName;
                    petDtoTemp.PetSkillArray = Utility.Json.ToObject<List<int>>(petTemp.PetSkillArray);
                    allPetDict.Add(petDtoTemp.ID, petDtoTemp);
                }
            }
            Dictionary<string, string> rolepetsDict = new Dictionary<string, string>();
            rolepetsDict.Add("RolePet", Utility.Json.ToJson(rolePetDTO));
            rolepetsDict.Add("Pet", Utility.Json.ToJson(allPetDict));
            S2CRolePetOperateSuccess(rolePet.RoleID,Utility.Json.ToJson(rolepetsDict));
        }
        /// <summary>
        /// 移除角色宠物
        /// </summary>
        /// <param name="rolePet"></param>
        /// <param name="rolePetDTO"></param>
        public async void RemoveRolePet(RolePet rolePet,RolePetDTO rolePetDTO, Pet pet)
        {
            var rolePetDict = Utility.Json.ToObject<Dictionary<int,int>>(rolePet.PetIDDict);
            if (rolePetDict.ContainsKey(rolePetDTO.RoleID))
            {
                #region Redis逻辑
                rolePet.PetIDDict = Utility.Json.ToJson(rolePetDict);
                rolePetDict.Remove(rolePetDTO.RoleID);
                rolePetDTO.PetIDDict = rolePetDict;
                rolePetDTO.PetIsBattle = rolePet.PetIsBattle;
                var result = RedisHelper.KeyExistsAsync(RedisKeyDefine._PetPerfix + rolePetDTO.RoleID).Result;
                if (result)
                {
                    await RedisHelper.Hash.HashDeleteAsync(RedisKeyDefine._PetPerfix + rolePetDTO.RoleID);
                    await RedisHelper.Hash.HashSetAsync<RolePetDTO>(RedisKeyDefine._PetPerfix, rolePetDTO.RoleID.ToString(), rolePetDTO);
                }
                #endregion
                await NHibernateQuerier.DeleteAsync<Pet>(pet);
                await NHibernateQuerier.UpdateAsync<RolePet>(rolePet);
            }
            S2CRemoveRolePet(rolePet.RoleID, Utility.Json.ToJson(rolePetDTO),ReturnCode.Success);
        }
        /// <summary>
        /// 获得宠物所有属性
        /// </summary>
        public void GetPetAllCompeleteStatus(int petid, NHCriteria nHCriteria,int roleid, NHCriteria nHCriteriapet  )
        {
            PetCompleteDTO petCompleteDTO = GameManager.ReferencePoolManager.Spawn<PetCompleteDTO>();
            if (RedisHelper.KeyExistsAsync(RedisKeyDefine._PetPerfix+ petid).Result)
            {
                var petObj= RedisHelper.Hash.HashGet<PetDTO>(RedisKeyDefine._PetPerfix, petid.ToString());
                petCompleteDTO.PetDTO = petObj;
            }
            else
            {
                Pet pet= NHibernateQuerier.CriteriaSelect<Pet>(nHCriteria);
                var  petObj = GameManager.ReferencePoolManager.Spawn<PetDTO>();
                petObj.ID = pet.ID;
                petObj.PetExp = pet.PetExp;
                petObj.DemonicSoul = Utility.Json.ToObject<Dictionary<int,List<int>>>(pet.DemonicSoul);
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
                var petAbilityPointObj = GameManager.ReferencePoolManager.Spawn<PetAbilityPointDTO>();
                petAbilityPointObj.ID= petAbilityPoint.ID;
                petAbilityPointObj.IsUnlockSlnThree = petAbilityPoint.IsUnlockSlnThree;
                petAbilityPointObj.SlnNow = petAbilityPoint.SlnNow;
                if (!string.IsNullOrEmpty(petAbilityPoint.AbilityPointSln))
                {
                    petAbilityPointObj.AbilityPointSln = new Dictionary<int, PetAbilityDTO>();
                    petAbilityPointObj.AbilityPointSln = Utility.Json.ToObject<Dictionary<int, PetAbilityDTO>>(petAbilityPoint.AbilityPointSln);
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
                var petAptitudeObj = GameManager.ReferencePoolManager.Spawn<PetAptitudeDTO>();
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
                var petStatusObj = GameManager.ReferencePoolManager.Spawn<PetStatusDTO>();
                var petAptitudeJson = Utility.Json.ToJson(petAptitude);
                petStatusObj = Utility.Json.ToObject<PetStatusDTO>(petAptitudeJson);
                petCompleteDTO.PetStatusDTO = petStatusObj;
            }
            petCompleteDTO.PetOrderType = PetCompleteDTO.PetOperationalOrder.PetGetStatus;
            S2CPetAllStatus(roleid,Utility.Json.ToJson(petCompleteDTO));
        }
        #region 宠物加点
        /// <summary>
        /// 更新宠物加点方案
        /// </summary>
        public void UpdataPetAbilityPoint(PetAbilityPoint petAbilityPoint,PetCompleteDTO petCompleteDTO, NHCriteria nHCriteria)
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
            var pointSlnDict = Utility.Json.ToObject<Dictionary<int,PetAbilityDTO>>(petAbilityPoint.AbilityPointSln);
            if (pointSlnDict.TryGetValue(petCompleteDTO.PetAbilityPointDTO.SlnNow,out var  petAbilityDTO))
            {
                petAbilityDTO.SlnName = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].SlnName;
                pointSlnDict[petCompleteDTO.PetAbilityPointDTO.SlnNow] = petAbilityDTO;

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
            var pointSlnDict = Utility.Json.ToObject<Dictionary<int, PetAbilityDTO>>(petAbilityPoint.AbilityPointSln);
            if (pointSlnDict.TryGetValue(petCompleteDTO.PetAbilityPointDTO.SlnNow, out var petAbilityDTO))
            {
                Utility.Debug.LogInfo("yzqData更新宠物加点"+Utility.Json.ToJson(pointSlnDict));
                Utility.Debug.LogInfo("yzqData更新宠物加点");
                petAbilityDTO.Agility = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Agility;
                petAbilityDTO.Power = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Power;
                petAbilityDTO.Strength = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Strength;
                petAbilityDTO.Soul = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Soul;
                petAbilityDTO.Corporeity = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Corporeity;
                petAbilityDTO.Stamina = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].Stamina;
                petAbilityDTO.SurplusAptitudePoint = petCompleteDTO.PetAbilityPointDTO.AbilityPointSln[petCompleteDTO.PetAbilityPointDTO.SlnNow].SurplusAptitudePoint;
                petAbilityDTO.IsSet = true;

                pointSlnDict[petCompleteDTO.PetAbilityPointDTO.SlnNow] = petAbilityDTO;

                petCompleteDTO.PetAbilityPointDTO.AbilityPointSln = pointSlnDict;
                petAbilityPoint.AbilityPointSln = Utility.Json.ToJson(pointSlnDict);

                await NHibernateQuerier.UpdateAsync<PetAbilityPoint>(petAbilityPoint);
                await RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, petCompleteDTO.RoleID.ToString(), petCompleteDTO.PetAbilityPointDTO);
                S2CPetAbilityPoint(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO), ReturnCode.Success);
            }
        }
        public async void UlockPointSln(PetAbilityPoint petAbilityPoint, NHCriteria nHCriteria, PetCompleteDTO petCompleteDTO)
        {
            var pointSlnDict = Utility.Json.ToObject<Dictionary<int, PetAbilityDTO>>(petAbilityPoint.AbilityPointSln);

            var roleassets = NHibernateQuerier.CriteriaSelectAsync<RoleAssets>(nHCriteria).Result;
            if (roleassets.SpiritStonesLow >= 0)
            {
                //if (pointSlnDict.TryGetValue(petCompleteDTO.PetAbilityPointDTO.SlnNow, out var petAbilityDTO))
                //{                  
                //}
                pointSlnDict.Add(petCompleteDTO.PetAbilityPointDTO.SlnNow, new PetAbilityDTO() {SlnName="方案三" });
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

        public void ResetAbilityPoint( PetAbilityPoint petAbilityPoint)
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
        public void PetCultivate(int drugID, NHCriteria nHCriteria, Pet pet)
        {
            var ringObj = GameManager.ReferencePoolManager.Spawn<RingDTO>();
            ringObj.RingItems = new Dictionary<int, RingItemsDTO>();
            ringObj.RingItems.Add(drugID, new RingItemsDTO());
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            var nHCriteriaRingID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
            if (InventoryManager.VerifyIsExist(drugID, nHCriteriaRingID))
            {
                if (VerifyDrugEffect(drugID, out DrugData drugData))
                {

                }
            }
        }
        /// <summary>
        /// 验证丹药作用
        /// </summary>
        public bool VerifyDrugEffect(int drugid, out DrugData drugData)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, DrugData>>(out var DrugDataDict);
            if (DrugDataDict.TryGetValue(drugid, out var drugDatatemp))
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
        public void PetExpDrug(DrugData drugData, Pet pet)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);
            if (drugData.Need_Level_ID <= pet.PetLevel && drugData.Max_Level_ID >= pet.PetLevel)
            {
                pet.PetExp += drugData.Drug_Value;
                if (petLevelDataDict[pet.PetLevel].ExpLevelUp <= pet.PetExp)
                {
                    if (petLevelDataDict[pet.PetLevel].IsFinalLevel)
                    {
                        pet.PetExp = petLevelDataDict[pet.PetLevel].ExpLevelUp;
                    }
                    else
                    {
                        pet.PetLevel += 1;
                        pet.PetExp = pet.PetExp - petLevelDataDict[pet.PetLevel].ExpLevelUp;
                        //TODO刷新寵物所有屬性
                    }
                }
            }
        }
        /// <summary>
        /// 增加资质
        /// </summary>
        public void PetAtitudeDrug(DrugData drugData, Pet pet,PetAptitude petAptitude)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);
            var drugDict = Utility.Json.ToObject<Dictionary<int, int>>(petAptitude.PetAptitudeDrug);
            if (drugData.Need_Level_ID <= pet.PetLevel && drugData.Max_Level_ID >= pet.PetLevel)
            {

            }
        }
        #endregion

        #region 宠物学习技能
        public async void PetStudySkill(int bookid, NHCriteria nHCriteria,Pet pet,PetCompleteDTO petCompleteDTO)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetSkillBookData>>(out var petSkillBookDataDict);

            var ringObj = GameManager.ReferencePoolManager.Spawn<RingDTO>();
            ringObj.RingItems = new Dictionary<int, RingItemsDTO>();
            ringObj.RingItems.Add(bookid, new RingItemsDTO());
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteria);
            var skillList = Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
            var nHCriteriaRingID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);
            if (InventoryManager.VerifyIsExist(bookid, nHCriteriaRingID))
            {
                skillList = RandomSkillRemoveAdd(skillList, petSkillBookDataDict[bookid].PetSkillID);
                pet.PetSkillArray = Utility.Json.ToJson(skillList);
                await NHibernateQuerier.UpdateAsync<Pet>(pet);
                petCompleteDTO.PetDTO.PetSkillArray = skillList;
                S2CPetStudySkill(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO.PetDTO), ReturnCode.Success);
                await RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, petCompleteDTO.RoleID.ToString(), petCompleteDTO.PetDTO);
                InventoryManager.RemoveCmd(bookid, ringObj, nHCriteriaRingID);
            }
            else
            {
                S2CPetStudySkill(petCompleteDTO.RoleID, Utility.Json.ToJson(petCompleteDTO.PetDTO), ReturnCode.Fail);
            }


        }
        #endregion

        #region 妖灵精魄

        public void SetDemonicSoul(int soulid,out List<int> getSkillList)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, DemonicSoulData>>(out var DemonicSoulDict);

            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, DemonicSoulSkillPool>>(out var DemonicSoulSkillPoolDict);

            var skillPoolList = DemonicSoulDict[soulid].DemonicSoulSkill;
            Random random = new Random();
            int num = random.Next(0, skillPoolList.Count);
            var skillList = skillPoolList[num];
            getSkillList = new List<int>();
            for (int i = 0; i < skillList[0]; i++)
            {
                num = random.Next(0, DemonicSoulSkillPoolDict[0].PetSoulSkillPool.Count);
                if (getSkillList.Contains(DemonicSoulSkillPoolDict[0].PetSoulSkillPool[num]))
                {
                    getSkillList.Add(DemonicSoulSkillPoolDict[0].PetSoulSkillPool[num]);
                }
            }

            for (int i = 0; i < skillList[1]; i++)
            {
                num = random.Next(0, DemonicSoulSkillPoolDict[1].PetSoulSkillPool.Count);
                if (getSkillList.Contains(DemonicSoulSkillPoolDict[1].PetSoulSkillPool[num]))
                {
                    getSkillList.Add(DemonicSoulSkillPoolDict[1].PetSoulSkillPool[num]);
                }
            }

            for (int i = 0; i < skillList[2]; i++)
            {
                num = random.Next(0, DemonicSoulSkillPoolDict[2].PetSoulSkillPool.Count);
                if (getSkillList.Contains(DemonicSoulSkillPoolDict[2].PetSoulSkillPool[num]))
                {
                    getSkillList.Add(DemonicSoulSkillPoolDict[2].PetSoulSkillPool[num]);
                }
            }
        }

        #endregion


    }
}
