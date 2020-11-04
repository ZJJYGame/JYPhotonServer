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
    public class GameResourceManager : Module<GameResourceManager>
    {
        /// <summary>
        /// 资源单位集合的字典
        /// </summary>
        public Dictionary<int, ResourceUnitSetDTO> ResUnitSetDict { get; private set; }
        IRecordAdventureSkillHelper recordAdventureSkillHelper;
        /// <summary>
        /// 临时的占用资源单位容器，需要迭代
        /// </summary>
        //TODO  临时的占用资源单位容器，需要迭代
        public HashSet<OccupiedUnitDTO> OccupiedUnitSetCache { get; private set; }
        public override void OnInitialization()
        {

            recordAdventureSkillHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IRecordAdventureSkillHelper>();

            ResUnitSetDict = new Dictionary<int, ResourceUnitSetDTO>();
            OccupiedUnitSetCache = new HashSet<OccupiedUnitDTO>();
        }
        public override void OnPreparatory()
        {
            ResourcesLoad();
            GameManager.CustomeModule<LevelManager>().OnRoleEnterLevel += SendResources;
        }


        /// <summary>
        /// 对资源进行占用；
        /// 若资源占用成功，则将参数类对象加入被占用的缓存集合中
        /// </summary>
        /// <param name="occupiedUnit">占用资源的参数类对象</param>
        /// <returns>是否占用成功</returns>
        public bool OccupiedResUnit(OccupiedUnitDTO occupiedUnit)
        {
            if (!ResUnitSetDict.ContainsKey(occupiedUnit.GlobalID))
                return false;
            bool result = ResUnitSetDict[occupiedUnit.GlobalID].OccupiedResUnit(occupiedUnit.ResID);
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
         void ResourcesLoad()
        {
            Vector2 border = new Vector2(54000, 39000);
            var str = RegionJsonDataManager.GetRegionJsonContent(AscensionData.Region.Adventure, 0);
            HashSet<ResVariable> resVarSet = Utility.Json.ToObject<HashSet<ResVariable>>(str);
            foreach (var res in resVarSet)
            {
                var resSetDto = ConcurrentSingleton<ResourceCreator>.Instance.CreateRandomResourceSet(res, border);
                ResUnitSetDict.Add(resSetDto.GlobalID, resSetDto);
            }
        }

        /// <summary>
        /// 发送已生成资源至指定场景
        /// </summary>
        void SendResources(int id, RoleEntity roleEntity)
        {
            OperationData operationData = new OperationData();
            operationData.DataMessage = Utility.Json.ToJson(ResUnitSetDict); 
            operationData.OperationCode = (byte)OperationCode.SyncResources;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleEntity.RoleId, operationData);
            Utility.Debug.LogInfo("yzqData" + Utility.Json.ToJson(ResUnitSetDict));
            //roleEntity.SendEvent((byte)EventCode.RelieveOccupiedResourceUnit, date);
            recordAdventureSkillHelper.RecordRoleSkill(roleEntity);
        }
    }
}
