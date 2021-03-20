using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public interface IBattleCharacterManager:IModuleManager
    {
        /// <summary>
        /// 添加玩家角色
        /// </summary>
        BattlePlayerEntity AddPlayerCharacter(int roomID, int RoleID,BattleFactionType battleFactionType);
        /// <summary>
        /// 添加宠物角色
        /// </summary>
        BattlePetEntity AddPetCharacter(int roomID, int roleID, BattleFactionType battleFactionType);
        /// <summary>
        /// 添加Ai角色
        /// </summary>
        BattleAIEntity AddAICharacter(int roomID, int aIID, BattleFactionType battleFactionType);
        /// <summary>
        /// 获取角色实体对象
        /// </summary>
        BattleCharacterEntity GetCharacterEntity(int uniqueID);
    }
}
