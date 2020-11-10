using Cosmos;
using System.Collections.Generic;
using System.Linq;
using AscensionProtocol.DTO;
using Protocol;
using AscensionProtocol;
using RedisDotNet;
using System;

namespace AscensionServer
{
    [CustomeModule]
    public class TacticalDeploymentManager : Module<TacticalDeploymentManager>
    {

        Dictionary<int, Dictionary<int, TacticalDTO>> AllTacticalDict { get; set; }

        int AllTacticalCount { get; set; }
        /// <summary>
        /// 记录确认创建的阵法实体
        /// </summary>
        Dictionary<int, TacticalEntity> TacticalEntityDict { get; set; }
        /// <summary>
        /// 记录临时储存的阵法，用于打断后撤销,key为roleid
        /// </summary>
        Dictionary<int, TacticalEntity> roletacticaltemp { get; set; }
        /// <summary>
        /// 通过Redis返回的值取总集合中value移除，list下标0为地图id,1为阵法自增ID
        /// </summary>
        Dictionary<string, TacticalDTO> RecordDelTactical { get; set; }
        Dictionary<int, List<TacticalDTO>>RoleTactical { get; set; }

        List<int> ExpendTacticalID { get;  set; }

        long latestTime;
        int updateInterval = ApplicationBuilder._MSPerTick;

        Action tacticalRefreshHandler;
        event Action TacticalRefreshHandler
        {
            add { tacticalRefreshHandler += value; }
            remove { tacticalRefreshHandler -= value; }
        }

        public override void OnInitialization()
        {
            AllTacticalDict = new Dictionary<int, Dictionary<int, TacticalDTO>>();
            TacticalEntityDict = new Dictionary<int, TacticalEntity>();
            roletacticaltemp = new Dictionary<int, TacticalEntity>();
            RecordDelTactical = new Dictionary<string, TacticalDTO>();
            ExpendTacticalID = new List<int>();
            RoleTactical = new Dictionary<int, List<TacticalDTO>>();
        }

        public override void OnPreparatory()
        {
            GameManager.CustomeModule<LevelManager>().OnRoleEnterLevel += SendTactical;
        }
        public override void OnRefresh()
        {
            var now = Utility.Time.MillisecondNow();
            if (latestTime <= now)
            {
                latestTime = now + updateInterval;
                tacticalRefreshHandler?.Invoke();
            }
        }
        /// <summary>
        /// 创建新的阵法实体，并添加到临时集合
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool TacticalCreateAdd(TacticalDTO tacticalDTO)
        {
            var tacticalentity = TacticalEntity.Create(tacticalDTO.ID, tacticalDTO.RoleID, tacticalDTO.LevelID);
            TacticalRefreshHandler += tacticalentity.OnRefresh;
            AllTacticalCount += 1;
            tacticalentity.TacticalDTO = tacticalDTO;
            var result = roletacticaltemp.TryAdd(tacticalentity.RoleID, tacticalentity);
            //Utility.Debug.LogInfo("yzqData阵法实体数据" + Utility.Json.ToJson(tacticalentity));
            return result;
        }
        /// <summary>
        /// 移除暂存集合放入总集合
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="tacticalDTO"></param>
        /// <returns></returns>
        public bool TryAddRemoveTactical(int roleid, out TacticalDTO tacticalDTO)
        {
            var result = roletacticaltemp.TryGetValue(roleid, out var tacticalEntity);
            if (result)
            {
                tacticalDTO = tacticalEntity.TacticalDTO;
                roletacticaltemp.Remove(roleid);
                RedisHelper.String.StringSet(RedisKeyDefine._DeldteTacticalPerfix + tacticalDTO.ID, tacticalDTO.RoleID.ToString(), new TimeSpan(0, 0, 0, tacticalDTO.Duration));
                RecordDelTactical.TryAdd("JY_Tactical" + tacticalDTO.ID, tacticalDTO);
                SendAllLevelRoleTactical(tacticalDTO, ReturnCode.Success);
                if (TryAdd(tacticalEntity.LevelID, tacticalEntity))
                {
                    return TacticalEntityDict.TryAdd(tacticalEntity.ID, tacticalEntity);
                }
            }
            else
                tacticalDTO = null;
            return result;
        }
        /// <summary>
        /// 打断操作移除临时集合储存阵法
        /// </summary>
        /// <param name="roleid"></param>
        public void TryRemoveTactical(int roleid)
        {
            var result = roletacticaltemp.TryGetValue(roleid, out var tacticalEntity);
            if (result)
            {
                if (ExpendTacticalID.Contains(tacticalEntity.ID))
                {
                    ExpendTacticalID.Remove(tacticalEntity.ID);
                }
                roletacticaltemp.Remove(roleid);
                GameManager.ReferencePoolManager.Despawn(tacticalEntity);
            }
        }

