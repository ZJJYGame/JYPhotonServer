using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using Google.Protobuf.WellKnownTypes;
using NHibernate.Linq.Clauses;

namespace AscensionServer
{
    public partial class ServerBattleManager
    {
        #region 出手速度 以及出手拥有者 以及对Ai的血量判断
        /// <summary>
        /// 出手速度
        /// </summary>
        public void ReleaseToSpeed(int roleId)
        {
            if (TeamIdToBattleInit.ContainsKey(roleId))
                BattleInitObject(roleId).battleUnits = BattleInitObject(roleId).battleUnits.OrderByDescending(t => t.ObjectSpeed).ToList();

            foreach (var item in BattleInitObject(roleId).battleUnits)
            {
                Utility.Debug.LogInfo("老陆 ，出手速度" + item.ObjectSpeed + "<>" + item.ObjectName);
            }
        }

        /// <summary>
        /// 返回一个出手拥有者  玩家或者AI或者宠物
        /// </summary>
        /// <returns></returns>
        public object ReleaseToOwner(int objectID, int objectId, int roleId)
        {
            //Utility.Debug.LogInfo("<出手速度>" + objectID + "<>" + objectId + "<>" + roleId);
            if (BattleInitObject(roleId).playerUnits.Find(t => (t.RoleStatusDTO.RoleID == objectID)) != null)
                return BattleInitObject(roleId).playerUnits.Find(t => (t.RoleStatusDTO.RoleID == objectID)).RoleStatusDTO;
            if (BattleInitObject(roleId).enemyUnits.Find(t => (t.EnemyStatusDTO.EnemyId == objectId)) != null)
                return BattleInitObject(roleId).enemyUnits.Find(t => (t.EnemyStatusDTO.EnemyId == objectId)).EnemyStatusDTO;
            if (BattleInitObject(roleId).petUnits.Find(t => (t.ObjectId == objectId)) != null)
                return BattleInitObject(roleId).petUnits.Find(t => (t.ObjectId == objectId)).PetStatusDTO;
            return null;
        }

        /// <summary>
        /// 针对AI  血量 >0
        /// </summary>
        public List<EnemyBattleDataDTO> AIToHPMethod(int roleId, List<EnemyBattleDataDTO> enemyBattleDatas)
        {
            List<EnemyBattleDataDTO> tempDataSet = new List<EnemyBattleDataDTO>();
            for (int i = 0; i < enemyBattleDatas.Count; i++)
            {
                if (BattleInitObject(roleId).enemyUnits[i].EnemyStatusDTO.EnemyHP > 0)
                    tempDataSet.Add(BattleInitObject(roleId).enemyUnits[i]);
            }
            return tempDataSet;
        }

        /// <summary>
        /// 针对玩家  血量 >0
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="roleBattleDatas"></param>
        /// <returns></returns>
        public List<RoleBattleDataDTO> PlayerToHPMethod(int roleId, int currentRoleId, List<RoleBattleDataDTO> roleBattleDatas)
        {
            List<RoleBattleDataDTO> tempDataSet = new List<RoleBattleDataDTO>();
            for (int i = 0; i < roleBattleDatas.Count; i++)
            {
                if (BattleInitObject(roleId).playerUnits[i].RoleStatusDTO.RoleID == currentRoleId)
                    continue;
                if (BattleInitObject(roleId).playerUnits[i].RoleStatusDTO.RoleHP > 0)
                    tempDataSet.Add(BattleInitObject(roleId).playerUnits[i]);
            }
            return tempDataSet;
        }
        /// <summary>
        /// 针对宠物 血量>0
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="petBattleDataDTOs"></param>
        /// <returns></returns>
        public List<PetBattleDataDTO> PetToHPMethod(int roleId, List<PetBattleDataDTO> petBattleDataDTOs)
        {
            List<PetBattleDataDTO> tempDataSet = new List<PetBattleDataDTO>();
            for (int i = 0; i < petBattleDataDTOs.Count; i++)
                if (BattleInitObject(roleId).petUnits[i].PetStatusDTO.PetHP > 0)
                    tempDataSet.Add(BattleInitObject(roleId).petUnits[i]);
            return tempDataSet;
        }
        #endregion


