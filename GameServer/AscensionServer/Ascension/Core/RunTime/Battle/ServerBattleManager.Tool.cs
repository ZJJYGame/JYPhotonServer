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
/// 针对所有的工具
/// </summary>
namespace AscensionServer
{
    public partial class ServerBattleManager
    {
        /// <summary>
        /// 针对战斗中的随机数
        /// </summary>
        /// <param name="ov"></param>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public int RandomManager(int ov,int minValue,int maxValue)
        {
            var targetValue = new Random((int)DateTime.Now.Ticks + ov).Next(minValue, maxValue);
            return targetValue;
        }


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
        public object SkillFormToSkillObject(int targerId)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillGongFaDatas>>(out var skillGongFaDict);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, SkillMiShuDatas>>(out var skillMiShuDict);
            if (skillGongFaDict.ContainsKey(targerId))
                return skillGongFaDict[targerId];
            if (skillMiShuDict.ContainsKey(targerId))
                return skillMiShuDict[targerId];
            return null;
        }
        /// <summary>
        /// 针对 道具中得 丹药和符箓
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public object PropsInstrutionFormToObject(int targetId)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, DrugData>>(out var drugDict);
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, RunesData>>(out var runesDict);
            if (drugDict.ContainsKey(targetId))
                return drugDict[targetId];
            if (runesDict.ContainsKey(targetId))
                return runesDict[targetId];
            return null;
        }
        /// <summary>
        /// 针对 法宝
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public MagicWeaponData MagicWeaponFormToObject(int targetId)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MagicWeaponData>>(out var magicDict);
            if (magicDict.ContainsKey(targetId))
                return magicDict[targetId];
            return null;
        }





        /// <summary>
        /// 不同技能行为的Cmd
        /// </summary>
        /// <param name="battleCmd"></param>
        /// <param name="roleId"></param>
        /// <param name="roomId"></param>
        public void SkillActionDifferentCmd(BattleCmd battleCmd, int roleId, int roomId)
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


        #region 2020.11.06 11:29 
        /// <summary>
        /// 判断释放的技能是不是存在json中
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        public BattleSkillData SkillFormToObject(int targetId)
        {
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, BattleSkillData>>(out var battleSkillDict);
            if (battleSkillDict.ContainsKey(targetId))
                return battleSkillDict[targetId];
            return null;
        }

        /// <summary>
        /// 返回给客户端的计算伤害
        /// </summary>
        public List<TargetInfoDTO> ServerToClientResult(TargetInfoDTO targetInfo)
        {
            List<TargetInfoDTO> TargetInfosSet = new List<TargetInfoDTO>();
            TargetInfoDTO tempTrans = new TargetInfoDTO();
            tempTrans.TargetID = targetInfo.TargetID;
            tempTrans.TargetHPDamage = targetInfo.TargetHPDamage;
            tempTrans.TargetMPDamage = targetInfo.TargetMPDamage;
            tempTrans.TargetShenHunDamage = targetInfo.TargetShenHunDamage;
            tempTrans.TargetShieldVaule = targetInfo.TargetShieldVaule;
            tempTrans.AddTargetBuff = targetInfo.AddTargetBuff;
            tempTrans.RemoveTargetBuff = targetInfo.RemoveTargetBuff;
            TargetInfosSet.Add(tempTrans);
            return TargetInfosSet;
        }
        /// <summary>
        /// 计算多段伤害用的
        /// </summary>
        /// <param name="targetInfo"></param>
        /// <returns></returns>
        public TargetInfoDTO ServerToClientResults(TargetInfoDTO targetInfo)
        {
            TargetInfoDTO tempTrans = new TargetInfoDTO();
            tempTrans.TargetID = targetInfo.TargetID;
            tempTrans.TargetHPDamage = targetInfo.TargetHPDamage;
            tempTrans.TargetMPDamage = targetInfo.TargetMPDamage;
            tempTrans.TargetShenHunDamage = targetInfo.TargetShenHunDamage;
            tempTrans.TargetShieldVaule = targetInfo.TargetShieldVaule;
            tempTrans.AddTargetBuff = targetInfo.AddTargetBuff;
            tempTrans.RemoveTargetBuff = targetInfo.RemoveTargetBuff;
            return tempTrans;
        }
        #endregion
    }
}
