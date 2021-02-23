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
        void InitPet(int petID, string petName, RolePet rolePet);
        /// <summary>
        /// 洗练宠物重置技能
        /// </summary>
        List<int> RestPetSkill(List<int> skillList);
        /// <summary>
        /// 技能书使用顶替技能
        /// </summary>
        List<int> RandomSkillRemoveAdd(List<int> skillList, int skillid);


        #region Init
        /// <summary>
        /// 宠物设置出战
        /// </summary>
        /// <param name="rolePetDTO"></param>
        /// <param name="rolePet"></param>
        void RolePetSetBattle(RolePetDTO rolePetDTO, RolePet rolePet, NHCriteria nHCriteriaRolePet, Pet pet);
 

        /// <summary>
        /// 获得角色所有宠物
        /// </summary>
        /// <param name="rolePet"></param>
        void GetRoleAllPet(RolePet rolePet, RolePetDTO rolePetDTO);
 
        /// <summary>
        /// 移除角色宠物
        /// </summary>
        /// <param name="rolePet"></param>
        /// <param name="rolePetDTO"></param>
        void RemoveRolePet(RolePet rolePet, RolePetDTO rolePetDTO, Pet pet);
 
        /// <summary>
        /// 获得宠物所有属性
        /// </summary>
        void GetPetAllCompeleteStatus(int petid, NHCriteria nHCriteria, int roleid, NHCriteria nHCriteriapet);
 

        #region 洗练宠物
  void ResetPetAllStatus(int petID, string petName, RolePet rolePet);
        #endregion

        #region 宠物加点
        /// <summary>
        /// 更新宠物加点方案
        /// </summary>
        void UpdataPetAbilityPoint(PetAbilityPoint petAbilityPoint, PetCompleteDTO petCompleteDTO, NHCriteria nHCriteria);
 

        void RenamePointSln(PetAbilityPoint petAbilityPoint, PetCompleteDTO petCompleteDTO);
    
        void UpdatePointSln(PetAbilityPoint petAbilityPoint, PetCompleteDTO petCompleteDTO);
  
        void UlockPointSln(PetAbilityPoint petAbilityPoint, NHCriteria nHCriteria, PetCompleteDTO petCompleteDTO);
 
        void ResetAbilityPoint(PetAbilityPoint petAbilityPoint);
 
        #endregion

        #region 使用丹药
        /// <summary>
        /// 宠物培养丹药区分
        /// </summary>
        /// <param name="drugID"></param>
        /// <param name="nHCriteria"></param>
        /// <param name="pet"></param>
        void PetCultivate(int drugID, NHCriteria nHCriteria, Pet pet, PetCompleteDTO petCompleteDTO);
        /// <summary>
        /// 验证丹药作用
        /// </summary>
        bool VerifyDrugEffect(int drugid, Pet pet, PetCompleteDTO petCompleteDTO);
 
        /// <summary>
        /// 增加经验
        /// </summary>
        /// <param name="drugData"></param>
        /// <param name="pet"></param>
        void PetExpDrug(DrugData drugData, int itemid, Pet pet, PetCompleteDTO petCompleteDTO);
   
        /// <summary>
        /// 增加资质
        /// </summary>
        void PetAtitudeDrug(DrugData drugData, int itemid, Pet pet, PetCompleteDTO petCompleteDTO);
 
        #endregion

        #region 宠物学习技能
        void PetStudySkill(int bookid, NHCriteria nHCriteria, Pet pet, PetCompleteDTO petCompleteDTO);
 
        #endregion

        #region 妖灵精魄
        void DemonicSoulHandler(PetCompleteDTO petCompleteDTO, Pet pet, NHCriteria nHCriteriarole);
        
        void UnEquipDemonicSoul(int soulid, Pet pet, int roleid, PetCompleteDTO petCompleteDTO);


        void AddDemonicSoul(int soulid, out List<int> getSkillList);
     
        void PetRename(Pet pet, PetCompleteDTO petCompleteDTO);


        void EquipDemonicSoul(int roleid, NHCriteria nHCriteriarole, PetCompleteDTO petCompleteDTO, Pet pet);
       
        #endregion

        #region 宠物进阶

        void PetEvolution(PetCompleteDTO petCompleteDTO, Pet pet, int itemid, NHCriteria nHCriteriarole);
        #endregion

        #region 宠物洗练
        void PetStatusRestoreDefault(int itemid, int roleid, PetCompleteDTO petCompleteDTO, Pet pet, NHCriteria nHCriteria);
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


