using Protocol;
using Cosmos;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using System.Collections.Generic;

namespace AscensionServer
{
   public partial class PetStatusManager
    {
        /// <summary>
        /// 验证宠物当前加点数
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="points"></param>
        public void VerifyPetAbilitypoint(Pet pet,out int points)
        {
            points = 0;          
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);
            foreach (var item in petLevelDataDict)
            {
                if (pet.PetLevel>= item.Key)
                {
                    points += item.Value.FreeAttributes;
                }
            }
        }
        public enum AttributionType
        {
            HP=0,
            MP=1,
            ShenHun=2,
            BestBlood=3,
            AttackPhysical=4,
            DefendPhysical=5,
            AttackPower=6,
            DefendPower=7,
            AttackSpeed=8,
            MoveSpeed=9,
            PhysicalCritProb=10,
            MagicCritProb=11,
            ReduceCritProb=12,
            PhysicalCritDamage=13,
            MagicCritDamage=14,
            ReduceCritDamage=15
        }

        public enum GrowUpDrugType : int
        {
            HPGrowUp = 16039,
            SoulGrowUp = 16041,
            SpeedGrowUp = 16042,
            AttackDamageGrowUp = 16043,
            ResistanceAttackGrowUp = 16044,
            PowerGrowUp = 16045,
            ResistancePowerGrowUp = 16046,
            GrowUp = 16049
        }

