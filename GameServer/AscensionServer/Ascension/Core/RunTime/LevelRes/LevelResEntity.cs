using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    public class LevelResEntity : IDisposable
    {
        static ConcurrentPool<LevelResEntity> levelResEntityPool;
        static LevelResEntity()
        {
            levelResEntityPool = new ConcurrentPool<LevelResEntity>(() => new LevelResEntity(), lre => { lre.Dispose(); });
        }
        public int LevelId { get; private set; }
        public LevelTypeEnum LevelType { get; private set; }
        public Dictionary<int, FixCollectable> CollectableDict { get { return collectableDict; } }
        Dictionary<int, FixCollectable> collectableDict;
        Dictionary<int, FixCollectable> uncollectableDict;
        public LevelResEntity()
        {
            collectableDict = new Dictionary<int, FixCollectable>();
            uncollectableDict = new Dictionary<int, FixCollectable>();
        }
        public bool Collect(int gId, int eleId)
        {
            if (collectableDict.TryGetValue(gId, out var col))
            {
                FixCollectable fixCollectable = null;
                if (!uncollectableDict.ContainsKey(gId))
                {
                    fixCollectable = new FixCollectable();
                    fixCollectable.CollectDict = new Dictionary<int, FixCollectable.CollectableRes>();
                    uncollectableDict.Add(gId, fixCollectable);
                }
                else
                {
                    uncollectableDict.TryGetValue(gId, out var fc);
                    fixCollectable = fc;
                }
                if (col.CollectDict.Remove(eleId, out var removeEle))
                {
                    if (fixCollectable.CollectDict.TryAdd(eleId, removeEle))
                    {
                        removeEle.CanCollected = false;
                        return true;
                    }
                }
            }
            return false;
        }
        public void BroadCast2AllS2C(OperationData opData)
        {
            GameEntry.LevelManager.SendMessageToLevelS2C(LevelType, LevelId, opData);
        }
        public void Dispose()
        {
            LevelId = 0;
            LevelType = LevelTypeEnum.None;
            collectableDict.Clear();
            uncollectableDict.Clear();
        }
        public static LevelResEntity Create(LevelTypeEnum levelType, int levelId)
        {
            var lre = levelResEntityPool.Spawn();
            lre.LevelId = levelId;
            lre.LevelType = levelType;
            return lre;
        }
        public static void Release(LevelResEntity levelResEntity)
        {
            levelResEntityPool.Despawn(levelResEntity);
        }
    }
}
