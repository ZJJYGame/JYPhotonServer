﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 秘术的ID
    /// 秘术基础数值列表
    /// </summary>
    [Serializable]
    [ConfigData]
    public class MiShuData
    {
        public int MishuID { get; set; }
        public List<MishuSkillData> mishuSkillDatas { get; set; }
    }
    /// <summary>
    /// 秘术详细数值类
    /// yzq添加
    /// </summary>
    [Serializable]
    [ConfigData]
    public class MishuSkillData 
    {
        public int MishuFloor { get; set; }
        public int NeedLevelID { get; set; }
        public byte MishuType { get; set; }
        public byte MishuQuality { get; set; }
        public byte MishuProperty { get; set; }
        public List<int> SkillArrayOne { get; set; }
        public List<int> SkillArrayTwo { get; set; }
        public int ExpLevelUp { get; set; }
        public int RoleHP { get; set; }
        public int RoleMP { get; set; }
        public int RoleSoul { get; set; }
        public float BestBlood { get; set; }
        public int AttactPhysical { get; set; }
        public int DefendPhysical { get; set; }
        public int AttactPower { get; set; }
        public int DefendPower { get; set; }
        public int AttactSpeed { get; set; }
        public float MoveSpeed { get; set; }
        public float PhysicalCritProb { get; set; }
        public float MagicCritProb { get; set; }
        public float ReduceCritProb { get; set; }
        public int PhysicalCritDamage { get; set; }
        public int MagicCritDamage { get; set; }
        public int ReduceCritDamage { get; set; }
        public int RolePopularity { get; set; }
        public int ValueHide { get; set; }
        public int GongfaLearnSpeed { get; set; }
        public int MishuLearnSpeed { get; set; }
    }
}


