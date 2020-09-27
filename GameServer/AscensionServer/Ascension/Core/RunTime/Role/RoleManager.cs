using System.Collections;
using System.Collections.Generic;
using Cosmos;
namespace AscensionServer
{
    [CustomeModule]
    public class RoleManager:Module<RoleManager>, IKeyValue<uint,RoleEntity >
    {
        public int RoleCount { get { return roleDict.Count; } }
        Dictionary<uint, RoleEntity> roleDict = new Dictionary<uint, RoleEntity>();
        public bool ContainsKey(uint key)
        {
            return roleDict.ContainsKey(key);
        }
        public bool TryAdd(uint key, RoleEntity value)
        {
            return roleDict.TryAdd(key, value);
        }
        public bool TryGetValue(uint key, out RoleEntity value)
        {
            return roleDict.TryGetValue(key, out value);
        }
        public bool TryRemove(uint key)
        {
            return roleDict.Remove(key);
        }
        public bool TryRemove(uint key, out RoleEntity value)
        {
            return roleDict.Remove(key,out value);
        }
        public bool TryUpdate(uint key, RoleEntity newValue, RoleEntity comparsionValue)
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