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

namespace AscensionServer
{
    class RecordAdventureSkillHelper : IRecordAdventureSkillHelper
    {
        public void RecordRoleSkill(RoleEntity roleEntity)
        {
            Utility.Debug.LogInfo("发送同步离线时间");

            if (RedisHelper.KeyExistsAsync("").Result)
            {
                var dict = RedisHelper.String.StringGet("");
                OperationData operationData = new OperationData();
                operationData.DataMessage = Utility.Json.ToJson(dict);
                operationData.OperationCode = (byte)OperationCode.RefreshSkillLayout;

                GameManager.CustomeModule<RoleManager>().SendMessage(roleEntity.RoleId, operationData);
            }
        }
    }
}
