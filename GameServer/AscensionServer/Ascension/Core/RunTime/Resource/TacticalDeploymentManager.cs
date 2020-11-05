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
            }

            return result;
        }

        /// <summary>
        /// 获取是否有可用的已生成ID,没有则使用自增ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetExpendTacticalID()
        {
            int id = AllTacticalDeploymentDict.Count + 1;
            if (ExpendTacticalID.Count > 0)
            {
                return id = ExpendTacticalID[0];
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
        public bool TryRemove(int levelid, int id)
        {
            var result = TryGetValue(levelid, out var tacticalDeployment);
            if (result)
            {
                var Exist = tacticalDeployment.tacticDict.TryGetValue(id, out var tactical);
                if (Exist)
                {
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
        public bool TryAddExist(TacticalDTO tacticalDTO, int levelid = 0)
        {
            TacticalDeploymentDTO tacticalDeploymentDTO;
            var result = TryGetValue(levelid, out tacticalDeploymentDTO);
            if (result)
            {
                return tacticalDeploymentDTO.tacticDict.TryAdd(tacticalDTO.ID, tacticalDTO);
            }
            return result;
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
                roletacticaltemp.Remove(roleid);
            }
            return result;
        }

        /// <summary>
        /// 发送已生成的阵法至指定场景
        /// </summary>
        void SendTactical(int id, RoleEntity roleEntity)
        {
            OperationData operationData = new OperationData();
            operationData.DataMessage = Utility.Json.ToJson(AllTacticalDeploymentDict);
            operationData.OperationCode = (byte)OperationCode.SyncCreatTactical;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleEntity.RoleId, operationData);
            Utility.Debug.LogInfo("yzqData发送的全部阵法" + Utility.Json.ToJson(AllTacticalDeploymentDict));
        }
    }
}
