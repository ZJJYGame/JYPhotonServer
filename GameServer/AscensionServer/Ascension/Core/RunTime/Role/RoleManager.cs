using System.Collections;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol;
namespace AscensionServer
{
    [CustomeModule]
    public class RoleManager:Module<RoleManager>, IKeyValue<int,IRemoteRole >
    {
        public int RoleCount { get { return roleDict.Count; } }
        Dictionary<int, IRemoteRole> roleDict = new Dictionary<int, IRemoteRole>();
        IRoleOperationHelper roleOpHelper;
        public override void OnInitialization()
        {
            roleOpHelper = Utility.Assembly.GetInstanceByAttribute<TargetHelperAttribute, IRoleOperationHelper>();
            if (roleOpHelper == null)
                Utility.Debug.LogError($"{this.GetType()} has no helper instance ,base type: {typeof(IRoleOperationHelper)}");
        }
        public bool ContainsKey(int key)
        {
            return roleDict.ContainsKey(key);
        }
        public bool TryAdd(int key, IRemoteRole value)
        {
            return roleDict.TryAdd(key, value);
        }
        public bool TryGetValue(int key, out IRemoteRole value)
        {
            return roleDict.TryGetValue(key, out value);
        }
        public bool TryRemove(int key)
        {
            return roleDict.Remove(key);
        }
        public bool TryRemove(int key, out IRemoteRole value)
        {
            return roleDict.Remove(key,out value);
        }
        public bool TryUpdate(int key, IRemoteRole newValue, IRemoteRole comparsionValue)
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
    }
}