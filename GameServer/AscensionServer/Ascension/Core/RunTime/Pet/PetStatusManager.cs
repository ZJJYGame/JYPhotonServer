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
        public void ResetPetStatus(Pet pet, PetAptitude petAptitudeObj, out PetStatus petStatusTemp)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);

            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<string, PetAbilityPointData>>(out var petabilityPointDict);

            var result = petLevelDataDict.TryGetValue(pet.PetLevel, out var petLevelData);

            petStatusTemp = GameManager.ReferencePoolManager.Spawn<PetStatus>();

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
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetAptitudeData>>(out var petAptitudeDataDict);
            var result = petAptitudeDataDict.TryGetValue(petID, out var petAptitudeData);
            petAptitudeObj = GameManager.ReferencePoolManager.Spawn<PetAptitude>();
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
        public void InitPet(int petID, string petName, RolePet rolePet)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetAptitudeData>>(out var petLevelDataDict);
            #region Pet
            var pet = GameManager.ReferencePoolManager.Spawn<Pet>();
            pet.PetID = petID;
            pet.PetName = petName;
            pet.PetLevel = 1;
            pet.PetSkillArray = Utility.Json.ToJson(RestPetSkill(petLevelDataDict[petID].SkillArray));
            pet = NHibernateQuerier.Insert<Pet>(pet);
            var petObj = GameManager.ReferencePoolManager.Spawn<PetDTO>();
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
            var petAbilityPointObj = GameManager.ReferencePoolManager.Spawn<PetAbilityPointDTO>();
            var petAbilityPoint = GameManager.ReferencePoolManager.Spawn<PetAbilityPoint>();
            petAbilityPoint.ID = pet.ID;
            petAbilityPoint.AbilityPointSln = Utility.Json.ToJson(petAbilityPointObj.AbilityPointSln);
            Utility.Debug.LogInfo("yzqData宠物加点方案" + Utility.Json.ToJson(petAbilityPointObj.AbilityPointSln));
            NHibernateQuerier.SaveOrUpdateAsync<PetAbilityPoint>(petAbilityPoint);
            petAbilityPointObj.ID = petAbilityPoint.ID;
            RedisHelper.Hash.HashSetAsync<PetAbilityPointDTO>(RedisKeyDefine._PetAbilityPointPerfix, pet.ID.ToString(), petAbilityPointObj);
            #endregion
            #region PetAptitudeDTO
            var petAptitude = GameManager.ReferencePoolManager.Spawn<PetAptitude>();
            ResetPetAptitude(petID, out petAptitude);
            petAptitude.PetID = pet.ID;
            var petAptitudeObj = GameManager.ReferencePoolManager.Spawn<PetAptitudeDTO>();
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
            var petStatus = GameManager.ReferencePoolManager.Spawn<PetStatus>();
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
        public List<int> RestPetSkill(List<int> skillList)
        {
            return RandomArrayElement(skillList);
        }
        /// <summary>
        /// 技能书使用顶替技能
        /// </summary>
        public List<int> RandomSkillRemoveAdd(List<int> skillList, int skillid)
        {
            int num = (int)AverageRandom(0, skillList.Count);
            skillList.Remove(num);
            skillList.Add(skillid);
            return skillList;
        }

        #region 正态分部随机数
        /// <summary>
        /// 正态分布随机数
        /// </summary>

        static Random aa = new Random((int)(DateTime.Now.Ticks / 10000));
        public double AverageRandom(double min, double max)//产生(min,max)之间均匀分布的随机数
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
        public List<int> RandomArrayElement(List<int> skillList)
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

        #region 真正的正态分布策划版
        int[,] Map = new int[25,10]
        {
            {5000,5040,5080,5120,5160,5199,5239,5279,5319,5359},
            {5398,5438,5478,5517,5557,5596,5636,5675,5714,5753},
            {5793,5832,5871,5910,5948,5987,6026,6064,6103,6141},
            {6179,6217,6255,6293,6331,6368,6406,6443,6480,6517},
            {6554,6591,6628,6664,6700,6736,6772,6808,6844,6879},
            {6915,6950,6985,7019,7054,7088,7123,7157,7190,7224},
            {7257,7291,7324,7357,7389,7422,7454,7486,7517,7549},
            {7580,7611,7642,7673,7703,7734,7764,7794,7823,7852},
            {7881,7910,7939,7967,7995,8023,8051,8078,8106,8133},
            {8159,8186,8212,8238,8264,8289,8315,8340,8365,8389},
            {8413,8438,8461,8485,8508,8531,8554,8577,8599,8621},
            {8643,8665,8686,8708,8729,8749,8770,8790,8810,8830},
            {8849,8869,8888,8907,8925,8944,8962,8980,8997,9015},
            {9032,9049,9066,9082,9099,9115,9131,9147,9162,9177},
            {9192,9207,9222,9236,9251,9265,9278,9292,9306,9319},
            {9332,9345,9357,9370,9382,9394,9406,9418,9430,9441},
            {9452,9463,9474,9484,9495,9505,9515,9525,9535,9545},
            {9554,9564,9573,9582,9591,9599,9608,9616,9625,9633},
            {9641,9648,9656,9664,9671,9678,9686,9693,9700,9706},
            {9713,9719,9726,9732,9738,9744,9750,9756,9762,9767},
            {9772,9778,9783,9788,9793,9798,9803,9808,9812,9817},
            {9821,9826,9830,9834,9838,9842,9846,9850,9854,9857},
            {9861,9864,9868,9871,9874,9878,9881,9884,9887,9890},
            {9893,9896,9898,9901,9904,9906,9909,9911,9913,9916},
            {9918,9920,9922,9925,9927,9929,9931,9932,9934,9936}
        };
        Random random = new Random();
        int NormalRandom2(int min, int max)
        {
            int result = 0;
            bool flag = true;
            while (flag)
            {

                int x = random.Next(0,250);
                int y = 10000 - Map[x / 10,x % 10];
                if (y >= (int)random.Next(0, 10000))
                {
                    result = x;
                    flag = false;
                }
            }
            if ((int)AverageRandom(0, 2)==1)
            {
                result = (min + max) / 2 - (result + (int)random.Next(0, 2)) * (max - min) / 2 / 250;
            }
            else
            {
                result = (min + max) / 2 + (result + (int)random.Next(0, 2)) * (max - min) / 2 / 250;
            }
            return result;
        }
        #endregion
    }
}
