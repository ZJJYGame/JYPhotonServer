using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using NHibernate.Linq.Clauses;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// 针对 指令类型 具体处理
/// </summary>
namespace AscensionServer
{
    public partial class ServerBattleManager
    {
        /// <summary>
        /// 判断技能功法秘术是不是存在json 数据表格里
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public bool IsToSkillForm(int targetId)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillGongFaDatas>>(out var skillGongFaDict);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillMiShuDatas>>(out var skillMiShuDict);
            if (skillGongFaDict.ContainsKey(targetId) || skillMiShuDict.ContainsKey(targetId))
                return true;
            return false;
        }

        /// <summary>
        /// 返回 一个存在的技能对象
        /// </summary>
        /// <param name="targerId"></param>
        /// <returns></returns>
        public object SkillFormToSkillObject(int targerId )
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillGongFaDatas>>(out var skillGongFaDict);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillMiShuDatas>>(out var skillMiShuDict);
            if (skillGongFaDict.ContainsKey(targerId))
                return  skillGongFaDict[targerId];
            if (skillMiShuDict.ContainsKey(targerId))
                return skillMiShuDict[targerId];
            return null;
        }

        /// <summary>
        /// 不同技能行为的Cmd
        /// </summary>
        /// <param name="battleCmd"></param>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        public void SkillActionDifferentCmd(BattleCmd battleCmd,int roleId,int roomId)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = RoundServerToClient();
            opData.OperationCode = (byte)OperationCode.SyncBattleTransfer;
            #region ob
            /*
            switch (battleCmd)
            {
                case BattleCmd.Init:
                    break;
                case BattleCmd.Prepare:
                    break;
                case BattleCmd.PropsInstruction:
                    break;
                case BattleCmd.SkillInstruction:
                    opData.OperationCode = (byte)OperationCode.SyncBattleTransfer;
                    break;
                case BattleCmd.RunAwayInstruction:
                    opData.OperationCode = (byte)OperationCode.SyncBattleMessageRunAway;
                    break;
                case BattleCmd.PerformBattleComplete:
                    break;
                case BattleCmd.MagicWeapon:
                    break;
                case BattleCmd.CatchPet:
                    break;
                case BattleCmd.SummonPet:
                    break;
                case BattleCmd.Tactical:
                    break;
            }*/
            #endregion
            GameManager.CustomeModule<RoleManager>().SendMessage(roleId, opData);
            GameManager.CustomeModule<ServerBattleManager>().RecordRoomId.Enqueue(roomId);
            GameManager.CustomeModule<ServerBattleManager>().TimestampBattleEnd(roomId);
        }
    }

    public enum SkillStatus
    {
        /// <summary>
        /// 攻击的数量
        /// </summary>
        Attack_Number,
        /// <summary>
        /// 伤害系数
        /// </summary>
        Attack_Factor,
        /// <summary>
        /// 攻击模式
        /// </summary>
        AttackProcess_Type
    }
}
