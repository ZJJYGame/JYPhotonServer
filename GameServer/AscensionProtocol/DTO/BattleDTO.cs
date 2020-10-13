using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


    [Serializable]
    public class RoleBattleDataDTO: BattleDataBase
    {
        public virtual RoleStatusDTO RoleStatusDTO { get; set; }
    }
    [Serializable]
    public class PetBattleDataDTO: BattleDataBase
    {
        public int RoleId { get; set; }
        public virtual PetStatusDTO PetStatusDTO { get; set; }
    }
    [Serializable]
    public class EnemyBattleDataDTO: BattleDataBase
    {
        public virtual int GlobalId { get; set; }

        public virtual EnemyStatusDTO  EnemyStatusDTO { get; set; }
    }


    //TODO  敌人的属性  可以直接用 玩家的数据模型   建议不使用
    [Serializable]
    public class EnemyStatusDTO
    {
        public virtual int EnemyId { get; set; }
        public virtual string EnemyName { get; set; }
        public virtual int EnemyHP { get; set; }
        public virtual int EnemyMP { get; set; }
        public virtual int EnemyShenHun { get; set; }
        public virtual string EnemyDescribe { get; set; }
        public virtual string EnemyLevel { get; set; }
        public virtual int EnemyGig_Level { get; set; }
        public virtual float EnemyAttact_Speed { get; set; }
        public virtual int EnemyAttact_Physical { get; set; }
        public virtual int EnemyDefence_Physical { get; set; }
        public virtual int EnemyAttact_Power { get; set; }
        public virtual int EnemyDefend_Power { get; set; }
        public virtual int EnemyAttact_Soul { get; set; }
        public virtual int EnemyDefend_Soul { get; set; }
        public virtual int EnemyUp_Double { get; set; }
        public virtual int EnemyDown_Double { get; set; }
        public virtual int EnemyValue_Flow { get; set; }
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
    /// 战斗传输数据DTO   列表 ， 出手速度  是按照玩家speed 排列的
    /// </summary>
    public class BattleTransferDTO
    {
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
        public virtual RoleDTO.BattleCmd BattleCmd { get; set; }
        /// <summary>
        /// 目标值和  自身值 传递的都是一样的
        /// </summary>
        public class TargetInfoDTO
        {
            /// <summary>
            /// 全局id
            /// </summary>
            public virtual int GlobalId { get; set; }
            /// <summary>
            /// 目标id
            /// </summary>
            public virtual  int TargetID { get; set; }
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
            /// 目标Buff
            /// </summary>
            public virtual int TargetBuff { get; set; }
           

        }
        /// <summary>
        /// 所有玩家行动结束后结算护盾值
        /// </summary>
        public virtual Dictionary<int,int> RoleIdShieldValueDict { get; set; }
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