        /// <summary>
        /// 处理AI 判断玩家是不是死亡 和要选择能出手的Ai                ??? TODO第四个参数有待完善
        /// </summary>
        public void AIToRelease(BattleTransferDTO battleTransferDTOs, EnemyStatusDTO enemyStatusData, int roleId,int currentId, int  transfer = 0)
        {
            var defendValue = battleTransferDTOs.BattleCmd == BattleCmd.Defend ? 80 : 100;
            if ((IsTeamDto(roleId) == null))
            {
                ///TODO  需要怪物的技能表格 释放技能
                if (BattleInitObject(roleId).petUnits.Count !=0&& BattleInitObject(roleId).petUnits[0].PetStatusDTO.PetHP>0&& !isPetRunAway)
                {
                    //Utility.Debug.LogInfo("我来随机啦+++++>>>" + new Random((int)DateTime.Now.Ticks+transfer).Next(0, 2));
                    var RandomTarget = RandomManager(transfer,0,2);
                    var target = RandomTarget == 0 ? BattleInitObject(roleId).playerUnits[0].RoleStatusDTO.RoleHP -= defendValue : BattleInitObject(roleId).petUnits[0].PetStatusDTO.PetHP -= defendValue;
                    var TargetInfosSet = ServerToClientResult(new TargetInfoDTO() { TargetID = RandomTarget == 0 ? currentId : BattleInitObject(roleId).petUnits[0].PetStatusDTO.PetID, TargetHPDamage = -defendValue });
                    TeamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = TargetInfosSet });
                }
                else
                {
                    BattleInitObject(roleId).playerUnits[0].RoleStatusDTO.RoleHP -= defendValue;
                    var TargetInfosSet = ServerToClientResult(new TargetInfoDTO() { TargetID = currentId, TargetHPDamage = -defendValue });
                    TeamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = TargetInfosSet });
                }
            }
            else
            {
                //Utility.Debug.LogInfo("老陆 ，=>" + BattleInitObject(roleId).playerUnits[transfer].RoleStatusDTO.RoleID);
                var petObject = BattleInitObject(roleId).petUnits.Find(x => x.PetStatusDTO.PetHP > 0);
                if (BattleInitObject(roleId).petUnits.Count != 0 && petObject != null&& !IsPetTeamRunAway)
                {
                    IsPetTeamRunAway = false;
                    var RandomTarget = RandomManager(transfer, 0, 2);
                    var target = RandomTarget == 0 ? BattleInitObject(roleId).playerUnits[transfer].RoleStatusDTO.RoleHP -= defendValue : petObject.PetStatusDTO.PetHP -= defendValue;
                    var TargetInfosSet = ServerToClientResult(new TargetInfoDTO() { TargetID = RandomTarget == 0 ? BattleInitObject(roleId).playerUnits[transfer].RoleStatusDTO.RoleID : petObject.PetStatusDTO.PetID, TargetHPDamage = -defendValue });
                    TeamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = TargetInfosSet });
                }
                else
                {
                    BattleInitObject(roleId).playerUnits[transfer].RoleStatusDTO.RoleHP -= defendValue;
                    var TargetInfosSet = ServerToClientResult(new TargetInfoDTO() { TargetID = currentId, TargetHPDamage = -defendValue });
                    TeamSet.Add(new BattleTransferDTO() { isFinish = true, BattleCmd = BattleCmd.SkillInstruction, RoleId = enemyStatusData.EnemyId, ClientCmdId = 21001, TargetInfos = TargetInfosSet });
                }
            }
        }


        
    }
}


