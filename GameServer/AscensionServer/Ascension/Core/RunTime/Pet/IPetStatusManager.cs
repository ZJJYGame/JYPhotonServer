using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public interface IPetStatusManager:IModuleManager
    {
        void ResetPetStatus(Pet pet, PetAptitude petAptitudeObj, out PetStatus petStatusTemp);
        /// <summary>
        /// 重置宠物资质
        /// </summary>
        /// <param name="petID"></param>
        /// <param name="petAptitudeObj"></param>
        void ResetPetAptitude(int petID, out PetAptitude petAptitudeObj);
        /// <summary>
        /// 新增新宠物
        /// </summary>
        /// <param name="petID"></param>
        /// <param name="petName"></param>
        /// <param name="rolePet"></param>
        void InitPet(int petID, string petName, int  roleid);
        /// <summary>
        /// 洗练宠物重置技能
        /// </summary>
        List<int> RestPetSkill(List<int> skillList);
        /// <summary>
        /// 技能书使用顶替技能
        /// </summary>
        List<int> RandomSkillRemoveAdd(List<int> skillList, int skillid);
        /// <summary>
        /// 正态分布随机数
        /// </summary>

        double AverageRandom(double min, double max);

        int NormalRandom2(int min, int max);
        #region Init
        /// <summary>
        /// 宠物设置出战
        /// </summary>
        /// <param name="rolePetDTO"></param>
        /// <param name="rolePet"></param>
        void RolePetSetBattle(RolePetDTO rolePetDTO);
 

        /// <summary>
        /// 获得角色所有宠物
        /// </summary>
        /// <param name="rolePet"></param>
        void GetRoleAllPetS2C( RolePetDTO rolePetDTO);

        /// <summary>
        /// 移除角色宠物
        /// </summary>
        /// <param name="rolePet"></param>
        /// <param name="rolePetDTO"></param>
        void RemoveRolePet(RolePetDTO rolePetDTO);
 
        /// <summary>
        /// 获得宠物所有属性
        /// </summary>
        void GetPetAllCompeleteStatus(int petid, int roleid);
 

        #region 洗练宠物
  void ResetPetAllStatus(int petID, string petName, RolePet rolePet);
        #endregion

        #region 宠物加点
        /// <summary>
        /// 更新宠物加点方案
        /// </summary>
       // void UpdataPetAbilityPoint(PetAbilityPoint petAbilityPoint, PetCompleteDTO petCompleteDTO, NHCriteria nHCriteria);
 

        void RenamePointSln(int roleid, PetAbilityPointDTO abilityPointDTO);

        void UpdatePointSln(int roleid, PetAbilityPointDTO abilityPointDTO);
  
        void UlockPointSln(int roleid, PetAbilityPointDTO abilityPointDTO);

        void ResetAbilityPoint(int roleid, PetAbilityPointDTO petAbilityPoint);
 
        #endregion

        #region 使用丹药
        /// <summary>
        /// 宠物培养丹药区分
        /// </summary>
        /// <param name="drugID"></param>
        /// <param name="nHCriteria"></param>
        /// <param name="pet"></param>
        void PetCultivate(int drugID,  int  petid, int roleid);
        /// <summary>
        /// 验证丹药作用
        /// </summary>
        bool VerifyDrugEffect(int drugid, Pet pet, int roleid);
 
        /// <summary>
        /// 增加经验
        /// </summary>
        /// <param name="drugData"></param>
        /// <param name="pet"></param>
        void PetExpDrug(DrugData drugData, int itemid, Pet pet, int roleid);
   
        /// <summary>
        /// 增加资质
        /// </summary>
        void PetAtitudeDrug(DrugData drugData, int itemid, Pet pet, int roleid);

        #endregion

        #region 宠物学习技能
        void PetStudySkill(int bookid, int petid, int roleid);

        #endregion

        #region 妖灵精魄
        
        void UnEquipDemonicSoul(int soulid, int petid, int roleid);

        void AddDemonicSoul(int soulid, out List<int> getSkillList);
     
        void PetRename(int roleid, PetDTO petDTO);


        void EquipDemonicSoul(int roleid, int useitemid, int petid);
       
        #endregion

        #region 宠物进阶

        void PetEvolution(int roleid, int petid, int itemid);
        #endregion

        #region 宠物洗练
        void PetStatusRestoreDefault(int itemid, int roleid, int petid);
        #endregion
        #endregion

        #region Message
        void S2CRolePetOperateSuccess(int roleid, string s2cMessage);
 

        void S2CRoleAddPetSuccess(int roleid);

        void S2CPetSetBattle(int roleid, string s2cMessage, ReturnCode returnCode);
     
        void S2CRemoveRolePet(int roleid, string s2cMessage, ReturnCode returnCode);
   

        #region 宠物数据发送待更换
        void S2CPetAllStatus(int roleid, string s2cMessage);
      
        void S2CPetAbilityPoint(int roleid, string s2cMessage, ReturnCode returnCode);
    
        void S2CPetStudySkill(int roleid, string s2cMessage, ReturnCode returnCode);
  

        void S2CPetRename(int roleid, string s2cMessage, ReturnCode returnCode);
    

        void S2CPetEquipDemonic(int roleid, string s2cMessage, ReturnCode returnCode);
       
        void S2CPetCultivate(int roleid, string s2cMessage, ReturnCode returnCode);
   
        #endregion

        void S2CPetDrugRefresh(int roleid, string s2cMessage);
        #endregion
    }
}


