using System.Collections;
using System.Collections.Generic;
using Cosmos;
using System;
namespace AscensionServer
{
    public class RemoteRole : IRemoteRole
    {
        public int RoleId { get; private set; }
        public int SessionId{ get; private set; }
        public int DataCount { get { return dataDict.Count; } }
        Dictionary<Type, object> dataDict = new Dictionary<Type, object>();
        public void OnInit(int roleId,int sessionId)
        {
            this.RoleId = roleId;
            this.SessionId= sessionId;
        }
        public object[] Find(Predicate<object> handler)
        {
            List<object> dataSet = new List<object>();
            dataSet.AddRange(dataDict.Values);
            return dataSet.FindAll(handler).ToArray();
        }
        public bool ContainsKey(Type key)
        {
            return dataDict.ContainsKey(key);
        }
        public bool TryAdd(Type key, object value)
        {
            return dataDict.TryAdd(key, value);
        }
        public bool TryGetValue(Type key, out object value)
        {
            return dataDict.TryGetValue(key, out value);
        }
        public bool TryRemove(Type key)
        {
            return dataDict.Remove(key);
        }
        public bool TryRemove(Type key, out object value)
        {
            return dataDict.Remove(key, out value);
        }
        public bool TryUpdate(Type key, object newValue, object comparsionValue)
        {
            if (newValue.GetType() != comparsionValue.GetType())
                return false;
            if (newValue.Equals(comparsionValue))
            {
                if (!dataDict.ContainsKey(key))
                    return false;
                dataDict[key] = newValue;
            }
            return false;
        }
        public void Clear()
        {
            RoleId = 0;
            dataDict.Clear();
        }
        public override bool Equals(object obj)
        {
            var entity = obj as RemoteRole;
            if (entity==null)
                return false;
            return entity.RoleId == this.RoleId;
        }
        public static RemoteRole Create(int roleId,int sessionId, params object[] datas)
        {
            var entity = GameManager.ReferencePoolManager.Spawn<RemoteRole>();
            entity.OnInit(roleId,sessionId);
            for (int i = 0; i < datas.Length; i++)
            {
                entity.TryAdd(datas[i].GetType(), datas[i]);
            }
            return entity;
        }
    }
}