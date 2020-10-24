using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 技能目标类型
    /// </summary>
    public enum TargetType
    {
        TeamMate = 2,
        Enemy = 1,
        Self = 3,
    }
   /// <summary>
   /// 攻击目标的数量
   /// </summary>
    public enum TargetCount
    {
        One = 1,//单目标
        Two = 2,//双目标
        All = 3//群体
    }
    /// <summary>
    /// 伤害的类型
    /// </summary>
    public enum DamageType
    {
        Physic = 6,
        Magic = 7,
        ShenHun = 8
    }
    /// <summary>
    /// 移动的方式
    /// </summary>
    public enum Battle_MoveType
    {
        MoveToTarget = 0,
        Local = 1
    }
    /// <summary>
    /// 攻击的类型
    /// </summary>
    public enum AttackProcess_Type
    {
        /// <summary>
        /// 每段伤害一次性打完所有敌人
        /// </summary>
        SingleUse = 0,
        /// <summary>
        /// 分阶段打完伤害
        /// </summary>
        Staged = 1
    }

    /// <summary>
    /// 技能类型
    /// 1.攻击
    /// 2.回血
    /// 3.护盾
    /// 4.buffer
    /// 5.复活
    /// </summary>
    public enum Skill_Type
    {
        Attact,
        ReturnBlood,
        Shield,
        Buffer,
        Resurgence
    }

  
}
