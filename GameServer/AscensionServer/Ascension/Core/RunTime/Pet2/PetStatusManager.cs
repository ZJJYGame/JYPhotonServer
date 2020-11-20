using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
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
        public  void ResetPetStatus(PetDTO petDTO , PetaPtitudeDTO petAptitudeObj,PetAbilityPointDTO petAbilityPointDTO,out PetStatusDTO petStatusTemp)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);

            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<string, PetAbilityPointData>>(out var petabilityPointDict);

            var result = petLevelDataDict.TryGetValue(petDTO.PetLevel, out var petLevelData);

            petStatusTemp = GameManager.ReferencePoolManager.Spawn<PetStatusDTO>();

            if (result)
            {
                petStatusTemp.PetHP = (petLevelDataDict[petDTO.PetLevel].PetHP * petAptitudeObj.HPAptitude / 1000);
                petStatusTemp.PetMaxHP = petStatusTemp.PetHP;
                petStatusTemp.PetMP = petLevelDataDict[petDTO.PetLevel].PetMP ;
                petStatusTemp.PetMaxMP = petStatusTemp.PetMP;
                petStatusTemp.PetShenhun = (int)(petLevelDataDict[petDTO.PetLevel].Petsoul * petAptitudeObj.SoulAptitude / 1000);
                petStatusTemp.PetMaxShenhun = petStatusTemp.PetShenhun;
                petStatusTemp.AttackPhysical = (petLevelDataDict[petDTO.PetLevel].AttackPhysical * petAptitudeObj.AttackphysicalAptitude / 1000);
                petStatusTemp.AttackPower = (petLevelDataDict[petDTO.PetLevel].AttackPower * petAptitudeObj.AttackpowerAptitude / 1000);
                petStatusTemp.AttackSpeed = (petLevelDataDict[petDTO.PetLevel].AttackSpeed * petAptitudeObj.AttackspeedAptitude / 1000);
                petStatusTemp.DefendPhysical = (petLevelDataDict[petDTO.PetLevel].DefendPhysical * petAptitudeObj.DefendphysicalAptitude / 1000);
                petStatusTemp.DefendPower = (petLevelDataDict[petDTO.PetLevel].DefendPower * petAptitudeObj.DefendpowerAptitude / 1000);
                petStatusTemp.ExpLevelUp = petLevelDataDict[petDTO.PetLevel].ExpLevelUp;
                petStatusTemp.MagicCritDamage = petLevelDataDict[petDTO.PetLevel].MagicCritDamage;
                petStatusTemp.MagicCritProb = petLevelDataDict[petDTO.PetLevel].MagicCritProb;
                petStatusTemp.PhysicalCritDamage = petLevelDataDict[petDTO.PetLevel].PhysicalCritDamage;
                petStatusTemp.PhysicalCritProb = petLevelDataDict[petDTO.PetLevel].PhysicalCritProb;
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
        public  void ResetPetAptitude(int petID,out PetaPtitudeDTO petAptitudeObj)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetAptitudeData>>(out var petAptitudeDataDict);
            var result = petAptitudeDataDict.TryGetValue(petID, out var petAptitudeData);
            petAptitudeObj= GameManager.ReferencePoolManager.Spawn<PetaPtitudeDTO>();
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
            for (int i = 0; i < randomNum; i++)
            {
                var temp = (int)AverageRandom(1, skillList.Count);
                if (!numList.Contains(temp))
                {
                    numList.Add(temp);
                }
            }
            return numList;
        }

        #endregion
    }
}
