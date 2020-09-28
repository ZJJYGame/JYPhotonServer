using System.Collections;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol;
namespace AscensionServer
{
    [CustomeModule]
    public class RoleManager:Module<RoleManager>, IKeyValue<int,RoleEntity >
    {
        public int RoleCount { get { return roleDict.Count; } }
        Dictionary<int, RoleEntity> roleDict = new Dictionary<int, RoleEntity>();
        IRoleOperationHelper opHelper;
        public override void OnInitialization()
        {
            opHelper = Utility.Assembly.GetInstanceByAttribute<TargetHelperAttribute, IRoleOperationHelper>();
            if (opHelper == null)
                Utility.Debug.LogError($"{this.GetType()} has no helper instance ,base type: {typeof(IRoleOperationHelper)}");
        }
        public override void OnPreparatory()
        {
            OpCodeEventCore.Instance.AddEventListener((byte)OperationCode.LoginRole, RoleLoginHandler);
            OpCodeEventCore.Instance.AddEventListener((byte)OperationCode.LogoffRole, RoleLogoffHandler);
        }
        public override void OnTermination()
        {
            OpCodeEventCore.Instance.RemoveEventListener((byte)OperationCode.LoginRole, RoleLoginHandler);
            OpCodeEventCore.Instance.RemoveEventListener((byte)OperationCode.LogoffRole, RoleLogoffHandler);
        }
        public bool ContainsKey(int key)
        {
            return roleDict.ContainsKey(key);
        }
        public bool TryAdd(int key, RoleEntity value)
        {
            return roleDict.TryAdd(key, value);
        }
        public bool TryGetValue(int key, out RoleEntity value)
        {
            return roleDict.TryGetValue(key, out value);
        }
        public bool TryRemove(int key)
        {
            return roleDict.Remove(key);
        }
        public bool TryRemove(int key, out RoleEntity value)
        {
            return roleDict.Remove(key,out value);
        }
        public bool TryUpdate(int key, RoleEntity newValue, RoleEntity comparsionValue)
        {
            if (!newValue.Equals(comparsionValue))
                return false;
            if(roleDict.ContainsKey(key))
            {
                roleDict[key] = newValue;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 角色登录事件；
        /// </summary>
        void RoleLoginHandler(object sender,object data)
        {
            opHelper?.LoginHandler(sender,data);
        }
        /// <summary>
        /// 角色登出事件；
        /// </summary>
        void RoleLogoffHandler(object sender, object data)
        {
            opHelper?.LogoffHandler(sender, data);
        }
    }
}