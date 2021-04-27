using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol
{
   public enum RolePetOpCode : byte
    {
        /// <summary>
        /// 获得角色宠物
        /// </summary>
        GetRolePet=1,
        /// <summary>
        /// 移除宠物
        /// </summary>
        RemovePet=2,
        /// <summary>
        /// 添加
        /// </summary>
        AddPet=3,
        /// <summary>
        /// 设置宠物出战
        /// </summary>
        SetBattle=4,
        /// <summary>
        /// 重置宠物加点
        /// </summary>
        ResetPetAbilitySln = 5,
        /// <summary>
        /// 重置宠物属性
        /// </summary>
        ResetPetStatus = 6,
        /// <summary>
        /// 宠物进阶
        /// </summary>
        PetEvolution=7,
        /// <summary>
        /// 装备妖灵精魄
        /// </summary>
        EquipDemonicSoul=8,
        /// <summary>
        /// 学习技能
        /// </summary>
        PetStudySkill=9,
        /// <summary>
        /// 宠物培养
        /// </summary>
        PetCultivate=10,
        /// <summary>
        /// 获得宠物属性
        /// </summary>
        GetPetStatus = 11,
        /// <summary>
        /// 切换加点方案
        /// </summary>
        SwitchPetAbilitySln=12,
        /// <summary>
        /// 设置加点
        /// </summary>
        SetAdditionPoint=13,
        /// <summary>
        /// 宠物改名
        /// </summary>
        RenamePet=14,
        /// <summary>
        /// 卸下妖灵精魄
        /// </summary>
        RemoveDemonicSoul=15,
        /// <summary>
        /// 解锁加点方案
        /// </summary>
        UnlockPetAbilitySln=16,
        /// <summary>
        /// 修改加点方案名称
        /// </summary>
        RenamePetAbilitySln=17,
        /// <summary>
        /// 刷新宠物丹药
        /// </summary>
        PetDrugFresh = 18,
    }
}
