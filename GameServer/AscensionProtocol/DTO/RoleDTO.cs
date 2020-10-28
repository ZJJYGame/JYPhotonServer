using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class RoleDTO:DataTransferObject
    {
        public virtual int RoleID { get; set; }
        public virtual byte RoleFaction { get; set; }
        public virtual bool RoleGender { get; set; }
        public virtual int RoleTalent { get; set; }
        public virtual string RoleRoot { get; set; }
        public virtual string RoleName { get; set; }
        public virtual int RoleLevel { get; set; }
        #region team  需要完善
        public virtual TeamInstructions teamInstructions { get; set; }
        public virtual TeamDTO teamDTO { get; set; } 
        public enum TeamInstructions
        {
            CreateTeam = 1,
            JoinTeam = 2,
            ApplyTeam = 3,
            RefusedTeam = 4,
        }
        #endregion
        public virtual BattleInitDTO BattleInitDTO { get; set; }
        /// <summary>
        /// 战斗指令
        /// </summary>
        public virtual BattleCmd SendBattleCmd { get; set; }
        public virtual int CmdId { get; set; }
        /// <summary>
        /// 主要是针对 组队中的逃跑 应对 参不参与计算
        /// </summary>
        public virtual bool isBattle { get; set; }
        public override void Clear()
        {
            RoleID = -1;
            RoleFaction = 0;
            RoleTalent = 0;
            RoleRoot = null;
            RoleName = null;
            RoleLevel = 0;
            teamInstructions = 0;
            teamDTO = null;
            isBattle = true;
        }
        public override string ToString()
        {
            string str = "RoleID : "+ RoleID+ " ; RoleGender : "+ RoleGender+ " ; RoleName : "+ RoleName;
            return str;
        }
    }


    #region 战斗Cmd

    /// <summary>
    /// 战斗指令列表
    /// </summary>
    public enum BattleCmd
    {
        /// <summary>
        /// 初始化战斗
        /// </summary>
        Init,
        /// <summary>
        /// 准备战斗指令
        /// </summary>
        Prepare,
        /// <summary>
        /// 使用道具指令
        /// </summary>
        PropsInstruction,
        /// <summary>
        /// 使用技能指令
        /// </summary>
        SkillInstruction,
        /// <summary>
        /// 逃跑指令
        /// </summary>
        RunAwayInstruction,
        /// <summary>
        /// 战斗表演完成 
        /// </summary>
        PerformBattleComplete,
        /// <summary>
        /// 法宝指令
        /// </summary>
        MagicWeapon,
        /// <summary>
        /// 捕捉指令
        /// </summary>
        CatchPet,
        /// <summary>
        /// 召唤指令
        /// </summary>
        SummonPet,
        /// <summary>
        /// 阵法指令
        /// </summary>
        Tactical

    }
    #endregion
}
