﻿using System;
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
        #region 宠物加点
        public async void RenamePointSln(int roleid, PetAbilityPointDTO pointDTO)
        {
            NHCriteria nHCriteriapetStatus = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", pointDTO.ID);
            var petAbilityPoint = NHibernateQuerier.CriteriaSelect<PetAbilityPoint>(nHCriteriapetStatus);


            var pointSlnDict = Utility.Json.ToObject<Dictionary<int, AbilityDTO>>(petAbilityPoint.AbilityPointSln);
            if (pointSlnDict.TryGetValue(pointDTO.SlnNow, out var AbilityDTO))
            {
                AbilityDTO.SlnName = pointDTO.AbilityPointSln[pointDTO.SlnNow].SlnName;
                pointSlnDict[pointDTO.SlnNow] = AbilityDTO;

                pointDTO.AbilityPointSln = pointSlnDict;
                pointDTO.IsUnlockSlnThree = petAbilityPoint.IsUnlockSlnThree;
                petAbilityPoint.AbilityPointSln = Utility.Json.ToJson(pointSlnDict);
                await NHibernateQuerier.UpdateAsync<PetAbilityPoint>(petAbilityPoint);
                await RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, pointDTO.ID.ToString(), pointDTO);
                ResultSuccseS2C(roleid, RolePetOpCode.RenamePetAbilitySln, pointDTO);
            }
        }
        /// <summary>
        /// 更新宠物加点
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="petAbilityPoint"></param>
        /// <param name="abilityPointDTO"></param>
        public async void UpdatePointSln(int roleid, PetAbilityPointDTO abilityPointDTO)
        {
            NHCriteria nHCriteriapetStatus = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", abilityPointDTO.ID);
            var petAbilityPoint = NHibernateQuerier.CriteriaSelect<PetAbilityPoint>(nHCriteriapetStatus);

            var pointSlnDict = Utility.Json.ToObject<Dictionary<int, AbilityDTO>>(petAbilityPoint.AbilityPointSln);
            if (pointSlnDict.TryGetValue(abilityPointDTO.SlnNow, out var AbilityDTO))
            {
                var point = abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Agility + abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Power + abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Strength + abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Soul + abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Corporeity + abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Stamina;
                if (point<= AbilityDTO.SurplusAptitudePoint)
                {

                    AbilityDTO.Agility += abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Agility;
                    AbilityDTO.Power += abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Power;
                    AbilityDTO.Strength += abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Strength;
                    AbilityDTO.Soul += abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Soul;
                    AbilityDTO.Corporeity += abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Corporeity;
                    AbilityDTO.Stamina += abilityPointDTO.AbilityPointSln[abilityPointDTO.SlnNow].Stamina;
                    AbilityDTO.SurplusAptitudePoint -= point;
                    AbilityDTO.IsSet = true;

                    pointSlnDict[abilityPointDTO.SlnNow] = AbilityDTO;

                    abilityPointDTO.AbilityPointSln = pointSlnDict;
                    petAbilityPoint.AbilityPointSln = Utility.Json.ToJson(pointSlnDict);

                    var status = VerifyPetAllStatus(petAbilityPoint.ID, ChangeDataType(petAbilityPoint), null, null);
                    status.PetID = abilityPointDTO.ID;
                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.PetStatus, status);
                    dict.Add((byte)ParameterCode.PetAbility, abilityPointDTO);
                    ResultSuccseS2C(roleid, RolePetOpCode.SetAdditionPoint, dict);

                    await NHibernateQuerier.UpdateAsync<PetAbilityPoint>(petAbilityPoint);
                    await RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, abilityPointDTO.ID.ToString(), abilityPointDTO);

                    await NHibernateQuerier.UpdateAsync(status);
                    await RedisHelper.Hash.HashSetAsync<PetStatus>(RedisKeyDefine._PetStatusPerfix, abilityPointDTO.ID.ToString(), status);
                }else
                    ResultFailS2C(roleid, RolePetOpCode.SetAdditionPoint);
            }
            else
                ResultFailS2C(roleid, RolePetOpCode.SetAdditionPoint);
        }

        /// <summary>
        /// 解锁加点方案
        /// </summary>
        /// <param name="petAbilityPoint"></param>
        /// <param name="nHCriteria"></param>
        /// <param name="petCompleteDTO"></param>
        public async void UlockPointSln(int roleid, PetAbilityPointDTO pointDTO)
        {
            NHCriteria nHCriteriapetStatus = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", pointDTO.ID);
            var petAbilityPoint = NHibernateQuerier.CriteriaSelect<PetAbilityPoint>(nHCriteriapetStatus);

            var pointSlnDict = Utility.Json.ToObject<Dictionary<int, AbilityDTO>>(petAbilityPoint.AbilityPointSln);

            NHCriteria nHCriteriarole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleid);
            var roleassets = NHibernateQuerier.CriteriaSelectAsync<RoleAssets>(nHCriteriarole).Result;
            if (roleassets.SpiritStonesLow >= 0)
            {
                //if (pointSlnDict.TryGetValue(petCompleteDTO.PetAbilityPointDTO.SlnNow, out var AbilityDTO))
                //{                  
                //}
                pointSlnDict.Add(pointDTO.SlnNow, new AbilityDTO() { SlnName = "方案三" });
                petAbilityPoint.IsUnlockSlnThree = true;
                petAbilityPoint.AbilityPointSln = Utility.Json.ToJson(pointSlnDict);
                roleassets.SpiritStonesLow -= 0;

                pointDTO.IsUnlockSlnThree = true;
                await NHibernateQuerier.UpdateAsync<RoleAssets>(roleassets);
                await RedisHelper.Hash.HashSetAsync<RoleAssets>(RedisKeyDefine._RoleAssetsPerfix, roleassets.RoleID.ToString(), roleassets);

                pointDTO.AbilityPointSln = pointSlnDict;

                pointDTO.IsUnlockSlnThree = true;
                await NHibernateQuerier.UpdateAsync<PetAbilityPoint>(petAbilityPoint);
                await RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, pointDTO.ID.ToString(), pointDTO);

                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                dict.Add((byte)ParameterCode.RoleAssets, roleassets);
                dict.Add((byte)ParameterCode.PetAbility, pointDTO);
                ResultSuccseS2C(roleid, RolePetOpCode.UnlockPetAbilitySln, dict);
            }
        }

        public void ResetAbilityPoint(int roleid, PetAbilityPointDTO petAbilityPoint)
        {
            NHCriteria nHCriteriapetStatus = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petAbilityPoint.ID);
            var petAbilityPointObj = NHibernateQuerier.CriteriaSelect<PetAbilityPoint>(nHCriteriapetStatus);
            var petObj = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriapetStatus);

            GameEntry.DataManager.TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);
            int point = 0;
            if (petAbilityPointObj != null && petObj != null)
            {
                foreach (var item in petLevelDataDict)
                {
                    if (item.Value.PetLevelID <= petObj.PetLevel)
                    {
                        point += item.Value.FreeAttributes;
                    }
                    else
                        break;
                }

                var slnDict = Utility.Json.ToObject<Dictionary<int, AbilityDTO>>(petAbilityPointObj.AbilityPointSln);

                Utility.Debug.LogError("重置加点加点收到的数据" + petAbilityPointObj.AbilityPointSln);
                if (slnDict.TryGetValue(petAbilityPoint.SlnNow, out var abilityDTO))
                {
                    Utility.Debug.LogError("重置加点加点收到的数据");

                    abilityDTO.Agility = 0;
                    abilityDTO.Corporeity = 0;
                    abilityDTO.Soul = 0;
                    abilityDTO.Stamina = 0;
                    abilityDTO.Power = 0;
                    abilityDTO.Strength = 0;
                    abilityDTO.SurplusAptitudePoint = point;
                    petAbilityPointObj.SlnNow = petAbilityPoint.SlnNow;
                    slnDict[petAbilityPoint.SlnNow] = abilityDTO;
                    petAbilityPointObj.AbilityPointSln = Utility.Json.ToJson(slnDict);

                    var status = VerifyPetAllStatus(petAbilityPoint.ID, ChangeDataType(petAbilityPointObj), null, null);

                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.PetStatus, status);
                    dict.Add((byte)ParameterCode.PetAbility, ChangeDataType(petAbilityPointObj));
                    ResultSuccseS2C(roleid, RolePetOpCode.ResetPetAbilitySln, dict);
                }
                else
                    ResultFailS2C(roleid, RolePetOpCode.ResetPetAbilitySln);
            }
        }

        #endregion

        #region 切换加点方案
        async void SwitchPetAbilitySlnS2C(int roleid, PetAbilityPointDTO pointDTO)
        {
            var pointExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._PetAbilityPointPerfix, pointDTO.ID.ToString()).Result;
            if (pointExist)
            {
                var point = RedisHelper.Hash.HashGetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, pointDTO.ID.ToString()).Result;
                if (point != null)
                {
                    if (point.AbilityPointSln.ContainsKey(pointDTO.SlnNow))
                    {
                        point.SlnNow = pointDTO.SlnNow;
                        var status = VerifyPetAllStatus(pointDTO.ID, pointDTO, null, null);

                        Dictionary<byte, object> dict = new Dictionary<byte, object>();
                        dict.Add((byte)ParameterCode.PetAbility, point);
                        dict.Add((byte)ParameterCode.PetStatus, status);
                        ResultSuccseS2C(roleid, RolePetOpCode.SwitchPetAbilitySln, dict);
                    }
                    else
                        ResultFailS2C(roleid, RolePetOpCode.SwitchPetAbilitySln);
                }
                else
                    SwitchPetAbilitySlnMySql(roleid, pointDTO);
            }
            else
                SwitchPetAbilitySlnMySql(roleid, pointDTO);
        }
        async void SwitchPetAbilitySlnMySql(int roleid, PetAbilityPointDTO pointDTO)
        {
            NHCriteria nHCriteriapet = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", pointDTO.ID);
            var point = NHibernateQuerier.CriteriaSelectAsync<PetAbilityPoint>(nHCriteriapet).Result;
            if (point != null)
            {
                var slnDict = Utility.Json.ToObject<Dictionary<int, AbilityDTO>>(point.AbilityPointSln);
                if (slnDict.ContainsKey(pointDTO.SlnNow))
                {
                    point.SlnNow = pointDTO.SlnNow;
                    var status = VerifyPetAllStatus(pointDTO.ID, pointDTO, null, null);

                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.PetAbility, ChangeDataType(point));
                    dict.Add((byte)ParameterCode.PetStatus, status);
                    ResultSuccseS2C(roleid, RolePetOpCode.SwitchPetAbilitySln, dict);
                }
                else
                    ResultFailS2C(roleid, RolePetOpCode.SwitchPetAbilitySln);
            }
        }
        #endregion

        #region 宠物洗练
        public async void PetStatusRestoreDefault(int itemid, int roleid, int petid)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, PetAptitudeData>>(out var petLevelDataDict);

            NHCriteria nHCriteriapetStatus = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petid);
            var pet = NHibernateQuerier.CriteriaSelect<Pet>(nHCriteriapetStatus);
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleid);
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

            pet.PetLevel = 1;
            pet.PetExp = 0;
            pet.PetSkillArray = Utility.Json.ToJson(petObj.PetSkillArray);
            await NHibernateQuerier.UpdateAsync(pet);

            var petAbitilyObj = CosmosEntry.ReferencePoolManager.Spawn<PetAbilityPointDTO>();
            petAbitilyObj.ID = pet.ID;
            await RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, petObj.ID.ToString(), petAbitilyObj);
            var petAbitily = CosmosEntry.ReferencePoolManager.Spawn<PetAbilityPoint>();
            petAbitily.ID = pet.ID;
            await NHibernateQuerier.UpdateAsync(petAbitily);

            var petAptitudeObj = CosmosEntry.ReferencePoolManager.Spawn<PetAptitudeDTO>();
            var petAptitude = CosmosEntry.ReferencePoolManager.Spawn<PetAptitude>();
            ResetPetAptitude(pet.PetID, out petAptitude);
            petAptitude.PetID = pet.ID;
            petAptitudeObj = AssignSameFieldValue(petAptitudeObj, petAptitude);
            petAptitudeObj.PetID = pet.ID;
            await RedisHelper.Hash.HashSetAsync<PetAptitudeDTO>(RedisKeyDefine._PetAptitudePerfix, petObj.ID.ToString(), petAptitudeObj);
            await NHibernateQuerier.UpdateAsync(petAptitude);

            var petstatusObj = CosmosEntry.ReferencePoolManager.Spawn<PetStatusDTO>();
            var petstatus = CosmosEntry.ReferencePoolManager.Spawn<PetStatus>();
            ResetPetStatus(pet, petAptitude, out petstatus);
            petstatus.PetID = pet.ID;
            petstatusObj = Utility.Assembly.AssignSameFieldValue<PetStatus, PetStatusDTO>(petstatus, petstatusObj);
            await RedisHelper.Hash.HashSetAsync<PetStatusDTO>(RedisKeyDefine._PetStatusPerfix, petObj.ID.ToString(), petstatusObj);
            await NHibernateQuerier.UpdateAsync(petstatus);

            Dictionary<byte, object> dict = new Dictionary<byte, object>();
            dict.Add((byte)ParameterCode.Pet, petObj);
            dict.Add((byte)ParameterCode.PetAptitude, petAptitudeObj);
            dict.Add((byte)ParameterCode.PetAbility, petAbitilyObj);
            dict.Add((byte)ParameterCode.PetStatus, petstatusObj);
            ResultSuccseS2C(roleid, RolePetOpCode.ResetPetStatus, dict);
        }
        #endregion

        /// <summary>
        /// 宠物洗练属性重置
        /// </summary>
        /// <param name="petDTO"></param>
        /// <param name="petAptitudeObj"></param>
        /// <param name="petAbilityPointDTO"></param>
        /// <param name="petStatusTemp"></param>
        public void ResetPetStatus(Pet pet, PetAptitude petAptitudeObj, out PetStatus petStatusTemp)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);
            GameEntry.DataManager.TryGetValue<Dictionary<string, PetAbilityPointData>>(out var petabilityPointDict);

            var result = petLevelDataDict.TryGetValue(pet.PetLevel, out var petLevelData);

            petStatusTemp = CosmosEntry.ReferencePoolManager.Spawn<PetStatus>();

            if (result)
            {
                petStatusTemp.PetHP = (petLevelDataDict[pet.PetLevel].PetHP * petAptitudeObj.HPAptitude / 1000);
                petStatusTemp.PetMaxHP = petStatusTemp.PetHP;
                petStatusTemp.PetMP = petLevelDataDict[pet.PetLevel].PetMP;
                petStatusTemp.PetMaxMP = petStatusTemp.PetMP;
                petStatusTemp.PetShenhun = (int)(petLevelDataDict[pet.PetLevel].Petsoul * petAptitudeObj.SoulAptitude / 1000);
                petStatusTemp.PetMaxShenhun = petStatusTemp.PetShenhun;
                petStatusTemp.AttackPhysical = (petLevelDataDict[pet.PetLevel].AttackPhysical * petAptitudeObj.AttackphysicalAptitude / 1000);
                petStatusTemp.AttackPower = (petLevelDataDict[pet.PetLevel].AttackPower * petAptitudeObj.AttackpowerAptitude / 1000);
                petStatusTemp.AttackSpeed = (petLevelDataDict[pet.PetLevel].AttackSpeed * petAptitudeObj.AttackspeedAptitude / 1000);
                petStatusTemp.DefendPhysical = (petLevelDataDict[pet.PetLevel].DefendPhysical * petAptitudeObj.DefendphysicalAptitude / 1000);
                petStatusTemp.DefendPower = (petLevelDataDict[pet.PetLevel].DefendPower * petAptitudeObj.DefendpowerAptitude / 1000);
                petStatusTemp.ExpLevelUp = petLevelDataDict[pet.PetLevel].ExpLevelUp;
                petStatusTemp.MagicCritDamage = petLevelDataDict[pet.PetLevel].MagicCritDamage;
                petStatusTemp.MagicCritProb = petLevelDataDict[pet.PetLevel].MagicCritProb;
                petStatusTemp.PhysicalCritDamage = petLevelDataDict[pet.PetLevel].PhysicalCritDamage;
                petStatusTemp.PhysicalCritProb = petLevelDataDict[pet.PetLevel].PhysicalCritProb;
            }
        }

        /// <summary>
        /// 重置宠物资质
        /// </summary>
        /// <param name="petID"></param>
        /// <param name="petAptitudeObj"></param>
        public void ResetPetAptitude(int petID, out PetAptitude petAptitudeObj)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, PetAptitudeData>>(out var petAptitudeDataDict);
            var result = petAptitudeDataDict.TryGetValue(petID, out var petAptitudeData);
            petAptitudeObj = CosmosEntry.ReferencePoolManager.Spawn<PetAptitude>();
            if (result)
            {
                petAptitudeObj.AttackphysicalAptitude = petAptitudeData.NaturalAttackPhysical[0];
                petAptitudeObj.AttackpowerAptitude = petAptitudeData.NaturalAttackPower[0];
                petAptitudeObj.AttackspeedAptitude = petAptitudeData.NaturalAttackSpeed[0];
                petAptitudeObj.DefendphysicalAptitude = petAptitudeData.NaturalDefendPhysical[0];
                petAptitudeObj.DefendpowerAptitude = petAptitudeData.NaturalAttackPower[0];
                petAptitudeObj.HPAptitude = petAptitudeData.NaturalHP[0];
                petAptitudeObj.Petaptitudecol = petAptitudeData.NaturalGrowUp[0];
                petAptitudeObj.SoulAptitude = petAptitudeData.NaturalSoul[0];
            }
        }

        #region 洗练宠物
        public async void ResetPetAllStatus(int petID, string petName, RolePet rolePet)
        {
            GameEntry.DataManager.TryGetValue<Dictionary<int, PetAptitudeData>>(out var petLevelDataDict);
            #region Pet
            var pet = new Pet();
            pet.PetID = petID;
            pet.PetName = petName;
            pet.PetLevel = 1;
            pet.PetSkillArray = Utility.Json.ToJson(RestPetSkill(petLevelDataDict[petID].SkillArray));
            pet = NHibernateQuerier.Insert<Pet>(pet);
            var petObj = new PetDTO();
            petObj.ID = pet.ID;
            petObj.PetExp = pet.PetExp;
            petObj.DemonicSoul = Utility.Json.ToObject<Dictionary<int, List<int>>>(pet.DemonicSoul);
            petObj.PetID = pet.PetID;
            petObj.PetLevel = pet.PetLevel;
            petObj.PetName = pet.PetName;
            petObj.PetSkillArray = Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
            await RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, pet.ID.ToString(), petObj);
            #endregion
            #region PetAbilityPointDTO
            var petAbilityPointObj = new PetAbilityPointDTO();
            var petAbilityPoint = new PetAbilityPoint();
            petAbilityPoint.ID = pet.ID;
            petAbilityPoint.AbilityPointSln = Utility.Json.ToJson(petAbilityPointObj.AbilityPointSln);
            Utility.Debug.LogInfo("yzqData宠物加点方案" + Utility.Json.ToJson(petAbilityPointObj.AbilityPointSln));
            await NHibernateQuerier.SaveOrUpdateAsync<PetAbilityPoint>(petAbilityPoint);
            petAbilityPointObj.ID = petAbilityPoint.ID;
            await RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, pet.ID.ToString(), petAbilityPointObj);
            #endregion
            #region PetAptitudeDTO
            var petAptitude = new PetAptitude();
            ResetPetAptitude(petID, out petAptitude);
            petAptitude.PetID = pet.ID;
            var petAptitudeObj = new PetAptitudeDTO();
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

            await RedisHelper.Hash.HashSetAsync<PetAptitudeDTO>(RedisKeyDefine._PetAptitudePerfix, pet.ID.ToString(), petAptitudeObj);
            await NHibernateQuerier.SaveOrUpdateAsync<PetAptitude>(petAptitude);
            #endregion
            #region PetStatusDTO
            var petStatus = new PetStatus();
            ResetPetStatus(pet, petAptitude, out petStatus);
            petStatus.PetID = pet.ID;
            await RedisHelper.Hash.HashSetAsync<PetStatus>(RedisKeyDefine._PetStatusPerfix, pet.ID.ToString(), petStatus);

            await NHibernateQuerier.SaveOrUpdateAsync<PetStatus>(petStatus);
            #endregion
            #region RolePetDTO
            var petDict = Utility.Json.ToObject<Dictionary<int, int>>(rolePet.PetIDDict);
            petDict.Add(pet.ID, pet.PetID);
            rolePet.PetIDDict = Utility.Json.ToJson(petDict);
            await NHibernateQuerier.SaveOrUpdateAsync<RolePet>(rolePet);
            var RolepetObj = new RolePetDTO();
            RolepetObj.RoleID = rolePet.RoleID;
            RolepetObj.PetIDDict = petDict;
            RolepetObj.PetIsBattle = rolePet.PetIsBattle;
            await RedisHelper.Hash.HashSetAsync<RolePetDTO>(RedisKeyDefine._RolePetPerfix, rolePet.RoleID.ToString(), RolepetObj);
            #endregion

            Dictionary<byte, object> dict = new Dictionary<byte, object>();
            dict.Add((byte)ParameterCode.Pet, petObj);
            dict.Add((byte)ParameterCode.PetAbility, petAbilityPointObj);
            dict.Add((byte)ParameterCode.PetAptitude, petAptitudeObj);
            dict.Add((byte)ParameterCode.PetStatus, petStatus);
            dict.Add((byte)ParameterCode.RolePet, RolepetObj);

            ResultSuccseS2C(rolePet.RoleID, RolePetOpCode.ResetPetStatus, dict);
        }
        #endregion

    }
}


