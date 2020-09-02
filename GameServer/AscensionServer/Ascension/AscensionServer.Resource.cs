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
using System.Collections.Concurrent;
namespace AscensionServer
{
    public partial class AscensionServer : ApplicationBase
    {
        #region Properties
        Dictionary<int, ResourceUnitSetDTO> resUnitSetDict = new Dictionary<int, ResourceUnitSetDTO>();
        /// <summary>
        /// 资源单位集合的字典
        /// </summary>
        public Dictionary<int, ResourceUnitSetDTO> ResUnitSetDict { get { return resUnitSetDict; } private set { resUnitSetDict = value; } }
        /// <summary>
        /// 临时的占用资源单位容器，需要迭代
        /// </summary>
        HashSet<OccupiedUnitDTO> occupiedUnitSetCache = new HashSet<OccupiedUnitDTO>();
        //TODO  临时的占用资源单位容器，需要迭代
        public HashSet<OccupiedUnitDTO> OccupiedUnitSetCache { get { return occupiedUnitSetCache; }private set { occupiedUnitSetCache = value; } }
        #endregion

        #region Methods
        /// <summary>
        /// 对资源进行占用；
        /// 若资源占用成功，则将参数类对象加入被占用的缓存集合中
        /// </summary>
        /// <param name="occupiedUnit">占用资源的参数类对象</param>
        /// <returns>是否占用成功</returns>
        public bool OccupiedResUnit(OccupiedUnitDTO occupiedUnit)
        {
            if (!resUnitSetDict.ContainsKey(occupiedUnit.GlobalID))
                return false;
			bool result= resUnitSetDict[occupiedUnit.GlobalID].OccupiedResUnit(occupiedUnit.ResID);
            if (result)
                OccupiedUnitSetCache.Add(occupiedUnit);
            return result;
        }
        /// <summary>
        /// 释放被占用的资源
        /// </summary>
        /// <param name="occupiedUnit">占用资源的参数类对象</param>
        /// <returns>是否释放成功</returns>
        public bool ReleaseResUnit(OccupiedUnitDTO occupiedUnit)
        {
            //TODO ReleaseResUnit 释放被占用的资源 ，未完成！
            return false;
        }
        /// <summary>
        /// 初始化资源分布类，实现
        /// </summary>
        partial void ResourcesLoad()
        {
            Vector2 border = new Vector2(54000, 39000);
            var str = RegionJsonDataManager.GetRegionJsonContent(AscensionData.Region.Adventure, 0);
            HashSet<ResVariable> resVarSet = Utility.Json.ToObject<HashSet<ResVariable>>(str);
            foreach (var res in resVarSet)
            {
                var resSetDto = ConcurrentSingleton<ResourceCreator>.Instance.CreateRandomResourceSet(res, border);
                resUnitSetDict.Add(resSetDto.GlobalID,resSetDto);
            }
        }
        #endregion
    }
}