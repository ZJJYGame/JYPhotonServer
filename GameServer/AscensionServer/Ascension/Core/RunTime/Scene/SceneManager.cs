using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 场景管理器，管理野外高同步的数据；
    /// </summary>
    [CustomeModule]
    public class SceneManager:Module<SceneManager>
    {
        Dictionary<int, SceneEntity> sceneEntityDict = new Dictionary<int, SceneEntity>();
        long latestTime;
        int updateInterval = ApplicationBuilder._MSPerTick;
        Action sceneRefreshHandler;
        event Action SceneRefreshHandler
        {
            add{sceneRefreshHandler += value;}
            remove
            {
                try{sceneRefreshHandler -= value;}
                catch (Exception e){Utility.Debug.LogError(e);}
            }
        }
        PeerManager peerMgrInstance;
        RoleManager roleMgrInstance;
        public override void OnPreparatory()
        {
            latestTime = Utility.Time.MillisecondNow() + updateInterval;
            peerMgrInstance = GameManager.CustomeModule<PeerManager>();
            roleMgrInstance= GameManager.CustomeModule<RoleManager>();
        }
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            var now= Utility.Time.MillisecondNow();
            if (latestTime <= now)
            {
                latestTime = now+updateInterval;
                sceneRefreshHandler?.Invoke();
            }
        }
        /// <summary>
        ///玩家或peer进入场景 
        /// </summary>
        public bool EnterScene(int sceneId,int roleId)
        {
            bool result = false;
            var hasScene= sceneEntityDict.TryGetValue(sceneId, out var sceneEntity);
            if (hasScene)
            {
                if( roleMgrInstance.TryGetValue(roleId,out var role))
                {
                    result= sceneEntity.TryAdd(roleId,role);
                }
            }
            else
            {
                 sceneEntity= SceneEntity.Create(sceneId);
                if (roleMgrInstance.TryGetValue(roleId, out var role))
                {
                    result = sceneEntity.TryAdd(roleId, role);
                }
            }
            return result;
        }
        public void EnterScene(int sceneId,IRoleEntity role)
        {
        }
        public bool ExitScene(int sceneId,int roleId)
        {
            bool result = false;
            var hasScene = sceneEntityDict.TryGetValue(sceneId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.ContainsKey(roleId))
                {
                    result = sceneEntity.TryRemove(roleId);
                    if (sceneEntity.PlayerCount <= 0)
                    {

                    }
                }
            }
            return result;
        }
    }
}
