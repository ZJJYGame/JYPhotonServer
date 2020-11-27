using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionServer.Model;
using RedisDotNet;
using Cosmos;
using Protocol;
using AscensionProtocol;
using AscensionProtocol.DTO;
namespace AscensionServer
{
    [ImplementProvider]
    public class RecordAdventureSkillHelper : IRecordAdventureSkillHelper
    {
        /// <summary>
        /// 玩家进入历练主动发送历练技能布局
        /// </summary>
        /// <param name="roleEntity"></param>
        public void RecordRoleSkill(RoleEntity roleEntity)
        {
            Utility.Debug.LogInfo("yzqData发送技能布局" + roleEntity.RoleId);

            if (RedisHelper.KeyExistsAsync(RedisKeyDefine._SkillLayoutPerfix+ roleEntity.RoleId).Result)
            {
                var dict = RedisHelper.Hash.HashGet<AdventureSkillLayoutDTO>(RedisKeyDefine._SkillLayoutPerfix+ roleEntity.RoleId, roleEntity.RoleId.ToString());
                OperationData operationData = new OperationData();
                operationData.DataMessage = Utility.Json.ToJson(dict);
                operationData.OperationCode = (byte)OperationCode.RefreshSkillLayout;
                Utility.Debug.LogInfo("yzqData发送技能布局"+ Utility.Json.ToJson(dict));
                GameManager.CustomeModule<RoleManager>().SendMessage(roleEntity.RoleId, operationData);
            }
        }
    }
}
