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

         Dictionary<int, Dictionary<int, TacticalDTO>> AllTacticalDict { get;  set; }

        public int AllTacticalCount { get; private set; }
        /// <summary>
        /// 记录确认创建的阵法实体
        /// </summary>
        Dictionary<int,TacticalEntity>TacticalEntityDict { get; set; }
        /// <summary>
        /// 记录临时储存的阵法，用于打断后撤销,key为roleid
        /// </summary>
         Dictionary<int, TacticalEntity> roletacticaltemp { get;  set; }
        /// <summary>
        /// 通过Redis返回的值取总集合中value移除，list下标0为地图id,1为阵法自增ID
        /// </summary>
        Dictionary<string, List<int>> RecordDelTactical { get;  set; }
        public List<int> ExpendTacticalID { get; private set; }

        public override void OnInitialization()
        {
            AllTacticalDict = new Dictionary<int, Dictionary<int, TacticalDTO>>();
            TacticalEntityDict = new Dictionary<int, TacticalEntity>();
            roletacticaltemp = new Dictionary<int, TacticalEntity>();
            RecordDelTactical = new Dictionary<string, List<int>>();
            ExpendTacticalID = new List<int>();
        }

        public override void OnPreparatory()
        {

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
            tacticalentity.TacticalDTO = tacticalDTO;
            var result = roletacticaltemp.TryAdd(tacticalentity.RoleID, tacticalentity);
            return result;
        }

        public bool TryAddRemoveTactical(int roleid,out TacticalDTO tacticalDTO )
        {
            var result = roletacticaltemp.TryGetValue(roleid,out var tacticalEntity);
            if (result)
            {
                tacticalDTO = tacticalEntity.TacticalDTO;
                roletacticaltemp.Remove(roleid);
                RedisHelper.String.StringSet("Tactical" + tacticalDTO.ID, tacticalDTO.RoleID.ToString(), new TimeSpan(0, 0, 0, 20));
                RedisManager.Instance.AddKeyExpireListener("JY_Tactical" + tacticalEntity.RoleID, tacticalEntity.RedisDeleteCaback);
                return TacticalEntityDict.TryAdd(tacticalEntity.ID, tacticalEntity);
            }
            else
                tacticalDTO = null;
            return result;
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
        //TODO 判断当前地图块是否可以创建
        public bool IsCreatTactic(int levelid = 0)
        {
            var result = TryGetValue(levelid, out var tacticalDict);

            if (result)
            {
                //TODO 具体判断重复覆盖的情况下
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 获取redis的暂存角色阵法
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="roletactical"></param>
        /// <returns></returns>
        public bool GetRoleTactic(int roleid, out List<TacticalDTO> roletactical)
        {
            roletactical = null;
            var result = RedisHelper.KeyExistsAsync("TacticalDTO").Result;
            if (result)
            {
                roletactical = RedisHelper.Hash.HashGetAsync<List<TacticalDTO>>("TacticalDTO" + roleid, roleid.ToString()).Result;
                if (roletactical.Count >= 3)
                {
                    GameManager.CustomeModule<TacticalDeploymentManager>().SendAllLevelRoleTactical(roletactical[0], ReturnCode.Fail);
                    TryRemove(roletactical[0]);
                    roletactical.Remove(roletactical[0]);
                }
            }
            return result;
        }

        public bool TryRemove(TacticalDTO  tacticalDTO)
        {
            var result = TryGetValue(tacticalDTO.LevelID,out var tacticalDict);
            if (result)
            {
                if (tacticalDict.ContainsKey(tacticalDTO.ID))
                {
                    tacticalDict.Remove(tacticalDTO.ID);
                }
            }
            if (TacticalEntityDict.TryGetValue(tacticalDTO.ID,out var tacticalEntity))
            {
                TacticalEntityDict.Remove(tacticalDTO.ID);
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
            Utility.Debug.LogInfo("yzqData对阵法的操作及数据" + Utility.Json.ToJson(tacticalDTO) + ">>>>>>>" + returnCode);
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
            var result = RecordDelTactical.TryGetValue(key, out List<int> tacticlise);
            if (result)
            {
                if (TryGetValue(tacticlise[0], out Dictionary<int, TacticalDTO> tacticalDict))
                {
                    Utility.Debug.LogInfo("yzqDataRedis返回的" + key + ">>>>>" + Utility.Json.ToJson(tacticlise));
                    SendAllLevelRoleTactical(tacticalDict[tacticlise[1]], ReturnCode.Fail);
                    TryRemove(tacticalDict[tacticlise[1]]);
                    var RedisExist = GameManager.CustomeModule<TacticalDeploymentManager>().GetRoleTactic(tacticalDict[tacticlise[1]].RoleID, out List<TacticalDTO> roletactical);
                    if (RedisExist)
                    {
                        roletactical.Remove(tacticalDict[tacticlise[1]]);
                        RedisHelper.Hash.HashSet("TacticalDTO" + tacticalDict[tacticlise[1]].RoleID, tacticalDict[tacticlise[1]].RoleID.ToString(), roletactical);
                    }
                    RecordDelTactical.Remove(key);
                }
            }
        }





            #region 待删
            //        /// <summary>
            //        /// 所有部署的阵法储存集合，key为地图块ID,vaule为具体阵法资源
            //        /// </summary>
            //        #region
            //        //public Dictionary<int, TacticalDeploymentDTO> AllTacticalDeploymentDict { get; private set; }
            //        public Dictionary<int, Dictionary<int,TacticalEntity>> AllTacticalDeploymentDict { get; private set; }
            //        #endregion
            //        public int TacticalCount {get;private set; }


            //        /// <summary>
            //        /// 消耗过的阵法自生成ID，用于重复使用
            //        /// </summary>
            //        public List<int> ExpendTacticalID { get; private set; }

            //        /// <summary>
            //        /// 临时阵法储存集合
            //        /// </summary>
            //        public Dictionary<int, TacticalEntity> roletacticaltemp { get; private set; }
            //        /// <summary>
            //        /// 通过Redis返回的值取总集合中value移除，list下标0为地图id,1为阵法自增ID
            //        /// </summary>
            //        public Dictionary<string, List<int>> RecordDelTactical { get; private set; }

            //        public override void OnInitialization()
            //        {
            //            AllTacticalDeploymentDict = new Dictionary<int, Dictionary<int, TacticalEntity>>();
            //            roletacticaltemp = new Dictionary<int, TacticalEntity>();
            //            ExpendTacticalID = new List<int>();
            //            RecordDelTactical = new Dictionary<string, List<int>>();
            //        }

            //        public override void OnPreparatory()
            //        {
            //            GameManager.CustomeModule<LevelManager>().OnRoleEnterLevel += SendTactical;
            //        }

            //        public bool ContainsKey(int levelId)
            //        {
            //            return AllTacticalDeploymentDict.ContainsKey(levelId);
            //        }
            //        //TODO 判断当前地图块是否可以创建
            //        public bool IsCreatTactic(out Dictionary<int, TacticalEntity> tacticalDeployment, int levelid = 0)
            //        {
            //            var result = TryGetValue(levelid, out tacticalDeployment);

            //            if (result)
            //            {
            //                //TODO 具体判断重复覆盖的情况下
            //                result = false;
            //            }
            //            return result;
            //        }
            //        public int GetExpendTacticalID()
            //        {
            //           int id = 0;
            //            if (ExpendTacticalID.Count > 0)
            //            {
            //                id = ExpendTacticalID[0];
            //                ExpendTacticalID.Remove(ExpendTacticalID[0]);
            //                return id;
            //            }
            //            else
            //                return TacticalCount;

            //        }
            //        /// <summary>
            //        /// 获取当前地图块的所有阵法
            //        /// </summary>
            //        /// <param name="roleId"></param>
            //        /// <param name="tactical"></param>
            //        /// <returns></returns>
            //        public bool TryGetValue(int levelid, out Dictionary<int, TacticalEntity> tacticalDeployment)
            //        {
            //            return AllTacticalDeploymentDict.TryGetValue(levelid, out tacticalDeployment);
            //        }
            //        /// <summary>
            //        /// 从储存的集合中移除
            //        /// </summary>
            //        /// <param name="levelid"></param>
            //        /// <param name="id"></param>
            //        /// <returns></returns>
            //        public bool TryRemove(int levelid, int id)
            //        {
            //            var result = TryGetValue(levelid, out var tacticalDeployment);
            //            if (result)
            //            {
            //                var Exist = tacticalDeployment.TryGetValue(id, out var tactical);
            //                if (Exist)
            //                {
            //                    ExpendTacticalID.Add(tactical.ID);
            //                    tacticalDeployment.Remove(tactical.ID);
            //                }
            //                return Exist;
            //            }
            //            return result;
            //        }
            //        /// <summary>
            //        /// 获取redis的暂存角色阵法
            //        /// </summary>
            //        /// <param name="roleid"></param>
            //        /// <param name="roletactical"></param>
            //        /// <returns></returns>
            //        public bool GetRoleTactic(int roleid, out List<TacticalDTO> roletactical)
            //        {
            //            roletactical = null;
            //            var result = RedisHelper.KeyExistsAsync("TacticalDTO").Result;
            //            if (result)
            //            {
            //                roletactical = RedisHelper.Hash.HashGetAsync<List<TacticalDTO>>("TacticalDTO" + roleid, roleid.ToString()).Result;
            //            }
            //            return result;
            //        }
            //        /// <summary>
            //        /// 尝试添加进已存在的集合
            //        /// </summary>
            //        /// <param name="tacticalDTO"></param>
            //        /// <param name="levelid"></param>
            //        /// <returns></returns>
            //        public bool TryAdd(TacticalEntity tacticalDTO, int levelid = 0)
            //        {
            //            var result = TryGetValue(levelid, out Dictionary<int, TacticalEntity> tacticalDeploymentDTO);
            //            if (result)
            //            {
            //               tacticalDeploymentDTO.TryAdd(tacticalDTO.ID, tacticalDTO);
            //                return AllTacticalDeploymentDict.TryAdd(levelid, tacticalDeploymentDTO);
            //            }
            //            else
            //            {
            //                tacticalDeploymentDTO = new Dictionary<int, TacticalEntity>();
            //                //tacticalDeploymentDTO.LevelID = levelid;
            //                //tacticalDeploymentDTO.tacticDict.TryAdd(tacticalDTO.ID, tacticalDTO);
            //                return AllTacticalDeploymentDict.TryAdd(levelid,tacticalDeploymentDTO);
            //            }
            //        }
            //        /// <summary>
            //        /// 过时需要替换的
            //        /// </summary>
            //        /// <param name="tacticalDTO"></param>
            //        /// <returns></returns>
            //        public bool AddTacTical(TacticalEntity tacticalDTO)
            //        {
            //            var result = roletacticaltemp.TryAdd(tacticalDTO.RoleID, tacticalDTO);
            //            return result;
            //        }
            //        /// <summary>
            //        /// 获取临时集合阵法并删除取出值
            //        /// </summary>
            //        /// <param name="tacticalDTO"></param>
            //        /// <returns></returns>
            //        public bool GetRemoveTacTical(int roleid, out TacticalEntity tacticalDTO)
            //        {
            //            var result = roletacticaltemp.TryGetValue(roleid, out tacticalDTO);
            //            if (result)
            //            {
            //                GameManager.CustomeModule<TacticalDeploymentManager>().TryAdd(tacticalDTO);
            //                roletacticaltemp.Remove(roleid);
            //            }
            //            return result;
            //        }
            //        /// <summary>
            //        /// 广播给当前场景所有人对阵法的操作
            //        /// </summary>
            //        /// <param name="tacticalDTO"></param>
            //        /// <param name="returnCode">成功为生成，失败为销毁</param>
            //        public void SendAllLevelRoleTactical(TacticalDTO tacticalDTO,ReturnCode returnCode)
            //        {
            //            Utility.Debug.LogInfo("yzqData对阵法的操作及数据"+Utility.Json.ToJson(tacticalDTO)+ ">>>>>>>"+returnCode);
            //            OperationData operationData = new OperationData();
            //            operationData.DataMessage = Utility.Json.ToJson(tacticalDTO);
            //            operationData.ReturnCode = (short)returnCode;
            //            operationData.OperationCode = (ushort)OperationCode.SyncGetNewTactical;
            //            GameManager.CustomeModule<LevelManager>().SendMsg2AllLevelRoleS2C(0, operationData);
            //        }
            //        /// <summary>
            //        /// 监听Redis删除后的回调
            //        /// </summary>
            //        public void RedisDeleteCaback(string key)
            //        {
            //         //var result=   RecordDelTactical.TryGetValue(key,out List<int>tacticlise );
            //         //   if (result)
            //         //   {
            //         //       if (TryGetValue(tacticlise[0], out TacticalDeploymentDTO tacticalDeployment))
            //         //       {
            //         //           Utility.Debug.LogInfo("yzqDataRedis返回的" + key + ">>>>>" + Utility.Json.ToJson(tacticlise));
            //         //           SendAllLevelRoleTactical(tacticalDeployment.tacticDict[tacticlise[1]], ReturnCode.Fail);
            //         //           TryRemove(tacticlise[0], tacticlise[1]);
            //         //           var RedisExist = GameManager.CustomeModule<TacticalDeploymentManager>().GetRoleTactic(tacticalDeployment.tacticDict[tacticlise[1]].RoleID, out List<TacticalDTO> roletactical);
            //         //           if (RedisExist)
            //         //           {
            //         //               roletactical.Remove(tacticalDeployment.tacticDict[tacticlise[1]]);
            //         //               RedisHelper.Hash.HashSet("TacticalDTO" + tacticalDeployment.tacticDict[tacticlise[1]].RoleID, tacticalDeployment.tacticDict[tacticlise[1]].RoleID.ToString(), roletactical);
            //         //           }
            //         //           RecordDelTactical.Remove(key);
            //         //       }
            //         //   }

            //        }    
            //        /// <summary>
            //        /// 发送已生成的阵法至指定场景
            //        /// </summary>
            //        void SendTactical(int id, RoleEntity roleEntity)
            //        {
            //            if (GetExpendTacticalID()>1)
            //            {
            //                OperationData operationData = new OperationData();
            //                operationData.DataMessage = Utility.Json.ToJson(AllTacticalDeploymentDict[id].Values);
            //                operationData.OperationCode = (byte)OperationCode.SyncCreatTactical;
            //                GameManager.CustomeModule<RoleManager>().SendMessage(roleEntity.RoleId, operationData);
            //                Utility.Debug.LogInfo("yzqData发送的全部阵法" + Utility.Json.ToJson(AllTacticalDeploymentDict) + "juese id " + roleEntity.RoleId);
            //            }
            //        }
            ///// <summary>
            ///// 创建新的阵法实体，并添加到临时集合
            ///// </summary>
            ///// <param name="roleid"></param>
            ///// <param name="id"></param>
            ///// <returns></returns>
            //        public bool TacticalCreateAdd(TacticalDTO tacticalDTO)
            //        {
            //            var tacticalentity= TacticalEntity.Create(tacticalDTO.ID, tacticalDTO.RoleID);
            //            tacticalentity.TacticalDTO = tacticalDTO;
            //            var result = roletacticaltemp.TryAdd(tacticalentity.RoleID, tacticalentity);
            //            return result;
            //        }
            #endregion
        }
    }
