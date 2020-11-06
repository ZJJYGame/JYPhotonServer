using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionData;
using AscensionProtocol.DTO;
using UnityEngine;
using Protocol;
using AscensionProtocol;
using RedisDotNet;
namespace AscensionServer
{
    [CustomeModule]
    public class TacticalDeploymentManager : Module<TacticalDeploymentManager>
    {
        /// <summary>
        /// 所有部署的阵法储存集合，key为地图块ID,vaule为具体阵法资源
        /// </summary>
       public Dictionary<int, TacticalDeploymentDTO> AllTacticalDeploymentDict { get; private set; }
        /// <summary>
        /// 消耗过的阵法自生成ID，用于重复使用
        /// </summary>
      public List<int> ExpendTacticalID { get; private set; }

        /// <summary>
        /// 临时阵法储存集合
        /// </summary>
        public Dictionary<int, TacticalDTO> roletacticaltemp { get; private set; }

        public override void OnInitialization()
        {
            AllTacticalDeploymentDict = new Dictionary<int, TacticalDeploymentDTO>();
            roletacticaltemp = new Dictionary<int, TacticalDTO>();
            ExpendTacticalID = new List<int>();
        }

        public override void OnPreparatory()
        {
            GameManager.CustomeModule<LevelManager>().OnRoleEnterLevel += SendTactical;
        }

