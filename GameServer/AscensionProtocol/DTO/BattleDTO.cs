using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static AscensionProtocol.DTO.BattleTransferDTO;
/*
* 战斗的映射
* Since : 2020 - 09 -22
* Author : xianren*/
namespace AscensionProtocol.DTO
{
    [Serializable]
    public class BattleInitDTO : DataTransferObject
    {
        //public virtual
        /*
         * 
         * 
         * 
         * 11. 攻击对象名字
         * 10.攻击伤害系数
         * 9.捕捉的
         * 8.逃跑的
         * 7.功法、秘术的DTO
         * 6.宠物的DTO
         * 5.法宝的DTO
         * 4.道具的DTO
         * 3.技能的DTO
         * 2.怪物的DTO
         * 1.队伍的DTO*/

        /// <summary>
        /// 当前房间ID
        /// </summary>
        public virtual int RoomId { get; set; }
        /// <summary>
        /// 倒计时秒
        /// </summary>
        public virtual int countDownSec { get; set; }


        /// <summary>
        /// 当前房间内战斗的回合数
        /// </summary>
        public virtual int roundCount { get; set; }
        /// <summary>
        /// 最大的回合数
        /// </summary>
        public virtual int maxRoundCount { get; set; }
        /// <summary>
        /// 所有参战对象的列表
        /// </summary>
        public virtual List<BattleDataBase> battleUnits { get; set; }
        /// <summary>
        /// 所有参战玩家的列表
        /// </summary>
        public virtual List<RoleBattleDataDTO> playerUnits { get; set; }
        /// <summary>
        /// 所有参战宠物的列表
        /// </summary>
        public virtual List<PetBattleDataDTO> petUnits { get; set; }
        /// <summary>
        /// 所有参战敌人的列表
        /// </summary>
        public virtual List<EnemyBattleDataDTO> enemyUnits { get; set; }
        /// <summary>
        /// 所有参战宠物的列表
        /// </summary>
        public virtual List<PetBattleDataDTO> enemyPetUnits { get; set; }
        /// <summary>
        /// buffer的列表
        /// </summary>
        public virtual List<BufferBattleDataDTO> bufferUnits { get; set; }

        public override void Clear()
        {
            throw new NotImplementedException();
        }
    }


    ///// <summary>
    ///// 战斗传输 初始化
    ///// </summary>
    /// 所有参战的列表
    /// </summary>

    /// <summary>
    /// 基类
    /// </summary>
    ///  [Serializable].
    [Serializable]
    public class BattleDataBase
    {
        /// <summary>
        /// 代表是全局id 
        /// </summary>
        public virtual int ObjectID { get; set; }
        /// <summary>
        /// AI 针对 的唯一id
        /// </summary>
        public virtual int ObjectId { get; set; }
        public virtual string ObjectName { get; set; }
        public virtual int ObjectHP { get; set; }
        public virtual int ObjectMP { get; set; }
        public virtual int ObjectSpeed { get; set; }
    }
    /// <summary>
    /// 战斗的基类接口
    /// </summary>
        /*
         * 1.HitRate(命中率)
         * 2.PhysicalDamage(物理暴击伤害)
         * 3.SpellCrit(法术暴击伤害)
         * 4.AmplifyDamage(伤害加深)
         * 5.DamageReduction(伤害减免)
         * 6.IgnoreDefense 忽视防御
         * 7.DamageFluctuations 伤害波动
         * 8.BasalEvasionRate 基础闪避率
         * 9.PhysicalEvasionRate 物理闪避率
         * 10.SpellEvasionRate 法术闪避率
         * 11.PhysicalRate 物理暴击率
         * 12.SpellRate 法术暴击率
         * 13.CriteRate 爆免率
         * 14.ReducedDamage 降爆伤害
         **/
    public interface IBattleDataBass
    {
          int HitRate { get; set; }
        int PhysicalDamage { get; set; }
        int SpellCrit { get; set; }
        int AmplifyDamage { get; set; }
        int DamageReduction { get; set; }
        int IgnoreDefense { get; set; }
        int DamageFluctuations { get; set; }
        int BasalEvasionRate { get; set; }
        int PhysicalEvasionRate { get; set; }
        int SpellEvasionRate { get; set; }
        int PhysicalRate { get; set; }
        int SpellRate { get; set; }
        int CriteRate { get; set; }
        int ReducedDamage { get; set; }
    }


