using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using RedisDotNet;
namespace AscensionServer
{
    [CustomeModule]
    public partial class PetStatusManager : Module<PetStatusManager>
    {
        /// <summary>
        /// 宠物洗练属性重置
        /// </summary>
        /// <param name="petDTO"></param>
        /// <param name="petAptitudeObj"></param>
        /// <param name="petAbilityPointDTO"></param>
        /// <param name="petStatusTemp"></param>
        public void ResetPetStatus(Pet pet , PetAptitude petAptitudeObj,out PetStatus petStatusTemp)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);

            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<string, PetAbilityPointData>>(out var petabilityPointDict);

            var result = petLevelDataDict.TryGetValue(pet.PetLevel, out var petLevelData);

            petStatusTemp = GameManager.ReferencePoolManager.Spawn<PetStatus>();

            if (result)
            {
                petStatusTemp.PetHP = (petLevelDataDict[pet.PetLevel].PetHP * petAptitudeObj.HPAptitude / 1000);
                petStatusTemp.PetMaxHP = petStatusTemp.PetHP;
                petStatusTemp.PetMP = petLevelDataDict[pet.PetLevel].PetMP ;
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
        /// 刷新宠物属性点
        /// </summary>
        /// <param name="petAbilityPointDTO"></param>
        /// <param name="petStatusDTO"></param>
        public  void RefreshAbilitty(PetAbilityPointDTO petAbilityPointDTO,out PetStatusDTO petStatusDTO)
        {
            petStatusDTO = GameManager.ReferencePoolManager.Spawn<PetStatusDTO>();
            petStatusDTO.PetMaxMP= petAbilityPointDTO.AbilityPointSln[petAbilityPointDTO.SlnNow].Strength;
        }
        /// <summary>
        /// 重置宠物资质
        /// </summary>
        /// <param name="petID"></param>
        /// <param name="petAptitudeObj"></param>
        public  void ResetPetAptitude(int petID,out PetAptitude petAptitudeObj)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetAptitudeData>>(out var petAptitudeDataDict);
            var result = petAptitudeDataDict.TryGetValue(petID, out var petAptitudeData);
            petAptitudeObj= GameManager.ReferencePoolManager.Spawn<PetAptitude>();
            if (result)
            {
                petAptitudeObj.AttackphysicalAptitude = petAptitudeData.NaturalAttackPhysical[0];
                petAptitudeObj.AttackpowerAptitude = petAptitudeData.NaturalAttackPower[0];
                petAptitudeObj.AttackspeedAptitude = petAptitudeData.NaturalAttackSpeed[0];
                petAptitudeObj.DefendphysicalAptitude = petAptitudeData.NaturalDefendPhysical[0];
                petAptitudeObj.DefendpowerAptitude = petAptitudeData.NaturalAttackPower[0];
                petAptitudeObj.HPAptitude = petAptitudeData.NaturalHP[0];
                petAptitudeObj.Petaptitudecol = petAptitudeData.NaturalGrowUp[0];
            }
        }
        /// <summary>
        /// 新增新宠物
        /// </summary>
        /// <param name="petID"></param>
        /// <param name="petName"></param>
        /// <param name="rolePet"></param>
        public void InitPet(int petID,string petName ,RolePet rolePet)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetAptitudeData>>(out var petLevelDataDict);
            #region
            var pet = GameManager.ReferencePoolManager.Spawn<Pet>();
            pet.PetID = petID;
            pet.PetName = petName;
            pet.PetSkillArray = Utility.Json.ToJson(RestPetSkill(petLevelDataDict[petID].SkillArray));
            pet = NHibernateQuerier.Insert<Pet>(pet);
            var petObj = GameManager.ReferencePoolManager.Spawn<PetDTO>();
            petObj.ID = pet.ID;
            petObj.PetExp = pet.PetExp;
            petObj.PetExtraSkill = Utility.Json.ToObject<Dictionary<int,int>>(pet.PetExtraSkill);
            petObj.PetID = pet.PetID;
            petObj.PetLevel = pet.PetLevel;
            petObj.PetName = pet.PetName;
            petObj.PetSkillArray =Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
            RedisHelper.Hash.HashSetAsync<PetDTO>(RedisKeyDefine._PetPerfix, pet.ID.ToString(), petObj);
            #endregion
            #region PetAbilityPointDTO
            var petAbilityPoint = GameManager.ReferencePoolManager.Spawn<PetAbilityPoint>();
            petAbilityPoint.ID = pet.ID;
            NHibernateQuerier.SaveOrUpdateAsync<PetAbilityPoint>(petAbilityPoint);
            var petAbilityPointObj = GameManager.ReferencePoolManager.Spawn<PetAbilityPointDTO>();
            petAbilityPointObj.ID = petAbilityPoint.ID;
            petAbilityPointObj.SlnNow = petAbilityPoint.SlnNow;
            petAbilityPointObj.IsUnlockSlnThree = petAbilityPoint.IsUnlockSlnThree;
            if (!string.IsNullOrEmpty(petAbilityPoint.AbilityPointSln))
            {
                petAbilityPointObj.AbilityPointSln = new Dictionary<int, PetAbilityDTO>();
                petAbilityPointObj.AbilityPointSln = Utility.Json.ToObject<Dictionary<int, PetAbilityDTO>>(petAbilityPoint.AbilityPointSln);
            }
            RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, pet.ID.ToString(), petAbilityPointObj);
            #endregion
            #region PetAptitudeDTO
            var petAptitude = GameManager.ReferencePoolManager.Spawn<PetAptitude>();
            petAptitude.PetID = pet.ID;
            ResetPetAptitude(petID, out petAptitude);
            var petAptitudeObj = GameManager.ReferencePoolManager.Spawn<PetAptitudeDTO>();
            petAptitudeObj.PetID = petAptitude.PetID;
            petAptitudeObj.AttackphysicalAptitude = petAptitude.AttackphysicalAptitude;
            petAptitudeObj.AttackpowerAptitude = petAptitude.AttackpowerAptitude;
            petAptitudeObj.AttackspeedAptitude = petAptitude.AttackspeedAptitude;
            petAptitudeObj.DefendphysicalAptitude = petAptitude.DefendphysicalAptitude;
            petAptitudeObj.DefendpowerAptitude = petAptitude.DefendpowerAptitude;
            petAptitudeObj.HPAptitude = petAptitude.HPAptitude;
            petAptitudeObj.Petaptitudecol = petAptitude.Petaptitudecol;
            petAptitudeObj.PetAptitudeDrug =Utility.Json.ToObject<Dictionary<int,int>>(petAptitude.PetAptitudeDrug);
            petAptitudeObj.SoulAptitude = petAptitude.SoulAptitude;

            RedisHelper.Hash.HashSetAsync<PetAptitudeDTO>(RedisKeyDefine._PetAptitudePerfix, pet.ID.ToString(), petAptitudeObj);
            NHibernateQuerier.SaveOrUpdateAsync<PetAptitude>(petAptitude);
            #endregion
            #region PetStatusDTO
            var petStatus = GameManager.ReferencePoolManager.Spawn<PetStatus>();
            ResetPetStatus(pet, petAptitude, out petStatus);
            petStatus.PetID = pet.ID;
            RedisHelper.Hash.HashSetAsync<PetStatus>(RedisKeyDefine._PetStatusPerfix , pet.ID.ToString(), petStatus);

            NHibernateQuerier.SaveOrUpdateAsync<PetStatus>(petStatus);
            #endregion
            #region RolePetDTO
            var petDict = Utility.Json.ToObject<Dictionary<int, int>>(rolePet.PetIDDict);
            petDict.Add(pet.ID, pet.PetID);
            rolePet.PetIDDict = Utility.Json.ToJson(petDict);
            NHibernateQuerier.SaveOrUpdateAsync<RolePet>(rolePet);
            var RolepetObj = GameManager.ReferencePoolManager.Spawn<RolePetDTO>();
            RolepetObj.RoleID = rolePet.RoleID;
            RolepetObj.PetIDDict = petDict;
            RolepetObj.PetIsBattle = rolePet.PetIsBattle;
            RedisHelper.Hash.HashSetAsync<RolePetDTO>(RedisKeyDefine._RolePetPerfix, rolePet.RoleID.ToString(), RolepetObj);
            #endregion

            S2CRoleAddPetSuccess(rolePet.RoleID);
            GameManager.ReferencePoolManager.Despawns(pet, petStatus, petAptitude, petAbilityPoint, RolepetObj, petAptitudeObj, petAbilityPointObj);
        }


        /// <summary>
        /// 洗练宠物重置技能
        /// </summary>
        public  List<int> RestPetSkill(List<int> skillList )
        {
         return RandomArrayElement(skillList);
        }
        /// <summary>
        /// 技能书使用顶替技能
        /// </summary>
        public  List<int> RandomSkillRemoveAdd(List<int> skillList)
        {
           int num=(int)AverageRandom(0,skillList.Count);
            skillList.Remove(num);
            return skillList;
        }

        #region 正态分部随机数
        /// <summary>
        /// 正态分布随机数
        /// </summary>

        static Random aa = new Random((int)(DateTime.Now.Ticks / 10000));
        public  double AverageRandom(double min, double max)//产生(min,max)之间均匀分布的随机数
        {
            int MINnteger = (int)(min * 10000);
            int MAXnteger = (int)(max * 10000);
            int resultInteger = aa.Next(MINnteger, MAXnteger);
            return resultInteger / 10000.0;
        }
        public double Normal(double x, double miu, double sigma) //正态分布概率密度函数
        {
            return 1.0 / (x * Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-1 * (Math.Log(x) - miu) * (Math.Log(x) - miu) / (2 * sigma * sigma));
        }
        public double Random_Normal(double miu, double sigma, double min, double max)//产生正态分布随机数
        {
            double x;
            double dScope;
            double y;
            do
            {
                x = AverageRandom(min, max);
                y = Normal(x, miu, sigma);
                dScope = AverageRandom(0, Normal(miu, miu, sigma));
            } while (dScope > y);
            return x;
        }

        #endregion

        #region 技能数组随机
        public  List<int> RandomArrayElement(List<int> skillList)
        {
            double randomNum = AverageRandom(0, skillList.Count);
            List<int> numList = new List<int>();
            List<int> tempSkillList = new List<int>();
            for (int i = 0; i < randomNum; i++)
            {
                var temp = (int)AverageRandom(1, skillList.Count);
                if (!numList.Contains(temp))
                {
                    numList.Add(temp);
                    tempSkillList.Add(skillList[temp]);
                }
            }
            return tempSkillList;
        }

        #endregion
    }
}
