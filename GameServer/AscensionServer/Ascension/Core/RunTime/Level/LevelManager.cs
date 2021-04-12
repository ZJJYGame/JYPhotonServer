using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
using AscensionProtocol;
namespace AscensionServer
{
    /// <summary>
    /// 场景管理器，管理野外高同步的数据；
    /// </summary>
    [Module]
    public partial class LevelManager : Cosmos. Module,ILevelManager
    {
        /// <summary>
        /// 历练场景字典;
        /// </summary>
        ConcurrentDictionary<int, LevelEntity> adventureLevelEntityDict;
        /// <summary>
        /// 秘境场景字典;
        /// </summary>
        ConcurrentDictionary<long , LevelEntity> secretAreaLevelEntityDict;

        long latestTime;
        int updateInterval = ApplicationBuilder._MSPerTick;

        Action sceneRefreshHandler;
        event Action SceneRefreshHandler
        {
            add { sceneRefreshHandler += value; }
            remove{sceneRefreshHandler -= value;}
        }

        IRoleManager roleMgrInstance;
        Action<RoleEntity> onRoleEnterLevel;
        /// <summary>
        /// 角色进入场景事件
        /// </summary>
        public event Action<RoleEntity> OnRoleEnterLevel
        {
            add { onRoleEnterLevel += value; }
            remove { onRoleEnterLevel -= value; }
        }

        Action<RoleEntity> onRoleExitLevel;
        /// <summary>
        /// 角色离开场景事件
        /// </summary>
        public event Action<RoleEntity> OnRoleExitLevel
        {
            add { onRoleExitLevel += value; }
            remove { onRoleExitLevel -= value; }
        }
        public override void OnPreparatory()
        {
            adventureLevelEntityDict = new ConcurrentDictionary<int, LevelEntity>();
            secretAreaLevelEntityDict = new ConcurrentDictionary<long, LevelEntity>();

            latestTime = Utility.Time.MillisecondNow() + updateInterval;
            roleMgrInstance = GameEntry.RoleManager;

            CommandEventCore.Instance.AddEventListener(ProtocolDefine.OPR_PLYAER_LOGOFF, OnPlayerLogoff);

            CommandEventCore.Instance.AddEventListener((byte)OperationCode.AdventureArea, ProcessAdventureHandlerS2C);
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SecretArea, ProcessSecretAreaHandlerS2C);
        }
        public override void OnRefresh()
        {
            if (IsPause)
                return;
            var now = Utility.Time.MillisecondNow();
            if (latestTime <= now)
            {
                latestTime = now + updateInterval;
                sceneRefreshHandler?.Invoke();
            }
        }

        /// <summary>
        ///场景是否包含有角色； 
        /// </summary>
        public bool LevelHasRole(LevelTypeEnum levelType,int levelId, int roleId)
        {
            switch (levelType)
            {
                case LevelTypeEnum.Adventure:
                    {
                        if (adventureLevelEntityDict.TryGetValue(levelId, out var levelEntity))
                        {
                            return levelEntity.HasRole(roleId);
                        }
                    }
                    break;
                case LevelTypeEnum.SecretArea:
                    {
                        if (secretAreaLevelEntityDict.TryGetValue(levelId, out var levelEntity))
                        {
                            return levelEntity.HasRole(roleId);
                        }
                    }
                    break;
            }
            return false;
        }
        /// <summary>
        ///广播消息到指定场景，若场景不存在，则不执行； 
        /// </summary>
        public void SendMessageToLevelS2C(LevelTypeEnum levelType,int levelId,OperationData opData)
        {
            switch (levelType)
            {
                case LevelTypeEnum.Adventure:
                    {
                        if (adventureLevelEntityDict.TryGetValue(levelId, out var levelEntity))
                            levelEntity.SndMsg2AllS2C(opData);
                    }
                    break;
                case LevelTypeEnum.SecretArea:
                    {
                        if (secretAreaLevelEntityDict.TryGetValue(levelId, out var levelEntity))
                            levelEntity.SndMsg2AllS2C(opData);
                    }
                    break;
            }
        }

        void OnPlayerLogoff(int sessionId, OperationData opData)
        {
            var roleEntity = opData.DataMessage as RoleEntity;
            if (roleEntity != null)
            {
                if (roleEntity.TryGetValue(typeof(LevelEntity), out var entity))
                {
                    var levelEntity = entity as LevelEntity;
                    levelEntity.ExitLevel(roleEntity.RoleId,out _ );
                    Utility.Debug.LogWarning($"RoleId:{roleEntity.RoleId} ;SessionId:{roleEntity.SessionId}由于强退，尝试从Level:{levelEntity.LevelId}中移除");
                    if (levelEntity.Empty)
                    {
                        adventureLevelEntityDict.TryRemove((int)levelEntity.LevelId, out _);
                        CosmosEntry.ReferencePoolManager.Despawn(levelEntity);
                        SceneRefreshHandler -= levelEntity.OnRefresh;
                    }
                }
            }
        }
    }
}