    [Serializable]
    public class RoleBattleDataDTO : BattleDataBase, IBattleDataBass
    {
        public virtual RoleStatusDTO RoleStatusDTO { get; set; }
        public int HitRate { get; set; }
        public int PhysicalDamage { get; set; }
        public int SpellCrit { get; set; }
        public int AmplifyDamage { get; set; }
        public int DamageReduction { get; set; }
        public int IgnoreDefense { get; set; }
        public int DamageFluctuations { get; set; }
        public int BasalEvasionRate { get; set; }
        public int PhysicalEvasionRate { get; set; }
        public int SpellEvasionRate { get; set; }
        public int PhysicalRate { get; set; }
        public int SpellRate { get; set; }
        public int CriteRate { get; set; }
        public int ReducedDamage { get; set; }
    }
    [Serializable]
    public class PetBattleDataDTO : BattleDataBase
    {
        public int RoleId { get; set; }
        public virtual PetStatusDTO PetStatusDTO { get; set; }
    }
    [Serializable]
    public class EnemyBattleDataDTO : BattleDataBase
    {
        public virtual int GlobalId { get; set; }

        public virtual EnemyStatusDTO EnemyStatusDTO { get; set; }
    }


    //TODO  敌人的属性  可以直接用 玩家的数据模型   建议不使用
    [Serializable]
    public class EnemyStatusDTO
    {
        public virtual int EnemyId { get; set; }
        public virtual string EnemyName { get; set; }
        public virtual int EnemyHP { get; set; }
        public virtual int EnemyMaxHP { get; set; }
        public virtual int EnemyMP { get; set; }
        public virtual int EnemyMaxMP { get; set; }
        public virtual int EnemySoul { get; set; }
        public virtual int EnemyMaxSoul { get; set; }
        public virtual string EnemyDescribe { get; set; }
        public virtual string EnemyLevel { get; set; }
        public virtual int EnemyGig_Level { get; set; }
        public virtual float EnemyAttact_Speed { get; set; }
        public virtual int EnemyAttact_Physical { get; set; }
        public virtual int EnemyDefence_Physical { get; set; }
        public virtual int EnemyAttact_Power { get; set; }
        public virtual int EnemyDefend_Power { get; set; }
        public virtual int EnemyValue_Flow { get; set; }
        public virtual int EnemyPhysicalCritProb { get; set; }
        public virtual int EnemyMagicCritProb { get; set; }
        public virtual int EnemyReduceCritProb { get; set; }
        public virtual int EnemyPhysicalCritDamage { set; get; }
        public virtual int EnemyMagicCritDamage { get; set; }
        public virtual int EnemyReduceCritDamage { get; set; }
        public virtual int EnemyAlert_Area { get; set; }
        public virtual int EnemyMove_Speed { get; set; }
        public virtual int EnemyValue_Hide { get; set; }
        public virtual int EnemyBest_Blood { get; set; }
        public virtual List<int> EnemySkill_Array { get; set; }
        public virtual List<int> EnemyDrop_Array { get; set; }
        public virtual List<int> EnemyDrop_Rate { get; set; }
        public virtual string EnemyMonster_Icon { get; set; }
        public virtual string EnemyMoster_Model { get; set; }
        public virtual int EnemyPet_ID { get; set; }
        public virtual int EnemyPet_Level_ID { get; set; }
    }

    /// <summary>
    /// buffer 实体对象 DTO
    /// </summary>
    [Serializable]
    public class BufferBattleDataDTO
    {
        public virtual int RoleId { get; set; }
        public virtual BufferData BufferData { get; set; }
    }
  
    [Serializable]
    public class BufferData
    {
        public virtual int bufferId { get; set; }
        public virtual int RoundNumber{ get; set; }
    }







    #region  战斗传输数据DTO


    /// <summary>
    /// 战斗传输数据DTO   列表 ， 出手速度  是按照玩家speed 排列的
    /// </summary>
    public class BattleTransferDTO
    {
        /// <summary>
        /// 控制每回合的时间
        /// </summary>
        //public virtual TimerManager timer { get; set; }
        public virtual BattleTransferDTO petBattleTransferDTO { get; set; }

