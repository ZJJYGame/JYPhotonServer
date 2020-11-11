/*
*Author : xianrenZhang
*Since :	2020-04-28
*Description :  角色属性 
*/
using System.Collections.Generic;
using System;
namespace AscensionServer.Model
{
    [Serializable]
    public class RoleStatus:DataObject
    {
        public RoleStatus()
        {
            FreeAttributes = 0;
            RoleHP = 488;
            RoleMaxHP = 100;
            RoleMP = 300;
            RoleMaxMP = 100;
            RoleSoul = 100;
            RoleMaxSoul = 100;
            BestBlood = 100;
            BestBloodMax = 100;
            AttackSpeed = 100;
            AttackPhysical = 100;
            DefendPhysical = 100;
            AttackPower = 100;
            DefendPower = 100;
            PhysicalCritProb = 100;
            MagicCritProb = 100;
            ReduceCritProb = 100;
            PhysicalCritDamage = 100;
            MagicCritDamage = 100;
            ReduceCritDamage = 100;
            MoveSpeed = 100;
            RolePopularity = 100;
            RoleMaxPopularity = 100;
            ValueHide = 100;
        }       /// <summary>
        /// 角色id
        /// 角色属性点
        /// 角色血量
        /// 角色最大血量
        /// 角色真元
        /// 角色最大真元
        /// 角色神魂
        /// 角色最大神魂
        /// 角色精血
        /// 攻击速度
        /// 物理攻击
        /// 物理防御
        /// 法术攻击
        /// 法术防御
        /// 物理暴击几率
        /// 法术暴击几率
        /// 暴免率
        /// 物理暴击伤害
        /// 法术暴击伤害
        /// 防暴伤害
        /// 移动速度
        /// 业果值
        /// 最大业果值(最大值为固定值及表中初始数值)
        /// 隐匿值
        /// </summary>
        public virtual int RoleID { get; set; }
        public virtual int FreeAttributes { get; set; }
        public virtual int RoleHP { get; set; }
        public virtual int RoleMaxHP { get; set; }
        public virtual int RoleMP { get; set; }
        public virtual int RoleMaxMP { get; set; }
        public virtual int RoleSoul { get; set; }
        public virtual int RoleMaxSoul { get; set; }
        public virtual short BestBlood { get; set; }
        public virtual short BestBloodMax { get; set; }
        public virtual int AttackSpeed { get; set; }
        public virtual int AttackPhysical { get; set; }
        public virtual int DefendPhysical { get; set; }
        public virtual int AttackPower { get; set; }
        public virtual int DefendPower { get; set; }
        public virtual int PhysicalCritProb { get; set; }
        public virtual int MagicCritProb { get; set; }
        public virtual int ReduceCritProb { get; set; }
        public virtual int PhysicalCritDamage { get; set; }
        public virtual int MagicCritDamage { get; set; }
        public virtual int ReduceCritDamage { get; set; }
        public virtual int MoveSpeed { get; set; }
        public virtual int RolePopularity { get; set; }
        public virtual int RoleMaxPopularity { get; set; }
        public virtual int ValueHide { get; set; }
        public override void Clear()
        {
            
        }
    }
}