        public int GetExpendTacticalID()
        {
            int id = 0;
            if (ExpendTacticalID.Count > 0)
            {
                id = ExpendTacticalID[0];
                ExpendTacticalID.Remove(ExpendTacticalID[0]);
                return id;
            }
            else
                return AllTacticalCount;
        }
        /// <summary>
        /// 获取当前地图块的所有阵法
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="tactical"></param>
        /// <returns></returns>
        public bool TryGetValue(int levelid, out Dictionary<int, TacticalDTO> tacticalDeployment)
        {
            return AllTacticalDict.TryGetValue(levelid, out tacticalDeployment);
        }
        public bool TryAdd(int levelid, TacticalEntity tacticalEntity)
        {
            var result = TryGetValue(levelid, out var tacticalDict);
            if (result)
            {
                return tacticalDict.TryAdd(tacticalEntity.ID, tacticalEntity.TacticalDTO);
            }
            else
            {
                tacticalDict = new Dictionary<int, TacticalDTO>();
                tacticalDict.TryAdd(tacticalEntity.ID, tacticalEntity.TacticalDTO);
                return AllTacticalDict.TryAdd(levelid, tacticalDict);
            }
        }

        //TODO 判断当前地图块是否可以创建
        public bool IsCreatTactic(int levelid = 0)
        {
            var result = TryGetValue(levelid, out var tacticalDict);
            if (!result)
            {
                result = true;
            }
            else
            {
                //TODO 具体判断重复覆盖的情况下,为了测试暂时显示为ture
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 获取本地的暂存角色阵法
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="roletactical"></param>
        /// <returns></returns>
        public void GetRoleTactic(int roleid, out List<TacticalDTO> roletactical)
        {
            var result = RoleTactical.TryGetValue(roleid,out var tacticalDTOs);
           // Utility.Debug.LogInfo("yzqData阵法移除进来了" + result);
            roletactical = tacticalDTOs;
            if (result)
            {
                //Utility.Debug.LogInfo("yzqData阵法长度拿到了" + tacticalDTOs.Count);
                if (tacticalDTOs.Count >=3)
                {
                    //Utility.Debug.LogInfo("yzqData阵法实体数据1" + RoleTactical[roleid].Count);
                    GameManager.CustomeModule<TacticalDeploymentManager>().SendAllLevelRoleTactical(tacticalDTOs[0], ReturnCode.Fail);
                    //Utility.Debug.LogInfo("yzqData阵法实体数据2" + RoleTactical[roleid].Count);
                    TryRemove(tacticalDTOs[0]);
                    //Utility.Debug.LogInfo("yzqData阵法实体数据3" + RoleTactical[roleid].Count);
                    tacticalDTOs.RemoveAt(0);
                  //  Utility.Debug.LogInfo("yzqData阵法实体数据4" + RoleTactical[roleid].Count);
                }               
            }
        }

        public bool TryRemove(TacticalDTO tacticalDTO)
        {
            var result = TryGetValue(tacticalDTO.LevelID, out var tacticalDict);
            if (result)
            {
                if (tacticalDict.ContainsKey(tacticalDTO.ID))
                {
                    tacticalDict.Remove(tacticalDTO.ID);
                }
            }
            if (TacticalEntityDict.TryGetValue(tacticalDTO.ID, out var tacticalEntity))
            {
                TacticalEntityDict.Remove(tacticalDTO.ID);
                ExpendTacticalID.Add(tacticalDTO.ID);
                GameManager.ReferencePoolManager.Despawn(tacticalEntity);
            }
            return result;
        }
        /// <summary>
        /// 广播给当前场景所有人对阵法的操作
        /// </summary>
        /// <param name="tacticalDTO"></param>
        /// <param name="returnCode">成功为生成，失败为销毁</param>
        public void SendAllLevelRoleTactical(TacticalDTO tacticalDTO, ReturnCode returnCode)
        {
            //Utility.Debug.LogInfo("yzqData对阵法的操作及数据" + Utility.Json.ToJson(tacticalDTO) + ">>>>>>>" + returnCode);
            OperationData operationData = new OperationData();
            operationData.DataMessage = Utility.Json.ToJson(tacticalDTO);
            operationData.ReturnCode = (short)returnCode;
            operationData.OperationCode = (ushort)OperationCode.SyncGetNewTactical;
            GameManager.CustomeModule<LevelManager>().SendMsg2AllLevelRoleS2C(tacticalDTO.LevelID, operationData);
        }
        /// <summary>
        /// 监听Redis删除后的回调
        /// </summary>
        public void RedisDeleteCaback(string key)
        {
            var result = RecordDelTactical.TryGetValue(key, out var tacticalEntity);
            if (result)
            {
                if (TryGetValue(tacticalEntity.LevelID, out Dictionary<int, TacticalDTO> tacticalDict))
                {
                    GameManager.CustomeModule<TacticalDeploymentManager>().GetRoleTactic(tacticalEntity.RoleID, out List<TacticalDTO> roletactical);
                    roletactical.Remove(tacticalDict[tacticalEntity.ID]);
                    SendAllLevelRoleTactical(tacticalDict[tacticalEntity.ID], ReturnCode.Fail);
                    var exits = RoleTactical.TryGetValue(tacticalEntity.RoleID, out var tacticalDTOs);
                    if (exits)
                    {
                        tacticalDTOs.Remove(tacticalDict[tacticalEntity.ID]);
                    }
                    TryRemove(tacticalDict[tacticalEntity.ID]);
                    ExpendTacticalID.Add(tacticalEntity.ID);     
                    RecordDelTactical.Remove(key);
                }
            }
        }
        /// <summary>
        /// 储存每个角色的所有阵法
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="tacticalDTO"></param>
        public void TryAddRoleAllTactical(int roleid,TacticalDTO tacticalDTO)
        {
            var result = RoleTactical.TryGetValue(roleid,out var tacticalDTOs);
            if (result)
            {
                tacticalDTOs.Add(tacticalDTO);
                //Utility.Debug.LogInfo("yzqData1储存角色阵法" + RoleTactical[roleid].Count);
            }
            else
            {
                tacticalDTOs = new List<TacticalDTO>();
                 tacticalDTOs.Add(tacticalDTO);
                RoleTactical.TryAdd(tacticalDTO.RoleID, tacticalDTOs);
               // Utility.Debug.LogInfo("yzqData2储存角色阵法" + RoleTactical[roleid].Count);
            }
        }
        /// <summary>
        /// 发送已生成的阵法至指定场景
        /// </summary>
        void SendTactical(int id, RoleEntity roleEntity)
        {
            if (GetExpendTacticalID() > 0)
            {
                OperationData operationData = new OperationData();
                operationData.DataMessage = Utility.Json.ToJson(AllTacticalDict[id]);
                operationData.OperationCode = (byte)OperationCode.SyncCreatTactical;
                GameManager.CustomeModule<RoleManager>().SendMessage(roleEntity.RoleId, operationData);
                //Utility.Debug.LogInfo("yzqData发送的全部阵法" + Utility.Json.ToJson(AllTacticalDict) + "juese id " + roleEntity.RoleId);
            }
        }
  }
}