        public bool ContainsKey(int levelId)
        {
            return AllTacticalDeploymentDict.ContainsKey(levelId);
        }
        //TODO 判断当前地图块是否可以创建
        public bool IsCreatTactic(out TacticalDeploymentDTO tacticalDeployment, int levelid = 0)
        {
            var result = TryGetValue(levelid, out tacticalDeployment);

            if (result)
            {
                //TODO 具体判断重复覆盖的情况下
                result = false;
            }
            return result;
        }
        public int GetExpendTacticalID()
        {
            int id = 0;
            var tacticalList= AllTacticalDeploymentDict.Values.ToList();
            for (int i = 0; i < tacticalList.Count; i++)
            {
                id += tacticalList[i].tacticDict.Count;
            }
            id += 1;
            if (ExpendTacticalID.Count > 0)
            {
                id = ExpendTacticalID[0];
                ExpendTacticalID.Remove(ExpendTacticalID[0]);
                return id;
            }
            else
                return id;
        }
        /// <summary>
        /// 获取当前地图块的所有阵法
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="tactical"></param>
        /// <returns></returns>
        public bool TryGetValue(int levelid, out TacticalDeploymentDTO tacticalDeployment)
        {
            return AllTacticalDeploymentDict.TryGetValue(levelid, out tacticalDeployment);
        }
        /// <summary>
        /// 从储存的集合中移除
        /// </summary>
        /// <param name="levelid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool TryRemove(int levelid, int id)
        {
            var result = TryGetValue(levelid, out var tacticalDeployment);
            if (result)
            {
                var Exist = tacticalDeployment.tacticDict.TryGetValue(id, out var tactical);
                if (Exist)
                {
                    ExpendTacticalID.Add(tactical.ID);
                    tacticalDeployment.tacticDict.Remove(tactical.ID);
                }
                return Exist;
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
            }
            return result;
        }
        /// <summary>
        /// 尝试添加进已存在的集合
        /// </summary>
        /// <param name="tacticalDTO"></param>
        /// <param name="levelid"></param>
        /// <returns></returns>
        public bool TryAdd(TacticalDTO tacticalDTO, int levelid = 0)
        {
            var result = TryGetValue(levelid, out TacticalDeploymentDTO tacticalDeploymentDTO);
            if (result)
            {
               tacticalDeploymentDTO.tacticDict.TryAdd(tacticalDTO.ID, tacticalDTO);
                return AllTacticalDeploymentDict.TryAdd(levelid, tacticalDeploymentDTO);
            }
            else
            {
                tacticalDeploymentDTO = new TacticalDeploymentDTO();
                tacticalDeploymentDTO.LevelID = levelid;
                tacticalDeploymentDTO.tacticDict.TryAdd(tacticalDTO.ID, tacticalDTO);
                return AllTacticalDeploymentDict.TryAdd(levelid,tacticalDeploymentDTO);
            }
        }
        /// <summary>
        /// 把创建出来的阵法放进临时集合中，等待操作
        /// </summary>
        /// <param name="tacticalDTO"></param>
        /// <returns></returns>
        public bool AddTacTical(TacticalDTO tacticalDTO)
        {
            var result = roletacticaltemp.TryAdd(tacticalDTO.RoleID, tacticalDTO);
            return result;
        }
        /// <summary>
        /// 获取临时集合阵法并删除取出值
        /// </summary>
        /// <param name="tacticalDTO"></param>
        /// <returns></returns>
        public bool GetRemoveTacTical(int roleid, out TacticalDTO tacticalDTO)
        {
            var result = roletacticaltemp.TryGetValue(roleid, out tacticalDTO);
            if (result)
            {
                GameManager.CustomeModule<TacticalDeploymentManager>().TryAdd(tacticalDTO);
                roletacticaltemp.Remove(roleid);
            }
            return result;
        }
        /// <summary>
        /// 广播给当前场景所有人对阵法的操作
        /// </summary>
        /// <param name="tacticalDTO"></param>
        /// <param name="returnCode">成功为生成，失败为销毁</param>
        public void SendAllLevelRoleTactical(TacticalDTO tacticalDTO,ReturnCode returnCode)
        {
            Utility.Debug.LogInfo("yzqData对阵法的操作及数据"+Utility.Json.ToJson(tacticalDTO)+ ">>>>>>>"+returnCode);
            OperationData operationData = new OperationData();
            operationData.DataMessage = Utility.Json.ToJson(tacticalDTO);
            operationData.ReturnCode = (short)returnCode;
            operationData.OperationCode = (ushort)OperationCode.SyncGetNewTactical;
            GameManager.CustomeModule<LevelManager>().SendMsg2AllLevelRoleS2C(0, operationData);
        }
        /// <summary>
        /// 监听Redis删除后的回调
        /// </summary>
        public void RedisDeleteCaback(string key)
        {
            //TODO 逻辑通了之后记得更换
            string[] strArray = key.Split('$');
            if (strArray.Length>0)
            {
                if (TryGetValue(int.Parse(strArray[1]), out TacticalDeploymentDTO tacticalDeployment))
                {
                    Utility.Debug.LogInfo("yzqData返回的Rediskey" + Utility.Json.ToJson(tacticalDeployment));
                    SendAllLevelRoleTactical(tacticalDeployment.tacticDict[int.Parse(strArray[2])], ReturnCode.Fail);
                    TryRemove(int.Parse(strArray[1]), int.Parse(strArray[2]));
                    var RedisExist = GameManager.CustomeModule<TacticalDeploymentManager>().GetRoleTactic(tacticalDeployment.tacticDict[int.Parse(strArray[2])].RoleID, out List<TacticalDTO> roletactical);
                    //移除redis记录的暂存的个人阵法储存
                    if (RedisExist)
                    {
                        roletactical.Remove(tacticalDeployment.tacticDict[int.Parse(strArray[2])]);
                        RedisHelper.Hash.HashSet("TacticalDTO" + tacticalDeployment.tacticDict[int.Parse(strArray[2])].RoleID, tacticalDeployment.tacticDict[int.Parse(strArray[2])].RoleID.ToString(), roletactical);
                    }
                }
            }
        }
        /// <summary>
        /// 发送已生成的阵法至指定场景
        /// </summary>
        void SendTactical(int id, RoleEntity roleEntity)
        {
            if (GetExpendTacticalID()>1)
            {
                OperationData operationData = new OperationData();
                operationData.DataMessage = Utility.Json.ToJson(AllTacticalDeploymentDict);
                operationData.OperationCode = (byte)OperationCode.SyncCreatTactical;
                GameManager.CustomeModule<RoleManager>().SendMessage(roleEntity.RoleId, operationData);
                Utility.Debug.LogInfo("yzqData发送的全部阵法" + Utility.Json.ToJson(AllTacticalDeploymentDict) + "juese id " + roleEntity.RoleId);
            }
        }

    }
}
