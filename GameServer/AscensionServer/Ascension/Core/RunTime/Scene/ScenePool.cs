using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    public class ScenePool : IKeyValue<uint, SceneEntity>
    {
        public uint ScenePoolId { get; private set; }
        ConcurrentDictionary<uint, SceneEntity> sceneDict;
        public int SceneEntityCount { get { return sceneDict.Count; } }
        public ScenePool()
        {
            sceneDict = new ConcurrentDictionary<uint, SceneEntity>();
        }
        public void OnInit(uint poolId)
        {
            this.ScenePoolId = poolId;
        }
        public bool ContainsKey(uint key)
        {
            return sceneDict.ContainsKey(key);
        }
        public bool TryAdd(uint key, SceneEntity Value)
        {
            return sceneDict.TryAdd(key, Value);
        }
        public bool TryGetValue(uint key, out SceneEntity value)
        {
            return sceneDict.TryGetValue(key, out value);
        }
        public bool TryRemove(uint key)
        {
            SceneEntity sceneEntity;
            return sceneDict.TryRemove(key, out sceneEntity);
        }
        public bool TryUpdate(uint key, SceneEntity newValue, SceneEntity comparsionValue)
        {
            return sceneDict.TryUpdate(key, newValue, comparsionValue);
        }
    }
}
