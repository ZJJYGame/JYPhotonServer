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
        /// 所有参战对象的ID列表
        /// </summary>
        public virtual List<int> battleUnits { get; set; }
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



        /*
        /// <summary>
        /// 剩余参战对敌人的列表
        /// </summary>
        public virtual List<int> remainingEnemyUnits { get; set; }
        /// <summary>
        /// 剩余参战对玩家的列表
        /// </summary>
        public virtual List<RoleDTO> remainingPlayerUnits { get; set; }
        /// <summary>
        /// 剩余参战对宠物的列表
        /// </summary>
        public virtual List<PetDTO> remainingPetUnits { get; set; }
        */

        public override void Clear()
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 战斗传输 初始化
    /// </summary>
    interface IBattleTransferData
    {
        string ObjectName { get; set; }
    }

    public class RoleBattleDataDTO: IBattleTransferData
    {
        public string ObjectName { get; set ; }

        public virtual RoleStatusDTO RoleStatusDTO { get; set; }
    }

    public class PetBattleDataDTO: IBattleTransferData
    {
        public string ObjectName { get; set; }
        public virtual PetStatusDTO PetStatusDTO { get; set; }
    }

    public class EnemyBattleDataDTO: IBattleTransferData
    {
        public string ObjectName { get; set; }
        public virtual EnemyStatusDTO  EnemyStatusDTO { get; set; }
    }

    //TODO  敌人的属性  可以直接用 玩家的数据模型   建议不使用
    public class EnemyStatusDTO
    {

    }



    /// <summary>
    /// 战斗传输数据DTO
    /// </summary>
    public class BattleTransferDTO
    {

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
         * 自身血量伤害
         * 自身蓝量伤害
         * 自身神魂伤害 
         * 自身护盾值
         * 给目标施加buff
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

    }


}
