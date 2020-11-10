﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    ///  2020.11.06 09.50
    /// </summary>
    /// 技能系统 对应的实体类
    [Serializable]
    [ConfigData]
    public class BattleSkillData
    {
        public string name;
        public int id;
        public string describe;
        /// <summary>
        /// 技能目标数
        /// </summary>
        public int TargetNumber;
        /// <summary>
        /// 伤害加成
        /// </summary>
        public int damageAddition;
        /// <summary>
        /// 暴击率
        /// </summary>
        public int critProp;
        /// <summary>
        /// 暴击伤害
        /// </summary>
        public int critDamage;
        /// <summary>
        /// 忽视防御
        /// </summary>
        public int ignoreDefensive;
        public int cd;
        /// <summary>
        /// 0=>近战,1=>远程
        /// </summary>
        //[Header("是=>近战,否=>远程")]
        public bool IsCloseAttack;
        //[Header("技能使用条件")]
        public BattleSkillUseCondition battleSkillUseCondition;
        public BattleSkillFactionType battleSkillFactionType;
        public BattleSkillTargetType battleSkillTargetType;
        public BattleSkillActionType battleSkillActionType;
        public AttackProcess_Type attackProcessType;
        /// <summary>
        /// 伤害系数列表
        /// </summary>
        //[Header("伤害系数列表")]
        public List<BattleSkillDamageNumData> battleSkillDamageNumDataList;
        //[Header("技能添加buff列表")]
        public List<BattleSkillAddBuffData> battleSkillAddBuffList;
        //[Header("技能移除buff列表")]
        public List<BattleSkillRemoveBuffData> battleSkillRemoveBuffDataList;
        //[Header("技能消耗设置")]
        public BattleSkillCostData battleSkillCostData;
        //[Header("技能触发事件列表")]
        public List<BattleSkillEventData> battleSkillEventDataList;
    }
    /// <summary>
    /// 战斗技能使用条件 
    /// </summary>
    [Serializable]
    public class BattleSkillUseCondition
    {
        public BattleSkillUseConditionType battleSkillUseConditionType;
        public int value;
    }
    /// <summary>
    /// 一段伤害具体数据
    /// </summary>
    [Serializable]
    public class BattleSkillDamageNumData
    {
        public BattleSkillDamageType battleSkillDamageType;
        /// <summary>
        /// 数值基础系数
        /// </summary>
        public int fixedNum;
        public List<BattleSkiilNumSourceData> baseNumSourceDataList;
        public List<BattleSkiilNumSourceData> extraNumSourceData;
    }
    [Serializable]
    public class BattleSkiilNumSourceData
    {
        public BattleSkillNumSourceType battleSkillNumSourceType;
        public int mulitity;
    }
    [Serializable]
    public class BattleSkillAddBuffData
    {
        public int buffId;
        public int round;
        /// <summary>
        /// 目标类型：0=>受击方,1=>自身
        /// </summary>
        //[Header("是=>受击方,否=>自身")]
        public bool TargetType;
        //[Header("buff数值")]
        public int buffValue;
        //[Header("基础概率列表")]
        public List<int> basePropList;
        //[Header("自身buff概率加成")]
        public BattleSkillAddBuffProbability selfAddBuffProbability;
        //[Header("目标buff概率加成")]
        public BattleSkillAddBuffProbability targetAddBuffProbability;
    }
    [Serializable]
    public class BattleSkillAddBuffProbability
    {
        public BattleSkillNumSourceType battleSkillBuffProbSource;
        public int multiplyPropValue;
        public int fixedPropValue;
        /// <summary>
        /// buff是加成或减少：0=>减少,1=>加成
        /// </summary>
        //[Header("buff是加成或减少：是=>加成,否=>减少")]
        public bool addOrReduce;
    }
    [Serializable]
    public class BattleSkillRemoveBuffData
    {
        public List<int> buffIdList;
        public int probability;
    }
    [Serializable]
    public class BattleSkillCostData
    {
        public BattleSkillCostType battleSkillCostType;
        public int percentValue;
        public int fixedValue;
    }
    [Serializable]
    public class BattleSkillEventData
    {
        //[Header("事件触发时机")]
        public BattleSkillEventTriggerTime battleSkillEventTriggerTime;
        //[Header("事件触发条件")]
        public BattleSkillEventTriggerCondition battleSkillEventTriggerCondition;
        //[Header("事件数据来源")]
        public BattleSkillEventTriggerNumSourceType battleSkillEventTriggerNumSourceType;
        //[Header("百分比数值")]
        public int conditionPercentNum;
        //[Header("固定数值")]
        public int conditionFixedNum;
        //[Header("触发事件类型")]
        public BattleSkillTriggerEventType battleSkillTriggerEventType;
        //[Header("事件数值，根据事件类型而定")]
        public int EventValue;
    }
    
}


