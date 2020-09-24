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
    public class BattleDTO : DataTransferObject
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
        public virtual List<RoleDTO> playerUnits { get; set; }
        /// <summary>
        /// 所有参战宠物的列表
        /// </summary>
        public virtual List<PetDTO> petUnits { get; set; }



        /// <summary>
        /// 所有参战敌人的列表
        /// </summary>
        /*
       public virtual List<int> enemyUnits { get; set; }
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

        public virtual string RoleName { set; get; }



        public override void Clear()
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 战斗传输 
    /// </summary>
    interface BattleTransferData
    {
        string ObjectName { get; set; }
    }

    public class RoleBattleDataDTO: BattleTransferData
    {
        public string ObjectName { get; set ; }

        public virtual RoleStatusDTO RoleStatusDTO { get; set; }
    }

    public class PetBattleDataDTO: BattleTransferData
    {
        public string ObjectName { get; set; }
        public virtual PetStatusDTO PetStatusDTO { get; set; }
    }

    public class EnemyBattleDataDTO: BattleTransferData
    {
        public string ObjectName { get; set; }
        public virtual EnemyStatusDTO  EnemyStatusDTO { get; set; }
    }

    //TODO  敌人的属性
    public class EnemyStatusDTO
    {

    }


}
