using System;
using System.Collections.Concurrent;
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
        ConcurrentDictionary<int, LevelEntity> levelEntityDict = new ConcurrentDictionary<int, LevelEntity>();
        long latestTime;
        int updateInterval = ApplicationBuilder._MSPerTick;
        Action sceneRefreshHandler;
        event Action SceneRefreshHandler
        {
            add { sceneRefreshHandler += value; }
            remove{sceneRefreshHandler -= value;}
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
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLYAER_INPUT, OnCommandC2S);
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLYAER_LOGOFF, OnPlayerLogoff);
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLAYER_ENTER, OnEnterLevelC2S);
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPERATION_PLAYER_EXIT, OnExitLevelC2S);

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
                if (levelEntityDict.TryGetValue(input.EntityContainerId, out var sceneEntity))
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
            var hasScene = levelEntityDict.TryGetValue(sceneId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.TryGetValue(roleId, out var role))
                {
                    result = sceneEntity.TryAdd(roleId, role);
                }
            }
            else
            {
                if (roleMgrInstance.TryGetValue(roleId, out var role))
                {
                    sceneEntity = LevelEntity.Create(sceneId);
                    SceneRefreshHandler += sceneEntity.OnRefresh;
                    result = sceneEntity.TryAdd(role.RoleId, role);
                    levelEntityDict.TryAdd(sceneEntity.LevelId, sceneEntity);
                }
            }
#else

#endif
            return result;
        }
        public bool EnterScene(int sceneId, RoleEntity role)
        {
            bool result = false;
#if SERVER

            var hasScene = levelEntityDict.TryGetValue(sceneId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.ContainsKey(role.RoleId))
                {
                    result = sceneEntity.TryAdd(role.RoleId, role);
                }
            }
            else
            {
                if (roleMgrInstance.ContainsKey(role.RoleId))
                {
                    sceneEntity = LevelEntity.Create(sceneId);
                    SceneRefreshHandler += sceneEntity.OnRefresh;
                    result = sceneEntity.TryAdd(role.RoleId, role);
                    levelEntityDict.TryAdd(sceneEntity.LevelId, sceneEntity);
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
            LevelEntity levelEntity;
            var hasScene = levelEntityDict.TryGetValue(sceneId, out levelEntity);
            if (hasScene)
            {
                {
                    result = levelEntity.TryRemove(roleId);
                    if (levelEntity.Empty)
                    {
                        levelEntityDict.TryRemove(sceneId, out _);
                        GameManager.ReferencePoolManager.Despawn(levelEntity);
                        SceneRefreshHandler -= levelEntity.OnRefresh;
                    }
                    if (roleMgrInstance.ContainsKey(roleId))
                    {

                    }        
                }
            }
#else

#endif
            return result;
        }
        public bool ExitScene(int sceneId, RoleEntity role)
        {
            bool result = false;
#if SERVER
            LevelEntity levelEntity;
            var hasScene = levelEntityDict.TryGetValue(sceneId, out levelEntity);
            if (hasScene)
            {
                result = levelEntity.TryRemove(role.RoleId);
                if (levelEntity.Empty)
                {
                    levelEntityDict.TryRemove(sceneId, out _);
                    GameManager.ReferencePoolManager.Despawn(levelEntity);
                    SceneRefreshHandler -= levelEntity.OnRefresh;
                }
                if (roleMgrInstance.ContainsKey(role.RoleId))
                {
       
                }
            }
#else

#endif
            return result;
        }
        void OnPlayerLogoff(OperationData opData)
        {
            var roleEntity = opData.DataMessage as RoleEntity;
            if (roleEntity != null)
            {
                if (roleEntity.TryGetValue(typeof(LevelEntity), out var entity))
                {
                    var levelEntity = entity as LevelEntity;
                    levelEntity.TryRemove(roleEntity.RoleId);
                    Utility.Debug.LogWarning($"RoleId:{roleEntity.RoleId} ;SessionId:{roleEntity.SessionId}由于强退，尝试从Level:{levelEntity.LevelId}中移除");
                    if (levelEntity.Empty)
                    {
                        levelEntityDict.TryRemove(levelEntity.LevelId, out _);
                        GameManager.ReferencePoolManager.Despawn(levelEntity);
                        SceneRefreshHandler -= levelEntity.OnRefresh;
                    }
                }
            }
        }
        void OnEnterLevelC2S(OperationData opData)
        {
            try
            {
                var entity = opData.DataContract as C2SEntityContainer;
                EnterScene(entity.EntityContainerId, entity.Player.PlayerId);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        void OnExitLevelC2S(OperationData opData)
        {
            try
            {
                var entity = opData.DataContract as C2SEntityContainer;
                ExitScene(entity.EntityContainerId, entity.Player.PlayerId);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
    }
}
