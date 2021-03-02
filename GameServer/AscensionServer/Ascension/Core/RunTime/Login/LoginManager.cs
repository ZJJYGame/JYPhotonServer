using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;

namespace AscensionServer
{
    [Module]
    public class LoginManager : Cosmos.Module, ILoginManager
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncRole, ProcessHandlerC2S);
        }
        /// <summary>
        /// 获取账号下的角色ID；
        /// </summary>
        /// <param name="user">需要查询的用户</param>
        /// <returns>账号对应的角色ID数组</returns>
        public int[] GetAccountRolesId(User user)
        {
            string _uuid = NHibernateQuerier.Query<User>("Account", user.Account).UUID;
            var userRoleObj = NHibernateQuerier.Query<UserRole>("UUID", _uuid);
            string roleIdListJson = userRoleObj.RoleIDArray;
            var roleIdListStr = Utility.Json.ToObject<List<string>>(roleIdListJson);
            var length = roleIdListStr.Count;
            var roleIdListInt = new List<int>();
            for (int i = 0; i < length; i++)
            {
                roleIdListInt.Add(int.Parse(roleIdListStr[i]));
            }
            return length == 0 ? null : roleIdListInt.ToArray();
        }
        void ProcessHandlerC2S(int sessionId, OperationData packet)
        {
            var dict = Utility.Json.ToObject<Dictionary<byte, object>>(Convert.ToString(packet.DataMessage));
            var subCode = (SubOperationCode)Convert.ToByte(Utility.GetValue(dict, (byte)OperationCode.SubOperationCode));
            switch (subCode)
            {
                case SubOperationCode.Add:
                    break;
                case SubOperationCode.Remove:
                    break;
                case SubOperationCode.Get:
                    {
                        GetAccountRolesS2C(sessionId, dict);
                    }
                    break;
                case SubOperationCode.Update:
                    break;
                case SubOperationCode.Verify:
                    break;
            }
        }
        /// <summary>
        ///客户端从服务器端获取当前账号下拥有角色的信息；
        /// </summary>
        /// <param name="packet">进入的请求数据</param>
        void GetAccountRolesS2C(int sessionId, Dictionary<byte, object> dataMessage)
        {
            Utility.Debug.LogInfo($"{sessionId} : GetAccountRolesS2C");
            var opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncRole;
            var messageDict = new Dictionary<byte, object>();
            //验证角色；
            var userObj = Utility.Json.ToObject<User>(Convert.ToString(Utility.GetValue(dataMessage, (byte)ParameterCode.User)));
            var roleIdList = GetAccountRolesId(userObj);
            if (roleIdList == null)
            {
                opData.ReturnCode = (short)ReturnCode.Empty;
            }
            else
            {
                List<Role> roleObjList = new List<Role>();
                for (int i = 0; i < roleIdList.Length; i++)
                {
                    Role tmpRole = NHibernateQuerier.Query<Role>("RoleID", roleIdList[i]);
                    roleObjList.Add(tmpRole);
                }
                messageDict.Add((byte)ParameterCode.RoleSet, Utility.Json.ToJson(roleObjList));
                opData.ReturnCode = (short)ReturnCode.Success;
            }
            opData.DataMessage = Utility.Json.ToJson(messageDict);
            GameEntry.PeerManager.SendMessage(sessionId, opData);
        }
    }
}