#region 战斗中用到的枚举
/// <summary>
/// 战斗使用条件类型枚举
/// </summary>
public enum BattleSkillUseConditionType : byte
{
    None = 0,
    HealthGreaterThan = 1,
    HealthLessThan = 2,
    ShenHunGreaterThan = 3,
    ShenHunLessThan = 4,
    /// <summary>
    /// 不可重复召唤
    /// </summary>
    UnRepeatSummon = 5,
    /// <summary>
    /// 覆盖召唤
    /// </summary>
    ReplaceSummon = 6,
    /// <summary>
    /// 武器限定
    /// </summary>
    WeaponLimit = 7,
}
/// <summary>
/// 技能目标类型枚举
/// </summary>
public enum BattleSkillTargetType : byte
{
    All = 0,
    player = 1,
    Pet = 2,
    Self = 3,
    Summon = 4
}
/// <summary>
/// 技能阵营类型枚举
/// </summary>
public enum BattleSkillFactionType : byte
{
    Enemy = 0,
    TeamMate = 1,
}
public enum BattleSkillCostType : byte
{
    None = 0,
    Health = 1,
    ZhenYuan = 2,
    ShenHun = 3,
    JingXue = 4,
}
public enum BattleSkillDamageType : byte
{
    Physic = 0,
    Magic = 1,
    ShenHun = 2,
    Reality = 3,
}
public enum BattleSkillNumSourceType : byte
{
    MaxHealth = 0,
    NowHealth = 1,
    HasLostHealth = 2,
    MaxZhenYuan = 3,
    NowZhenYuan = 4,
    MaxShenHun = 5,
    NowShenHun = 6,
    PhysicAttack = 7,
    MagicAttack = 8,
    PhysicDefense = 9,
    MagicDefense = 10,
    AttackSpeed = 11,
}
public enum BattleSkillEventTriggerTime : byte
{
    BeforeAttack = 0,
    BehindAttack = 1,
}
public enum BattleSkillEventTriggerCondition : byte
{
    None = 0,
    Crit = 1,
    TargetPropertyUnder = 2,
    TargetPropertyOver = 3,
    SelfPropertyUnder = 4,
    SelfPropertyOver = 5,
}
public enum BattleSkillEventTriggerNumSourceType : byte
{
    Health = 0,
    PhysicDefense = 1,
    MagicDefense = 2,
    ShenHun = 3,
}
public enum BattleSkillTriggerEventType
{
    Skill = 0,
    Heal = 1,
    SuckBlood = 2,
    AddCrit = 3,
    AddDamage = 4,
    /// <summary>
    /// 增加穿透
    /// </summary>
    AddPierce = 5,
}

/// <summary>
/// 1.伤害
/// 2.回血
/// 3.复活
/// 4.召唤
/// </summary>
public enum BattleSkillActionType
{
    Damage = 0,
    Heal = 1,
    Resurrection = 2,
    Summon = 3,
}
#endregion
