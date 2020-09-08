using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    /// <summary>
    /// 数据注入用ParameterCode对应的数据类型作Key Value;
    /// </summary>
    public class RoleEntity : IRoleEntity, IReference
    {
        /// <summary>
        /// 初始化后的role ID
        /// </summary>
        public uint RoleId { get; private set; }
        public byte DataCount { get { return Convert.ToByte(dataDict.Count); } }
        ConcurrentDictionary<byte, object> dataDict;
        public RoleEntity()
        {
            dataDict = new ConcurrentDictionary<byte, object>();
        }
        public void OnInit(uint roleId)
        {
            this.RoleId = roleId;
        }
        public bool ContainsKey(byte key)
        {
            return dataDict.ContainsKey(key);
        }
        public bool TryAdd(byte key, object Value)
        {
            return dataDict.TryAdd(key, Value);
        }
        public bool TryGetValue(byte key, out object value)
        {
            return dataDict.TryGetValue(key, out value);
        }
        public bool TryRemove(byte key)
        {
            object data;
            return dataDict.TryRemove(key, out data);
        }
        public void Clear()
        {
            dataDict.Clear();
            RoleId = 0;
        }
        public static RoleEntity Create(uint roleId)
        {
            var re = GameManager.ReferencePoolManager.Spawn<RoleEntity>();
            re.OnInit(roleId);
            return re;
        }
    }
}
