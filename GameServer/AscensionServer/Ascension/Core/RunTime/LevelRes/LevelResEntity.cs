using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    //========================================
    //采集：
    //         玩家采集流程，在LevelResEntity中，是直接判断当
    //前资源是否在可采集的列表中。若可采集，则将可采集容器中
    //的对象移至不可采集容器中，并返回true。否则就返回false。
    //
   //战斗：
   //          1、玩家与怪物进行战斗时，会对怪进行占用。
   //          2、当怪被占用时其他玩家则无法与这个被占用的怪进行战斗。
   //          3、被占用的怪会进入pending容器中，直到战斗成功、
   //          失败的结果返回，再通知玩家显示怪的状态。
   //          4、战斗失败则玩家触发死亡，怪进行保留。
   //          5、战斗成功则玩家/获得奖励，怪被移至不可战斗容器中。
    //========================================
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
        /// <summary>
        /// index---combatable
        /// </summary>
        Dictionary<int, FixCombatable> pendingCombatableDict;



        public LevelResEntity()
        {
            collectableDict = new Dictionary<int, FixCollectable>();
            uncollectableDict = new Dictionary<int, FixCollectable>();
            combatableDict = new Dictionary<int, FixCombatable>();
            uncombatableDict = new Dictionary<int, FixCombatable>();
            pendingCombatableDict = new Dictionary<int, FixCombatable>();
        }
        /// <summary>
        /// 表示进入战斗是否成功；
        /// 如果进入战斗成功，则这只怪会进入pending状态，其他玩家就无法再与之战斗；
        /// </summary>
        /// <param name="index">资源生成时的id</param>
        /// <param name="gId">全局id</param>
        /// <param name="eleId">元素id</param>
        /// <returns>是否进入战斗成功</returns>
        public bool Combat(int index, int gId, int eleId)
        {
            if (combatableDict.TryGetValue(index, out var col))
            {
                if (col.Id != gId)
                {
                    return false;
                }
                FixCombatable fixCombatable= null; ;
                if (!uncollectableDict.ContainsKey(index))
                {
                    fixCombatable = new  FixCombatable();
                    fixCombatable.Id = gId;
                    fixCombatable.CombatableDict= new Dictionary<int, FixResObject>();
                    pendingCombatableDict.Add(index, fixCombatable);
                }
                else
                {
                    pendingCombatableDict.TryGetValue(index, out var fc);
                    if (fc.Id != gId)
                        return false;
                    fixCombatable = fc;
                }
                if (col.CombatableDict.Remove(eleId, out var removeEle))
                {
                    if (fixCombatable.CombatableDict.TryAdd(eleId, removeEle))
                    {
                        removeEle.Occupied = true;
                        return true;
                    }
                }
            }
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
            combatableDict.Clear();
            uncombatableDict.Clear();
            pendingCombatableDict.Clear();
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
