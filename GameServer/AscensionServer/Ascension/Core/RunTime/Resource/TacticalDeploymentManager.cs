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
namespace AscensionServer
{
    [CustomeModule]
    public class TacticalDeploymentManager : Module<TacticalDeploymentManager>
    {
        /// <summary>
        /// 所有部署的阵法储存集合
        /// </summary>
        Dictionary<int, TacticalDeploymentDTO> AllTacticalDeploymentDict = new Dictionary<int, TacticalDeploymentDTO>();
        /// <summary>
        /// 消耗过的阵法自生成ID，用于重复使用
        /// </summary>
        List<int> ExpendTacticalID = new List<int>();

        public bool ContainsKey(int levelId)
        {
            return AllTacticalDeploymentDict.ContainsKey(levelId);
        }

        /// <summary>
        /// 获取是否有可用的已生成ID,没有则使用自增ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetExpendTacticalID()
        {
          int  id = AllTacticalDeploymentDict.Count+1;
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


        public bool TryRemove(int levelid,int id)
        {
            var result = TryGetValue(levelid, out var tacticalDeployment);
            if (result)
            {
               var exit= tacticalDeployment.tacticDict.TryGetValue(id,out var tactical);
                if (exit)
                {
                    tacticalDeployment.tacticDict.Remove(tactical.ID);
                }
                return exit;
            }
            return result;
        }

        public void ReplacefFirstTactical()
        {


        }
    }
}
