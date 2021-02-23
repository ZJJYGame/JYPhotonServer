using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;

namespace AscensionServer
{
    public interface ILevelManager:IModuleManager
    {
        event Action<RoleEntity> OnRoleEnterLevel;
        /// <summary>
        /// 角色离开场景事件
        /// </summary>
        event Action<RoleEntity> OnRoleExitLevel;
        /// <summary>
        ///场景是否包含有角色； 
        /// </summary>
        bool LevelHasRole(int levelId, int roleId);
        /// <summary>
        ///广播消息到指定场景，若场景不存在，则不执行； 
        /// </summary>
        void SendMessageToLevelS2C(int levelId, OperationData opData);
        /// <summary>
        ///玩家或peer进入场景 
        /// </summary>
        bool EnterScene(int levelId, int roleId);
        bool EnterScene(int levelId, RoleEntity role);
        bool ExitScene(int levelId, int roleId);
        bool ExitScene(int levelId, RoleEntity role);
    }
}


