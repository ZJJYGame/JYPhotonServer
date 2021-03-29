﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Protocol;

namespace AscensionServer
{
    [Module]
    public class BattleRoomManager:Module,IBattleRoomManager
    {
        #region 等待时间参数 
        public float PrepareWaitTime { get; } = 5;
        public float RoundTIme { get; } = 15;
        #endregion

        //倒计时相关事件，每帧刷新
        Action timeAction;
        public event Action TimeAction
        {
            add
            {
                timeAction += value;
            }
            remove
            {
                timeAction -= value;
            }
        }

        //战斗房间id起始位置
        int battleRoomIdStartIndex = 1000;
        //回收后可被使用的房间Id列表
        List<int> canUseRoomIdList = new List<int>();
        //占用中的房间Id列表
        List<int> occupiedRoomIdList = new List<int>();

        //记录当前存在的房间对象,key=>房间号，value=>房间实体对象
        Dictionary<int, BattleRoomEntity> battleRoomDict = new Dictionary<int, BattleRoomEntity>();


        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncBattle, EnterBattleC2S);
        }
        public override void OnRefresh()
        {
            timeAction?.Invoke();
        }


        /// <summary>
        /// 创建房间
        /// </summary>
        public void CreateRoom(BattleInitDTO battleInitDTO)
        {
            BattleRoomEntity battleRoomEntity = CosmosEntry.ReferencePoolManager.Spawn<BattleRoomEntity>();
            int roomID = GetRoomId();
            battleRoomEntity.InitRoom(roomID, battleInitDTO);
            battleRoomDict.Add(roomID, battleRoomEntity);
        }
        /// <summary>
        /// 销毁房间
        /// </summary>
        public void DestoryRoom()
        {
        }
        //告知对应房间角色准备完成
        void RoomRolePrepare(int roleID)
        {
            int roomID= GameEntry.BattleCharacterManager.GetCharacterEntity(roleID).RoomID;
            battleRoomDict[roomID].CharacterPrepare(roleID);
        }
        //获取人物的战斗指令
        void GetRoleBattleCmd(int roleID,BattleCmd battleCmd,BattleTransferDTO battleTransferDTO)
        {
            int roomID = GameEntry.BattleCharacterManager.GetCharacterEntity(roleID).RoomID;
            battleRoomDict[roomID].GetCharacterCmd(roleID, battleCmd,battleTransferDTO);
        }
        /// <summary>
        /// 获取可使用的roomId
        /// </summary>
        /// <returns>返回房间号</returns>
        int GetRoomId()
        {
            int roomId;
            if (canUseRoomIdList.Count > 0)
            {
                roomId = canUseRoomIdList[0];
            }
            else
            {
                roomId = battleRoomIdStartIndex;
                battleRoomIdStartIndex++;
            }
            return roomId;
        }

        #region 网络请求
        /// <summary>
        /// 进入战斗的请求
        /// </summary>
        void EnterBattleC2S(int sessionId,OperationData packet)
        {
            //收到服务器的进入战斗请求
            var areaSubCode = (BattleCmd)packet.SubOperationCode;
            var dict = Utility.Json.ToObject<Dictionary<byte, object>>(Convert.ToString(packet.DataMessage));
            RoleDTO roleDTO = Utility.Json.ToObject<RoleDTO>(Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.Role)));
            BattleTransferDTO battleTransferDTO= Utility.Json.ToObject<BattleTransferDTO>(Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleBattle)));
            switch (areaSubCode)
            {
                case BattleCmd.Init:
                    Utility.Debug.LogError("收到进入战斗请求");
                    CreateRoom(roleDTO.BattleInitDTO);
                    break;
                case BattleCmd.Prepare:
                    int roleID = roleDTO.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID;
                    RoomRolePrepare(roleID);
                    break;
                //战斗指令
                case BattleCmd.PropsInstruction:
                case BattleCmd.SkillInstruction:
                case BattleCmd.RunAwayInstruction:
                case BattleCmd.MagicWeapon:
                case BattleCmd.CatchPet:
                case BattleCmd.SummonPet:
                case BattleCmd.Defend:
                    roleID = roleDTO.BattleInitDTO.playerUnits[0].RoleStatusDTO.RoleID;
                    GetRoleBattleCmd(roleID, areaSubCode, battleTransferDTO);
                    break;
            }
        }
        #endregion
    }
}