using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Cosmos;
using Protocol;
namespace AscensionServer
{
    public partial class LevelManager 
    {
        void ProcessAdventureHandlerS2C(int sessionId, OperationData packet)
        {
            var subCode = (AdventureOpCode)packet.SubOperationCode;
            switch (subCode)
            {
                case AdventureOpCode.Enter:
                    {
                        OnEnterAdventureLevelC2S(sessionId, packet);
                    }
                    break;
                case AdventureOpCode.Exit:
                    {
                        OnExitAdventureLevelC2S(sessionId, packet);
                    }
                    break;
                case AdventureOpCode.CmdInput:
                    {
                        OnAdventureCommandC2S(sessionId, packet);
                    }
                    break;
            }
        }
        void OnEnterAdventureLevelC2S(int sessionId, OperationData opData)
        {
            try
            {
                var entity = opData.DataContract as C2SContainer;
                EnterAdventureScene(entity.Container.ContainerId, entity.Player.PlayerId);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        void OnExitAdventureLevelC2S(int sessionId, OperationData opData)
        {
            try
            {
                var entity = opData.DataContract as C2SContainer;
                ExitAdventureScene(entity.Container.ContainerId, entity.Player.PlayerId);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        void OnAdventureCommandC2S(int sessionId, OperationData opData)
        {
            var input = opData.DataContract as C2SInput;
            if (input != null)
            {
                if (adventureLevelEntityDict.TryGetValue(input.EntityContainer.ContainerId, out var sceneEntity))
                {
                    sceneEntity.OnCommandC2S(input);
                }
            }
        }
        bool EnterAdventureScene(int levelId, int roleId)
        {
            bool result = false;
            var hasScene = adventureLevelEntityDict.TryGetValue(levelId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.TryGetValue(roleId, out var role))
                {
                    result = sceneEntity.TryAdd(roleId, role);
                    if (result)
                    {
                        onRoleEnterLevel?.Invoke(role);
                    }
                }
            }
            else
            {
                if (roleMgrInstance.TryGetValue(roleId, out var role))
                {
                    sceneEntity = LevelEntity.Create(LevelTypeEnum.Adventure, levelId);
                    SceneRefreshHandler += sceneEntity.OnRefresh;
                    result = sceneEntity.TryAdd(role.RoleId, role);
                    if (result)
                    {
                        onRoleEnterLevel?.Invoke(role);
                    }
                    adventureLevelEntityDict.TryAdd(sceneEntity.LevelId, sceneEntity);
                }
            }
            return result;
        }
        bool ExitAdventureScene( int levelId, int roleId)
        {
            bool result = false;
            LevelEntity levelEntity;
            var hasScene = adventureLevelEntityDict.TryGetValue(levelId, out levelEntity);
            if (hasScene)
            {
                {
                    result = levelEntity.TryRemove(roleId, out var role);
                    if (result)
                    {
                        onRoleExitLevel?.Invoke(role);
                    }
                    if (levelEntity.Empty)
                    {
                        adventureLevelEntityDict.TryRemove(levelId, out _);
                        CosmosEntry.ReferencePoolManager.Despawn(levelEntity);
                        SceneRefreshHandler -= levelEntity.OnRefresh;
                    }
                    if (roleMgrInstance.ContainsKey(roleId))
                    {

                    }
                }
            }
            return result;
        }
    }
}
