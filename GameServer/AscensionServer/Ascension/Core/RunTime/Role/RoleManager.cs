using System.Collections;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol;
namespace AscensionServer
{
    [CustomeModule]
    public class RoleManager:Module<RoleManager>, IKeyValue<int,IRoleEntity >
    {
        public int RoleCount { get { return roleDict.Count; } }
        Dictionary<int, IRoleEntity> roleDict = new Dictionary<int, IRoleEntity>();
        Queue<IRoleEntity> playerPoolQueue = new Queue<IRoleEntity>();

        public bool ContainsKey(int key)
        {
            return roleDict.ContainsKey(key);
        }
        public bool TryAdd(int key, IRoleEntity value)
        {
            return roleDict.TryAdd(key, value);
        }
        public bool TryGetValue(int key, out IRoleEntity value)
        {
            return roleDict.TryGetValue(key, out value);
        }
        public bool TryRemove(int key)
        {
            return roleDict.Remove(key);
        }
        public bool TryRemove(int key, out IRoleEntity value)
        {
            return roleDict.Remove(key,out value);
        }
        public bool TryUpdate(int key, IRoleEntity newValue, IRoleEntity comparsionValue)
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