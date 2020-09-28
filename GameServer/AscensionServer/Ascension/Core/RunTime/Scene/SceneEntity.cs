using Cosmos;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 场景实体对象
    /// </summary>
    public class SceneEntity : IKeyValue<int, IPeerAgent>, IReference
    {
        public uint SceneId { get; private set; }
        public bool Available { get; private set; }
        ConcurrentDictionary<int, IPeerAgent> peerDict;
        public SceneEntity()
        {
            peerDict = new ConcurrentDictionary<int , IPeerAgent>();
        }
        public void OnInit(uint sceneId)
        {
            this.SceneId = sceneId;
            this.Available = true;
        }
        public bool ContainsKey(int  key)
        {
            return peerDict.ContainsKey(key);
        }
        public bool TryAdd(int key, IPeerAgent Value)
        {
            return peerDict.TryAdd(key, Value);
        }
        public bool TryGetValue(int key, out IPeerAgent value)
        {
            return peerDict.TryGetValue(key, out value);
        }
        public bool TryRemove(int key)
        {
            IPeerAgent peerEntity;
            return peerDict.TryRemove(key, out peerEntity);
        }
        public bool TryRemove(int key, out IPeerAgent value)
        {
            return peerDict.TryRemove(key, out value);
        }
        public bool TryUpdate(int key, IPeerAgent newValue, IPeerAgent comparsionValue)
        {
            return peerDict.TryUpdate(key, newValue, comparsionValue);
        }
        public void Clear()
        {
            peerDict.Clear();
            this.SceneId = 0;
            this.Available = false;
        }
        public static SceneEntity Create(uint sceneId, params IPeerAgent[] peerEntities)
        {
            SceneEntity se = new SceneEntity();
            se.OnInit(sceneId);
            int length = peerEntities.Length;
            for (int i = 0; i < length; i++)
            {
                se.TryAdd(peerEntities[i].SessionId, peerEntities[i]);
            }
            return se;
        }
        public static SceneEntity Create(uint sceneId, List<IPeerAgent> peerEntities)
        {
            SceneEntity se = new SceneEntity();
            se.OnInit(sceneId);
            int length = peerEntities.Count;
            for (int i = 0; i < length; i++)
            {
                se.TryAdd(peerEntities[i].SessionId, peerEntities[i]);
            }
            return se;
        }


    }
}
