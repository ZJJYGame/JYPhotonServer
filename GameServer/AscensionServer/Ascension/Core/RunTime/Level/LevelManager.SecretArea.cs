using AscensionProtocol;
using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer 
{
    public partial class LevelManager
    {
        void ProcessSecretAreaHandlerS2C(int sessionId, OperationData packet)
        {
            var subCode = (SecretAreaOpCode)packet.OperationCode;
            switch (subCode)
            {
                case SecretAreaOpCode.Enter:
                    {
                        OnEnterSecretAreaLevelC2S(sessionId, packet);
                    }
                    break;
                case SecretAreaOpCode.Exit:
                    {
                        OnExitSecretAreaLevelC2S(sessionId, packet);
                    }
                    break;
                case SecretAreaOpCode.CmdInput:
                    {
                        OnSecretAreaCommandC2S(sessionId, packet);
                    }
                    break;
            }
        }
        void OnEnterSecretAreaLevelC2S(int sessionId, OperationData opData)
        {
            try
            {
                var entity = opData.BinParameters as C2SContainer;
                EnterSecretAreaScene(entity.Container.ContainerId, entity.Player.RoleId);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        void OnExitSecretAreaLevelC2S(int sessionId, OperationData opData)
        {
            try
            {
                var entity = opData.BinParameters as C2SContainer;
                ExitSecretAreaScene(entity.Container.ContainerId, entity.Player.RoleId);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
        }
        void OnSecretAreaCommandC2S(int sessionId, OperationData opData)
        {
            var input = opData.BinParameters as CmdInput;
            if (input != null)
            {
                if (secretAreaLevelEntityDict.TryGetValue(input.EntityContainer.ContainerId, out var sceneEntity))
                {
                    sceneEntity.OnCommandC2S(input);
                }
            }
        }
        bool EnterSecretAreaScene(int levelId, int roleId)
        {
            bool result = false;
            var hasScene = secretAreaLevelEntityDict.TryGetValue(levelId, out var sceneEntity);
            if (hasScene)
            {
                if (roleMgrInstance.TryGetValue(roleId, out var role))
                {
                    result = sceneEntity.EnterLevel(roleId, role);
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
                    sceneEntity = LevelEntity.Create(LevelTypeEnum.SecretArea, levelId);
                    SceneRefreshHandler += sceneEntity.OnRefresh;
                    result = sceneEntity.EnterLevel(role.RoleId, role);
                    if (result)
                    {
                        onRoleEnterLevel?.Invoke(role);
                    }
                    secretAreaLevelEntityDict.TryAdd(sceneEntity.LevelId, sceneEntity);
                }
            }
            return result;
        }
        bool ExitSecretAreaScene(int levelId, int roleId)
        {
            bool result = false;
            LevelEntity levelEntity;
            var hasScene = secretAreaLevelEntityDict.TryGetValue(levelId, out levelEntity);
            if (hasScene)
            {
                {
                    result = levelEntity.ExitLevel(roleId, out var role);
                    if (result)
                    {
                        onRoleExitLevel?.Invoke(role);
                    }
                    if (levelEntity.Empty)
                    {
                        secretAreaLevelEntityDict.TryRemove(levelId, out _);
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
