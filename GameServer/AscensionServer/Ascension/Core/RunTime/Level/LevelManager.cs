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
        Action<int, RoleEntity> onRoleEnterLevel;
        /// <summary>
        /// 角色进入场景事件
        /// </summary>
        public event Action<int, RoleEntity> OnRoleEnterLevel
        {
            add { onRoleEnterLevel += value; }
            remove { onRoleEnterLevel -= value; }
        }
        Action<int, RoleEntity> onRoleExitLevel;
        /// <summary>
        /// 角色离开场景事件
        /// </summary>
        public event Action<int, RoleEntity> OnRoleExitLevel
        {
            add { onRoleExitLevel += value; }
            remove { onRoleExitLevel -= value; }
        }
        public override void OnPreparatory()
        {
#if SERVER
            latestTime = Utility.Time.MillisecondNow() + updateInterval;
            peerMgrInstance = GameManager.CustomeModule<PeerManager>();
            roleMgrInstance = GameManager.CustomeModule<RoleManager>();
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPR_PLYAER_INPUT, OnCommandC2S);
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPR_PLYAER_LOGOFF, OnPlayerLogoff);
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPR_PLAYER_ENTER, OnEnterLevelC2S);
            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPR_PLAYER_EXIT, OnExitLevelC2S);
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

        /// <summary>
        ///场景是否包含有角色； 
        /// </summary>
        public bool LevelHasRole(int levelId, int roleId)
        {
            if (levelEntityDict.TryGetValue(levelId, out var levelEntity))
            {
                return levelEntity.ContainsKey(roleId);
            }
            return false;
        }
        /// <summary>
        ///广播消息到指定场景，若场景不存在，则不执行； 
        /// </summary>
        public void SendMsg2AllLevelRoleS2C(int levelId,OperationData opData)
        {
            if (levelEntityDict.TryGetValue(levelId, out var levelEntity))
            {
                levelEntity.SndMsg2AllS2C(opData);
            }
        }
#endif
        /// <summary>
        ///玩家或peer进入场景 
        /// </summary>
        public bool EnterScene(int levelId, int roleId)
        {
            bool result = false;
#if SERVER
            var hasScene = levelEntityDict.TryGetValue(levelId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.TryGetValue(roleId, out var role))
                {
                    result = sceneEntity.TryAdd(roleId, role);
                    if (result)
                    {
                        onRoleEnterLevel?.Invoke(levelId, role);
                    }
                }
            }
            else
            {
                if (roleMgrInstance.TryGetValue(roleId, out var role))
                {
                    sceneEntity = LevelEntity.Create(levelId);
                    SceneRefreshHandler += sceneEntity.OnRefresh;
                    result = sceneEntity.TryAdd(role.RoleId, role);
                    if (result)
                    {
                        onRoleEnterLevel?.Invoke(levelId,role);
                    }
                    levelEntityDict.TryAdd(sceneEntity.LevelId, sceneEntity);
                }
            }
#else

#endif
            return result;
        }
        public bool EnterScene(int levelId, RoleEntity role)
        {
            bool result = false;
#if SERVER

            var hasScene = levelEntityDict.TryGetValue(levelId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.ContainsKey(role.RoleId))
                {
                    result = sceneEntity.TryAdd(role.RoleId, role);
                    if (result)
                    {
                        onRoleEnterLevel?.Invoke(levelId, role);
                    }
                }
            }
            else
            {
                if (roleMgrInstance.ContainsKey(role.RoleId))
                {
                    sceneEntity = LevelEntity.Create(levelId);
                    SceneRefreshHandler += sceneEntity.OnRefresh;
                    result = sceneEntity.TryAdd(role.RoleId, role);
                    if (result)
                    {
                        onRoleEnterLevel?.Invoke(levelId,role);
                    }
                    levelEntityDict.TryAdd(sceneEntity.LevelId, sceneEntity);
                }
            }
#else

#endif
            return result;
        }
        public bool ExitScene(int levelId, int roleId)
        {
            bool result = false;
#if SERVER
            LevelEntity levelEntity;
            var hasScene = levelEntityDict.TryGetValue(levelId, out levelEntity);
            if (hasScene)
            {
                {
                    result = levelEntity.TryRemove(roleId,out var role);
                    if (result)
                    {
                        onRoleExitLevel?.Invoke(levelId,role);
                    }
                    if (levelEntity.Empty)
                    {
                        levelEntityDict.TryRemove(levelId, out _ );
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
        public bool ExitScene(int levelId, RoleEntity role)
        {
            bool result = false;
#if SERVER
            LevelEntity levelEntity;
            var hasScene = levelEntityDict.TryGetValue(levelId, out levelEntity);
            if (hasScene)
            {
                result = levelEntity.TryRemove(role.RoleId);
                if (result)
                {
                    onRoleExitLevel?.Invoke(levelId,role);
                }
                if (levelEntity.Empty)
                {
                    levelEntityDict.TryRemove(levelId, out _);
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
#if SERVER
        void OnCommandC2S(OperationData opData)
        {
            var input = opData.DataContract as C2SInput;
            if (input != null)
            {
                if (levelEntityDict.TryGetValue(input.EntityContainer.EntityContainerId, out var sceneEntity))
                {
                    sceneEntity.OnCommandC2S(input);
                }
            }
            else
            {
                Utility.Debug.LogError("C2SInput  null");
            }
        }
#else
         void OnCommandS2C(OperationData opData)
        {
            levelEntity?.OnCommandS2C(opData.DataContract);
        }
#endif
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
