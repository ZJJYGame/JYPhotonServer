using Cosmos;
using AscensionProtocol.DTO;
using Protocol;
using AscensionProtocol;
using System.Collections.Generic;

namespace AscensionServer
{
    [CustomeModule]
    public class MapResourceManager : Module<MapResourceManager>
    {
        IMapResourceHelper mapResourceHelper;
        long latestTime;
        const int updateInterval = ApplicationBuilder.MapResourceRefreshInterval;
        public override void OnPreparatory()
        {
            GameManager.CustomeModule<LevelManager>().OnRoleEnterLevel += OnRoleEnterMap;
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.TakeUpResource, TakeUpResourceC2S);
            mapResourceHelper = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IMapResourceHelper>();
            mapResourceHelper?.LoadMapResource();
            latestTime = Utility.Time.MillisecondNow() + updateInterval;
        }
        public override void OnRefresh()
        {
            if (IsPause)
                return;
#if SERVER
            var now = Utility.Time.MillisecondNow();
            if (latestTime <= now)
            {
                latestTime = now + updateInterval;
                mapResourceHelper.OnRefreshResource();
            }
#endif
        }
        /// <summary>
        /// 占用资源 客户端->服务器
        /// </summary>
        void TakeUpResourceC2S(OperationData opData)
        {
            mapResourceHelper.TakeUpMapResource(opData.DataMessage  as Dictionary<byte, object>);
        }
        /// <summary>
        /// 发送已生成资源至指定场景
        /// </summary>
        void OnRoleEnterMap(RoleEntity roleEntity)
        {
            mapResourceHelper.OnRoleEnterMap(roleEntity);
        }
    }
}
