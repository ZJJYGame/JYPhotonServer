using System;
using System.Collections.Generic;
using System.Text;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using AscensionServer.Model;
using ExitGames.Concurrency.Fibers;
using AscensionRegion;
using AscensionData;
using Cosmos;

namespace AscensionServer
{
    public partial class AscensionServer : ApplicationBase
    {
        #region Properties
        public Dictionary<int, HashSet<ResourcesDTO>> resDic = new Dictionary<int, HashSet<ResourcesDTO>>();
        Dictionary<int, ResourceUnitSetDTO> resUnitSetDict = new Dictionary<int, ResourceUnitSetDTO>();
		/// <summary>
        /// 资源单位集合的字典
        /// </summary>
		public Dictionary<int, ResourceUnitSetDTO> ResSetDict { get { return resUnitSetDict; }private set { resUnitSetDict = value; } }
        #endregion

        #region Methods
		/// <summary>
        /// 占用资源
        /// </summary>
        /// <param name="globalID">资源的全局ID</param>
        /// <param name="resID">资源被分配时的ID</param>
        /// <returns>是否占用成功</returns>
		public bool OccupiedResUnit(int globalID,int resID)
        {
            if (!resUnitSetDict.ContainsKey(globalID))
                return false;
            return resUnitSetDict[globalID].OccupiedResUnit(resID);
        }
        /// <summary>
        /// 释放被占用的资源
        /// <param name="globalID">资源的全局ID</param>
        /// <param name="resID">资源被分配时的ID</param>
        /// <returns>是否释放成功</returns>
        public bool ReleaseResUnit(int globalID,int resID)
        {
            //TODO ReleaseResUnit 释放被占用的资源 ，未完成！
            return false;
        }
        /// <summary>
        /// 初始化资源分布类，实现
        /// </summary>
        partial void ResourcesLoad()
        {
            Vector2 border = new Vector2(5400, 3900);
            var str = RegionJsonDataManager.GetRegionJsonContent(AscensionData.Region.Adventure, 0);
            HashSet<ResVariable> resVarSet = Utility.Json.ToObject<HashSet<ResVariable>>(str);
            foreach (var res in resVarSet)
            {
                var resSetDto = Singleton<ResourceCreator>.Instance.CreateRandomResourceSet(res, border);
                resUnitSetDict.Add(resSetDto.GlobalID,resSetDto);
            }
            _Log.Info(Utility.Json.ToJson(resUnitSetDict));
        }
        #endregion
    }
}