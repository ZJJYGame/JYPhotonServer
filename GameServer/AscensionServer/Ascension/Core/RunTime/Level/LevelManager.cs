using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;

namespace AscensionServer
{
    /// <summary>
    /// 场景管理器，管理野外高同步的数据；
    /// </summary>
    [CustomeModule]
    public class LevelManager : Module<LevelManager>
    {
#if SERVER
        Dictionary<int, LevelEntity> sceneEntityDict = new Dictionary<int, LevelEntity>();
        long latestTime;
        int updateInterval = ApplicationBuilder._MSPerTick;
        Action sceneRefreshHandler;
        event Action SceneRefreshHandler
        {
            add { sceneRefreshHandler += value; }
            remove
            {
                try { sceneRefreshHandler -= value; }
                catch (Exception e) { Utility.Debug.LogError(e); }
            }
        }
        PeerManager peerMgrInstance;
#else
        LevelEntity levelEntity = new LevelEntity();
#endif
        RoleManager roleMgrInstance;
        public override void OnPreparatory()
        {
#if SERVER
            latestTime = Utility.Time.MillisecondNow() + updateInterval;
            peerMgrInstance = GameManager.CustomeModule<PeerManager>();
            roleMgrInstance = GameManager.CustomeModule<RoleManager>();
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLYAERINPUT, OnCommandC2S);
#else
            roleMgrInstance = Facade.CustomeModule<RoleManager>();
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLYAERINPUT, OnCommandS2C);
#endif
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
                sceneRefreshHandler?.Invoke();
            }
#else

#endif
        }
#if SERVER
        public void OnCommandC2S(OperationData opData)
        {
            var input = opData.DataContract as C2SInput;
            if (input != null)
            {
                if (sceneEntityDict.TryGetValue(input.EntityContainerId, out var sceneEntity))
                {
                    sceneEntity.OnCommandC2S(input);
                }
            }
        }
#else
        public void OnCommandS2C(OperationData opData)
        {
            levelEntity?.OnCommandS2C(opData.DataContract);
        }
#endif
        /// <summary>
        ///玩家或peer进入场景 
        /// </summary>
        public bool EnterScene(int sceneId, int roleId)
        {
            bool result = false;
#if SERVER
            var hasScene = sceneEntityDict.TryGetValue(sceneId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.TryGetValue(roleId, out var role))
                {
                    result = sceneEntity.TryAdd(roleId, role);
                }
            }
            else
            {
                sceneEntity = LevelEntity.Create(sceneId);
                if (roleMgrInstance.TryGetValue(roleId, out var role))
                {
                    result = sceneEntity.TryAdd(roleId, role);
                }
            }
#else

#endif
            return result;
        }
        public bool EnterScene(int sceneId, IRoleEntity role)
        {
            bool result = false;
#if SERVER

            var hasScene = sceneEntityDict.TryGetValue(sceneId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.ContainsKey(role.RoleId))
                {
                    result = sceneEntity.TryAdd(role.RoleId, role);
                }
            }
            else
            {
                sceneEntity = LevelEntity.Create(sceneId);
                if (roleMgrInstance.ContainsKey(role.RoleId))
                {
                    result = sceneEntity.TryAdd(role.RoleId, role);
                }
            }
#else

#endif
            return result;
        }
        public bool ExitScene(int sceneId, int roleId)
        {
            bool result = false;
#if SERVER
            var hasScene = sceneEntityDict.TryGetValue(sceneId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.ContainsKey(roleId))
                {
                    result = sceneEntity.TryRemove(roleId);
                    if (sceneEntity.PlayerCount <= 0)
                    {
                        GameManager.ReferencePoolManager.Despawn(sceneEntity);
                    }
                }
            }
#else

#endif
            return result;
        }
        public bool ExitScene(int sceneId, IRoleEntity role)
        {
            bool result = false;
#if SERVER
            var hasScene = sceneEntityDict.TryGetValue(sceneId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.ContainsKey(role.RoleId))
                {
                    result = sceneEntity.TryRemove(role.RoleId);
                    if (sceneEntity.PlayerCount <= 0)
                    {
                        GameManager.ReferencePoolManager.Despawn(sceneEntity);
                    }
                }
            }
#else

#endif
            return result;
        }
    }
}