        /// <summary>
        /// 是否结束
        /// </summary>
        public bool isFinish { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 客户端目标id
        /// </summary>
        public int ClientCmdId { get; set; }
        /// <summary>
        /// 目标行为信息 列表：
        /// </summary>
        public virtual List<TargetInfoDTO> TargetInfos { get; set; }
        ///// <summary>
        ///// 自身行为信息 列表
        ///// </summary>
        //public virtual List<TargetInfoDTO> OwnInfos { get; set; }
        /// <summary>
        /// 触发技能指令
        /// </summary>
        public virtual SkillReactionCmd SendSkillReactionCmd { set; get; }
        /// <summary>
        /// 触发技能数值
        /// </summary>
        public virtual int SkillReactionValue { get; set; }
        /// <summary>
        /// 每回合战斗指令
        /// </summary>
        public virtual BattleCmd BattleCmd { get; set; }
        /// <summary>
        /// 目标值和  自身值 传递的都是一样的
        /// </summary>
       
        /// <summary>
        /// 所有玩家行动结束后结算护盾值
        /// </summary>
        public virtual Dictionary<int, int> RoleIdShieldValueDict { get; set; }
        #region 设计

        //所有玩家行动之前结算buff造成伤害
        /*
         buufid_1:{{roleid,damage},.....}
         buufid:_2{{roleid,damage},.....}
         */


        //列表，按玩家出手顺序排序
        //1. 指令类型
        //2. 指令id列表
        //行者ID
        //3.行为目标i信息  列表：
        /*目标信息集合：
         * 目标id
         * 目标血量伤害
         * 目标蓝量伤害
         * 目标神魂伤害 
         * 目标护盾值
         * 自身id
         * 自身血量伤害
         * 自身蓝量伤害
         * 自身神魂伤害 
         * 自身护盾值
         * 给目标施加buff
         * 
         *触发技能反应枚举（如反击，守护，闪避，反震，格挡等）
         *触发数值
         *
         */
        //给自身的buff

        //所有玩家行动结束buff造成伤害
        /*
         buufid_1:{{roleid,damage},.....}
         buufid:_2{{roleid,damage},.....}
         */
        //所有玩家行动结束后结算护盾值
        //Dict<(int)roleID,(int)护盾值>
        #endregion
    }
    #endregion

    public class TargetInfoDTO
    {
        /// <summary>
        /// 全局id
        /// </summary>
        public virtual int GlobalId { get; set; }
        /// <summary>
        /// 目标id
        /// </summary>
        public virtual int TargetID { get; set; }
        /// <summary>
        /// 目标血量伤害
        /// </summary>
        public virtual int TargetHPDamage { set; get; }

        /// <summary>
        /// 目标蓝量伤害
        /// </summary>
        public virtual int TargetMPDamage { set; get; }
        /// <summary>
        /// 目标神魂伤害
        /// </summary>
        public virtual int TargetShenHunDamage { set; get; }
        /// <summary>
        /// 目标护盾值
        /// </summary>
        public virtual int TargetShieldVaule { get; set; }
        /// <summary>
        ///添加 目标Buff
        /// </summary>
        public virtual List<BufferBattleDataDTO> AddTargetBuff { get; set; }
        /// <summary>
        ///移除 目标Buff
        /// </summary>
        public virtual List<int> RemoveTargetBuff { get; set; }

        public virtual List<BattleBuffDTO> battleBuffDTOs { get; set; }
    }


    /// <summary>
    /// 战斗回合 前后给你的
    /// </summary>
    public class BattleBuffDTO
    {
        public int index { get; set; }
        public virtual int bufferId { get; set; }
        public int BuffValue { get; set; }
        public int TriggerId { get; set; }
        public int TargetId { get; set; }
        public List<BufferData> bufferData { get; set; }
        public  List<BattleBuffDTO> battleBuffDTOs { get; set; }

    }

}

/// <summary>
/// 触发技能反应列表
/// </summary>
public enum SkillReactionCmd
{
    /// <summary>
    /// 反击
    /// </summary>
    BeatBack,
    /// <summary>
    /// 守护
    /// </summary>
    Guard,
    /// <summary>
    /// 闪避
    /// </summary>
    Dodge,
    /// <summary>
    /// 反震
    /// </summary>
    Shock,
    /// <summary>
    /// 格挡
    /// </summary>
    Parry
}