        /// <summary>
        /// 验证宠物完整属性
        /// </summary>
        public PetStatusDTO VerifyPetAllStatus(PetAbilityPointDTO petAbilityPoint, PetAptitude petAptitude,PetStatus petStatus,PetCompleteDTO petCompleteDTO,Pet pet)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);

         //  var petStatus=  VerifyPetAbilityAddition(petAbilityPoint, petStatusDTO, petLevelDataDict[pet.PetLevel]);
            var skillList = Utility.Json.ToObject<List<int>>(pet.PetSkillArray);
            VerifyPetPassivitySkill(skillList,out var petStatusDTOs);
            ResetPetStatus(pet, petAptitude,out var petStatusAtitude);
            StatusAddition(petStatusAtitude, petStatusDTOs, out var petStatusTemp);
            return petStatusTemp;
        }
        /// <summary>
        /// 验证加点后的加成
        /// </summary>
        /// <param name="petAbilityPoint"></param>
        /// <param name="petStatusDTO"></param>
        /// <param name="petLevelData"></param>
        /// <returns></returns>
        public PetStatusDTO VerifyPetAbilityAddition(PetAbilityPointDTO petAbilityPoint,PetStatusDTO petStatusDTO,PetLevelData petLevelData)
        {
            petStatusDTO.PetHP = (petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Stamina * 2 + petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Corporeity * 4) * petLevelData.PetHP;
            petStatusDTO.PetMP = petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Power * 5 * petLevelData.PetMP;
            petStatusDTO.PetShenhun = (int)(petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Soul * 1 * petLevelData.Petsoul);
            petStatusDTO.AttackSpeed =(int) (petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Agility *0.5f * petLevelData.AttackSpeed);
            petStatusDTO.AttackPhysical = petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Strength * 1*petLevelData.AttackPhysical;
            petStatusDTO.DefendPhysical = (int)((petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Strength * 0.1+ petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Stamina * 0.6+ petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Corporeity * 0.2)*petLevelData.DefendPhysical);
            petStatusDTO.AttackPower = petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Power * 1* petLevelData.AttackPower;
            petStatusDTO.DefendPower = (int)((petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Corporeity * 0.2 + petAbilityPoint.AbilityPointSln[petAbilityPoint.SlnNow].Stamina * 0.6)* petLevelData.DefendPower);
            return petStatusDTO;
        }
        /// <summary>
        ///  获得所有被动技能的加成
        /// </summary>
        /// <param name="skillid"></param>
        public void VerifyPetPassivitySkill(List<int>skillid,out List<PetStatusDTO> petAbilityStatus)
        {
            List<PetSkillData> passivitySkillList = new List<PetSkillData>();
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetSkillData>>(out var petSkillDataDict);
            petAbilityStatus = new List<PetStatusDTO>();
            var petstatusProp = GameManager.ReferencePoolManager.Spawn<PetStatusDTO>();
            var petstatusFixed = GameManager.ReferencePoolManager.Spawn<PetStatusDTO>();
            for (int i = 0; i < skillid.Count; i++)
            {
                if (!petSkillDataDict[skillid[i]].ISActiveSkill&& petSkillDataDict[skillid[i]].AttributionType.Count>0)
                {
                    passivitySkillList.Add(petSkillDataDict[skillid[i]]);
                }
            }

            for (int i = 0; i < passivitySkillList.Count; i++)
            {
                for (int j = 0; j < passivitySkillList[i].AttributionType.Count; j++)
                {
                    switch ((AttributionType)passivitySkillList[i].AttributionType[j])
                    {
                        case AttributionType.HP:
                            petstatusProp.PetHP += passivitySkillList[i].Percentage[j];
                            petstatusFixed.PetHP += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.MP:
                            petstatusProp.PetMP += passivitySkillList[i].Percentage[j];
                            petstatusFixed.PetMP += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.ShenHun:
                            petstatusProp.PetShenhun += passivitySkillList[i].Percentage[j];
                            petstatusFixed.PetShenhun += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.AttackPhysical:
                            petstatusProp.AttackPhysical += passivitySkillList[i].Percentage[j];
                            petstatusFixed.AttackPhysical += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.DefendPhysical:
                            petstatusProp.DefendPhysical += passivitySkillList[i].Percentage[j];
                            petstatusFixed.DefendPhysical += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.AttackPower:
                            petstatusProp.AttackPower += passivitySkillList[i].Percentage[j];
                            petstatusFixed.AttackPower += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.DefendPower:
                            petstatusProp.DefendPower += passivitySkillList[i].Percentage[j];
                            petstatusFixed.DefendPower += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.AttackSpeed:
                            petstatusProp.AttackSpeed += passivitySkillList[i].Percentage[j];
                            petstatusFixed.AttackSpeed += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.PhysicalCritProb:
                            petstatusProp.PhysicalCritProb += passivitySkillList[i].Percentage[j];
                            petstatusFixed.PhysicalCritProb += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.MagicCritProb:
                            petstatusProp.MagicCritProb += passivitySkillList[i].Percentage[j];
                            petstatusFixed.MagicCritProb += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.ReduceCritProb:
                            petstatusProp.ReduceCritProb += passivitySkillList[i].Percentage[j];
                            petstatusFixed.ReduceCritProb += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.PhysicalCritDamage:
                            petstatusProp.PhysicalCritDamage += passivitySkillList[i].Percentage[j];
                            petstatusFixed.PhysicalCritDamage += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.MagicCritDamage:
                            petstatusProp.MagicCritDamage += passivitySkillList[i].Percentage[j];
                            petstatusFixed.MagicCritDamage += passivitySkillList[i].FixedValue[j];
                            break;
                        case AttributionType.ReduceCritDamage:
                            petstatusProp.ReduceCritDamage += passivitySkillList[i].Percentage[j];
                            petstatusFixed.ReduceCritDamage += passivitySkillList[i].FixedValue[j];
                            break;
                        default:
                            break;
                    }
                }
            }

            petAbilityStatus.Add(petstatusProp);
            petAbilityStatus.Add(petstatusFixed);
        }
        /// <summary>
        /// 加成计算资质后计算加点后的总加成
        /// </summary>
        /// <param name="petStatus"></param>
        /// <param name="petAbilityStatus"></param>
        /// <param name="petStatusTemp"></param>
        public void StatusAddition(PetStatus petStatus, List<PetStatusDTO> petAbilityStatus,out PetStatusDTO petStatusTemp)
        {
            petStatusTemp= GameManager.ReferencePoolManager.Spawn<PetStatusDTO>();
            petStatusTemp.AttackPhysical =(petStatus.AttackPhysical ) + petAbilityStatus[0].AttackPhysical / 100 + petAbilityStatus[1].AttackPhysical;
            petStatusTemp.AttackPower = (petStatus.AttackPower) + petAbilityStatus[0].AttackPower / 100 + petAbilityStatus[1].AttackPower;
            petStatusTemp.AttackSpeed =  (petStatus.AttackSpeed ) + petAbilityStatus[0].AttackSpeed / 100 + petAbilityStatus[1].AttackSpeed;
            petStatusTemp.DefendPhysical = (petStatus.DefendPhysical ) + petAbilityStatus[0].DefendPhysical / 100 + petAbilityStatus[1].DefendPhysical;
            petStatusTemp.DefendPower = (petStatus.DefendPower ) + petAbilityStatus[0].DefendPower / 100 + petAbilityStatus[1].DefendPower;
            petStatusTemp.ExpLevelUp =  (petStatus.ExpLevelUp ) + petAbilityStatus[0].ExpLevelUp / 100 + petAbilityStatus[1].ExpLevelUp;
            petStatusTemp.MagicCritDamage =  (petStatus.MagicCritDamage ) + petAbilityStatus[0].MagicCritDamage / 100 + petAbilityStatus[1].MagicCritDamage;
            petStatusTemp.MagicCritProb = (petStatus.MagicCritProb ) + petAbilityStatus[0].MagicCritProb / 100 + petAbilityStatus[1].MagicCritProb;
            petStatusTemp.PetHP = (petStatus.PetHP ) + petAbilityStatus[0].PetHP / 100 + petAbilityStatus[1].PetHP;
            petStatusTemp.PetMaxHP = petStatusTemp.PetHP;
            petStatusTemp.PetMP =  (petStatus.PetMP) + petAbilityStatus[0].PetMP / 100 + petAbilityStatus[1].PetMP;
            petStatusTemp.PetMaxMP = petStatusTemp.PetMP;
            petStatusTemp.PetShenhun = (petStatus.PetShenhun )+ petAbilityStatus[0].PetShenhun / 100 + petAbilityStatus[1].PetShenhun;
            petStatusTemp.PetMaxShenhun = petStatusTemp.PetShenhun;
            petStatusTemp.PhysicalCritDamage =  (petStatus.PhysicalCritDamage ) + petAbilityStatus[0].PhysicalCritDamage / 100 + petAbilityStatus[1].PhysicalCritDamage;
            petStatusTemp.PhysicalCritProb = (petStatus.PhysicalCritProb) + petAbilityStatus[0].PhysicalCritProb / 100 + petAbilityStatus[1].PhysicalCritProb;
            petStatusTemp.ReduceCritDamage = (petStatus.ReduceCritDamage ) + petAbilityStatus[0].ReduceCritDamage / 100 + petAbilityStatus[1].ReduceCritDamage;
            petStatusTemp.ReduceCritProb =  (petStatus.ReduceCritProb ) + petAbilityStatus[0].ReduceCritProb / 100 + petAbilityStatus[1].ReduceCritProb;
        }

        /// <summary>
        /// 资质DO转换DTO
        /// </summary>
        /// <param name="petAptitude"></param>
        /// <param name="petAptitudeDTO"></param>
        /// <returns></returns>
        public PetAptitude AssignSameFieldValue(PetAptitude petAptitude, PetAptitudeDTO petAptitudeDTO)
        {
            petAptitude.AttackphysicalAptitude = petAptitudeDTO.AttackphysicalAptitude;
            petAptitude.AttackpowerAptitude = petAptitudeDTO.AttackpowerAptitude;
            petAptitude.AttackspeedAptitude = petAptitudeDTO.AttackspeedAptitude;
            petAptitude.DefendphysicalAptitude = petAptitudeDTO.DefendphysicalAptitude;
            petAptitude.DefendpowerAptitude = petAptitudeDTO.DefendpowerAptitude;
            petAptitude.HPAptitude = petAptitudeDTO.HPAptitude;
            petAptitude.Petaptitudecol = petAptitudeDTO.Petaptitudecol;
            petAptitude.SoulAptitude = petAptitudeDTO.SoulAptitude;
            petAptitude.PetID = petAptitudeDTO.PetID;
            petAptitude.PetAptitudeDrug = Utility.Json.ToJson(petAptitudeDTO.PetAptitudeDrug);
            return petAptitude;
        }
        public PetAptitudeDTO AssignSameFieldValue(PetAptitudeDTO petAptitudeDTO, PetAptitude petAptitude)
        {
            petAptitudeDTO.AttackphysicalAptitude = petAptitude.AttackphysicalAptitude;
            petAptitudeDTO.AttackpowerAptitude = petAptitude.AttackpowerAptitude;
            petAptitudeDTO.AttackspeedAptitude = petAptitude.AttackspeedAptitude;
            petAptitudeDTO.DefendphysicalAptitude = petAptitude.DefendphysicalAptitude;
            petAptitudeDTO.DefendpowerAptitude = petAptitude.DefendpowerAptitude;
            petAptitudeDTO.HPAptitude = petAptitude.HPAptitude;
            petAptitudeDTO.Petaptitudecol = petAptitude.Petaptitudecol;
            petAptitudeDTO.SoulAptitude = petAptitude.SoulAptitude;
            petAptitudeDTO.PetID = petAptitude.PetID;
            petAptitudeDTO.PetAptitudeDrug = Utility.Json.ToObject<Dictionary<int,int>>(petAptitude.PetAptitudeDrug);
            return petAptitudeDTO;
        }
    }
}
