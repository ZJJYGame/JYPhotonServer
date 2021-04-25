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
        /// 妖灵精魄
        /// </summary>
        DemonicSoul=8,
        /// <summary>
        /// 学习技能
        /// </summary>
        PetStudtSkill=9,
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
        SwitchPetAbilitySln=12
    }
}
