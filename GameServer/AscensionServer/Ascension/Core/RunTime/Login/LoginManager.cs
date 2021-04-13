using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;

namespace AscensionServer
{
    [Module]
    public class LoginManager : Cosmos.Module, ILoginManager
    {
        Type createRoleHelperType;
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.LoginArea, ProcessHandlerC2S);
            createRoleHelperType = Utility.Assembly.GetDerivedTypesByAttribute<ImplementProviderAttribute, ICreateRoleHelper>(GetType().Assembly)[0];
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
            int length = 0;
            if (!string.IsNullOrEmpty(roleIdListJson))
            {
                var roleIdListStr = Utility.Json.ToObject<List<string>>(roleIdListJson);
                length = roleIdListStr.Count;
                var roleIdListInt = new List<int>();
                for (int i = 0; i < length; i++)
                {
                    roleIdListInt.Add(int.Parse(roleIdListStr[i]));
                }
                return  roleIdListInt.ToArray();
            }
            return null;
        }
        void ProcessHandlerC2S(int sessionId, OperationData packet)
        {
            var areaSubCode = (LoginAreaOpCode)packet.SubOperationCode;
            var dict = Utility.Json.ToObject<Dictionary<byte, object>>(Convert.ToString(packet.DataMessage));
            switch (areaSubCode)
            {
                case LoginAreaOpCode.GetAccountRoles:
                    {
                        GetAccountRolesS2C(sessionId, dict);
                    }
                    break;
                case LoginAreaOpCode.CreateRole:
                    {
                        CreateRoleS2C(sessionId, dict);
                    }
                    break;
                case LoginAreaOpCode.LoginRole:
                    {
                        LoginRoleS2C(sessionId, dict);
                    }
                    break;
                case LoginAreaOpCode.LogoffRole:
                    {
                        LogoffRoleS2C(sessionId, dict);
                    }
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
            var opData = new OperationData()
            {
                OperationCode = (byte)OperationCode.LoginArea,
                SubOperationCode = (short)LoginAreaOpCode.GetAccountRoles
            };
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
        void CreateRoleS2C(int sessionId, Dictionary<byte, object> dataMessage)
        {
            var createRoleHelper = CosmosEntry.ReferencePoolManager.Spawn(createRoleHelperType) as ICreateRoleHelper;
            var opData = createRoleHelper.CreateRole(dataMessage);
            opData.OperationCode = (byte)OperationCode.LoginArea;
            opData.SubOperationCode = (short)LoginAreaOpCode.CreateRole;
            GameEntry.PeerManager.SendMessage(sessionId, opData);
            CosmosEntry.ReferencePoolManager.Despawn(createRoleHelper);
        }
        void LoginRoleS2C(int sessionId, Dictionary<byte, object> dataMessage)
        {
            var opData = new OperationData()
            {
                OperationCode = (byte)OperationCode.LoginArea,
                SubOperationCode = (short)LoginAreaOpCode.LoginRole
            };
            var json = Convert.ToString(Utility.GetValue(dataMessage, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(json);
            var role = RoleEntity.Create(roleObj.RoleID, sessionId, roleObj);
            RoleEntity remoteRole;
            var roleExist = GameEntry.RoleManager.TryGetValue(roleObj.RoleID, out remoteRole);
            if (roleExist)
            {
                IPeerEntity pa;
                GameEntry.PeerManager.TryGetValue(remoteRole.SessionId, out pa);
                pa.SendEventMsg((byte)EventCode.ReplacePlayer, null);//从这里发送挤下线消息；
                GameEntry.RoleManager.TryRemove(roleObj.RoleID);
            }
            IPeerEntity peerAgent;
            var result = GameEntry.PeerManager.TryGetValue(sessionId, out peerAgent);
            if (result)
            {
                Utility.Debug.LogInfo("yzqData" + "验证登录的Sessionid:" + sessionId);
                var remoteRoleType = typeof(RoleEntity);
                var exist = peerAgent.ContainsKey(remoteRoleType);
                if (!exist)
                {
                    GameEntry.RoleManager.TryAdd(roleObj.RoleID, role);
                    Utility.Debug.LogInfo("yzqData" + "进入角色判断只有一个账号选择角色");
                    #region 
                    var roleAllianceobj = NHibernateQuerier.Query<RoleAlliance>("RoleID", roleObj.RoleID);
                    if (roleAllianceobj != null)
                    {
                        Utility.Debug.LogInfo("yzqData判断仙盟查找到了" + roleAllianceobj.AllianceID);
                        var allianceStatusobj = NHibernateQuerier.Query<AllianceStatus>("ID", roleAllianceobj.AllianceID);

                        if (allianceStatusobj != null)
                        {
                            allianceStatusobj.OnLineNum += 1;
                            NHibernateQuerier.Update(allianceStatusobj);
                            Utility.Debug.LogInfo("yzqData仙盟在线人数增加了" + allianceStatusobj.OnLineNum);
                        }
                        roleAllianceobj.JoinOffline = "在线";
                        NHibernateQuerier.Update(roleAllianceobj);
                    }
                    #endregion
                    peerAgent.TryAdd(remoteRoleType, role);
                    opData.ReturnCode = (byte)ReturnCode.Success;
                }
                else
                {
                    Utility.Debug.LogInfo("yzqData" + "已有账号登陆角色");
                    object legacyRole;
                    peerAgent.TryGetValue(remoteRoleType, out legacyRole);
                    var remoteRoleObj = legacyRole as RoleEntity;
                    var updateResult = peerAgent.TryUpdate(remoteRoleType, role, legacyRole);
                    if (updateResult)
                    {
                        Utility.Debug.LogInfo("yzqData" + "已有账号登陆角色，替换相同角色成功");
                        #region 
                        var roleAllianceobj = NHibernateQuerier.Query<RoleAlliance>("RoleID", roleObj.RoleID);
                        if (roleAllianceobj != null)
                        {
                            var allianceStatusobj = NHibernateQuerier.Query<AllianceStatus>("ID", roleAllianceobj.AllianceID);
                            if (allianceStatusobj != null)
                            {
                                NHibernateQuerier.Update(allianceStatusobj);
                                Utility.Debug.LogInfo("yzqData仙盟在线人数增加了" + allianceStatusobj.OnLineNum);
                            }
                            roleAllianceobj.JoinOffline = "在线";
                            NHibernateQuerier.Update(roleAllianceobj);
                        }
                        #endregion
                        GameEntry.RoleManager.TryRemove(roleObj.RoleID);
                        GameEntry.RoleManager.TryAdd(roleObj.RoleID, role);
                        opData.ReturnCode = (byte)ReturnCode.Success;//登录成功
                        GameEntry.RecordManager.RecordRole(remoteRoleObj as RoleEntity);
                        CosmosEntry.ReferencePoolManager.Despawn(remoteRoleObj);//回收这个RemoteRole对象
                    }
                    else
                    {
                        Utility.Debug.LogInfo("yzqData" + "替换角色失败");
                        opData.ReturnCode = (byte)ReturnCode.Fail;//登录失败
                    }
                }
            }
            else
                opData.ReturnCode = (byte)ReturnCode.Fail;//登录失败
            GameEntry.PeerManager.SendMessage(sessionId, opData);
        }
        void LogoffRoleS2C(int sessionId, Dictionary<byte, object> dataMessage)
        {
            var opData = new OperationData()
            {
                OperationCode = (byte)OperationCode.LoginArea,
                SubOperationCode = (short)LoginAreaOpCode.LogoffRole
            };
            IPeerEntity peer;
            GameEntry.PeerManager.TryGetValue(sessionId, out peer);
            var json = Convert.ToString(Utility.GetValue(dataMessage, (byte)ParameterCode.Role));
            var roleObj = Utility.Json.ToObject<RoleDTO>(json);
            IPeerEntity peerAgent;
            var result = GameEntry.PeerManager.TryGetValue(peer.SessionId, out peerAgent);
            if (result)
            {
                var remoteRoleType = typeof(RoleEntity);
                object remoteRoleObj;
                var removeResult = peerAgent.TryRemove(remoteRoleType, out remoteRoleObj);
                if (removeResult)
                {
                    var remoteRole = remoteRoleObj as RoleEntity;
                    CosmosEntry.ReferencePoolManager.Despawn(remoteRole);//回收这个RemoteRole对象
                    GameEntry.RoleManager.TryRemove(roleObj.RoleID);
                }
                opData.ReturnCode = (byte)ReturnCode.Success;
                GameEntry.RecordManager.RecordRole(remoteRoleObj as RoleEntity);
            }
            else
            {
                opData.ReturnCode = (byte)ReturnCode.ItemNotFound;
            }
            GameEntry.PeerManager.SendMessage(sessionId, opData);
        }
    }
}
