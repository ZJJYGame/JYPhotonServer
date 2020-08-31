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
    /// Actor类型为Peer类型
    /// </summary>
    public sealed class ActorManager : Module<ActorManager>
    {
        /// <summary>
        /// Peer类型的Actor
        /// </summary>
        Dictionary<byte, ActorPool> actorPoolDict = new Dictionary<byte, ActorPool>();
        public bool TryGetValue(byte typeID, int id, out ActorBase actor)
        {
            actor = null;
            if (!actorPoolDict.ContainsKey(typeID))
                return false;
            else
            {
                return actorPoolDict[typeID].TryGetValue(id, out actor);
            }
        }
        /// <summary>
        /// 添加Actor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        public bool TryAdd(byte typeID, int id, AscensionPeer peer)
        {
            if (actorPoolDict.ContainsKey(typeID))
                return false;
            else
            {
                var act = Actor<AscensionPeer>.Create((byte)ActorTypeEnum.Player, id, peer);
                return actorPoolDict[typeID].TryAdd(id, act);
            }
        }
        /// <summary>
        /// 添加或更新Actor;
        /// 若存在，则更新；
        /// 若不存在，则添加；
        /// </summary>
        /// <param name="id"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        public bool AddOrUpdate(byte typeID, int id, AscensionPeer peer)
        {
            if (peer == null)
                return false;
            if (!actorPoolDict.ContainsKey(typeID))
            {
                var pool = new ActorPool();
                var newAct = Actor<AscensionPeer>.Create((byte)ActorTypeEnum.Player, id, peer);
                pool.AddOrUpdate(id, newAct);
            }
            else
            {
                ActorBase actB;
               var result= actorPoolDict[typeID].TryGetValue(id,out actB );
                if (result)
                {
                    Actor<AscensionPeer> oldAct=actB as Actor<AscensionPeer>;
                    GameManager.ReferencePoolManager.Despawn(oldAct);
                }
                var act = Actor<AscensionPeer>.Create((byte)ActorTypeEnum.Player, id, peer);
                actorPoolDict[typeID].AddOrUpdate(id, act);
            }
            return true;
        }
        public bool HasActor(byte typeID, int id)
        {
            if (actorPoolDict.ContainsKey(typeID))
                return false;
            else
            {
                return actorPoolDict[typeID].Contains(id);
            }
        }
    }
}
