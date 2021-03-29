using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Cosmos;

namespace AscensionServer
{
    public interface ITacticalManager:IModuleManager
    {
        /// <summary>
        /// 创建新的阵法实体，并添加到临时集合
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        bool TacticalCreateAdd(TacticalDTO tacticalDTO);
        /// <summary>
        /// 移除暂存集合放入总集合
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="tacticalDTO"></param>
        /// <returns></returns>
        bool TryAddRemoveTactical(int roleid, out TacticalDTO tacticalDTO);
        /// <summary>
        /// 打断操作移除临时集合储存阵法
        /// </summary>
        /// <param name="roleid"></param>
        void TryRemoveTactical(int roleid);
        /// <summary>
        /// 获取自增ID
        /// </summary>
        /// <returns></returns>
        int GetExpendTacticalID();
        /// <summary>
        /// 获取当前地图块的所有阵法
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="tactical"></param>
        /// <returns></returns>
        bool TryGetValue(int levelid, out ConcurrentDictionary<int, TacticalDTO> tacticalDeployment);
        bool TryAdd(int levelid, TacticalEntity tacticalEntity);
        //TODO 判断当前地图块是否可以创建
        bool IsCreatTactic(int levelid = 0);
        /// <summary>
        /// 获取本地的暂存角色阵法
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="roletactical"></param>
        /// <returns></returns>
        void GetRoleTactic(int roleid, out List<TacticalDTO> roletactical);
        bool TryRemove(TacticalDTO tacticalDTO);
        /// <summary>
        /// 广播给当前场景所有人对阵法的操作
        /// </summary>
        /// <param name="tacticalDTO"></param>
        /// <param name="returnCode">成功为生成，失败为销毁</param>
        void SendAllLevelRoleTactical(TacticalDTO tacticalDTO, ReturnCode returnCode);
        /// <summary>
        /// 监听Redis删除后的回调
        /// </summary>
        void RedisDeleteCaback(string key);
   
        /// <summary>
        /// 储存每个角色的所有阵法
        /// </summary>
        /// <param name="roleid"></param>
        /// <param name="tacticalDTO"></param>
        void TryAddRoleAllTactical(int roleid, TacticalDTO tacticalDTO);
    }
}


