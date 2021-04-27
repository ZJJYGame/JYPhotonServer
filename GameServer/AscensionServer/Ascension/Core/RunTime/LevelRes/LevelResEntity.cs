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
        /// <summary>
        /// index---collectable
        /// </summary>
        public Dictionary<int, FixCollectable> CollectableDict { get { return collectableDict; } }
        public Dictionary<int, FixCombatable> CombatableDict { get { return combatableDict; } }
        /// <summary>
        /// index---collectable
        /// </summary>
        Dictionary<int, FixCollectable> collectableDict;
        /// <summary>
        /// index---collectable
        /// </summary>
        Dictionary<int, FixCollectable> uncollectableDict;


        /// <summary>
        /// index---combatable
        /// </summary>
        Dictionary<int, FixCombatable> combatableDict;
        /// <summary>
        /// index---combatable
        /// </summary>
        Dictionary<int, FixCombatable> uncombatableDict;

        public LevelResEntity()
        {
            collectableDict = new Dictionary<int, FixCollectable>();
            uncollectableDict = new Dictionary<int, FixCollectable>();
            combatableDict = new Dictionary<int, FixCombatable>();
            uncombatableDict = new Dictionary<int, FixCombatable>();
        }
        public bool Combat(int index, int gId, int eleId)
        {
            return false;
        }
        public bool Gather(int index, int gId, int eleId)
        {
            if (collectableDict.TryGetValue(index, out var col))
            {
                if (col.Id != gId)
                {
                    return false;
                }
                FixCollectable fixCollectable = null; ;
                if (!uncollectableDict.ContainsKey(index))
                {
                    fixCollectable = new FixCollectable();
                    fixCollectable.Id = gId;
                    fixCollectable.CollectableDict = new Dictionary<int, FixResObject>();
                    uncollectableDict.Add(index, fixCollectable);
                }
                else
                {
                    uncollectableDict.TryGetValue(index, out var fc);
                    if (fc.Id != gId)
                        return false;
                    fixCollectable = fc;
                }
                if (col.CollectableDict.Remove(eleId, out var removeEle))
                {
                    if (fixCollectable.CollectableDict.TryAdd(eleId, removeEle))
                    {
                        removeEle.Occupied= true;
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
