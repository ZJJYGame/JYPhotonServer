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
    public class SceneEntity : IKeyValue<long, PeerEntity>, IReference
    {
        public uint SceneId { get; private set; }
        public bool Available { get; private set; }
        ConcurrentDictionary<long, PeerEntity> peerDict;
        public SceneEntity()
        {
            peerDict = new ConcurrentDictionary<long, PeerEntity>();
        }
        public void OnInit(uint sceneId)
        {
            this.SceneId = sceneId;
            this.Available = true;
        }
        public bool ContainsKey(long key)
        {
            return peerDict.ContainsKey(key);
        }
        public bool TryAdd(long key, PeerEntity Value)
        {
            return peerDict.TryAdd(key, Value);
        }
        public bool TryGetValue(long key, out PeerEntity value)
        {
            return peerDict.TryGetValue(key, out value);
        }
        public bool TryRemove(long key)
        {
            PeerEntity peerEntity;
            return peerDict.TryRemove(key, out peerEntity);
        }
        public bool TryUpdate(long key, PeerEntity newValue, PeerEntity comparsionValue)
        {
            return peerDict.TryUpdate(key, newValue, comparsionValue);
        }
        public void Clear()
        {
            peerDict.Clear();
            this.SceneId = 0;
            this.Available = false;
        }
        public static SceneEntity Create(uint sceneId, params PeerEntity[] peerEntities)
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
        public static SceneEntity Create(uint sceneId, List<PeerEntity> peerEntities)
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
