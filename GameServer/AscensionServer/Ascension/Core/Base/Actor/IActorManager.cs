using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;

namespace AscensionServer
{
    public interface IActorManager
    {
        bool TryGetValue(byte typeID, int id, out ActorBase actor);
        /// <summary>
        /// 添加Actor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        bool TryAdd(byte typeID, int id, AscensionPeer peer);
        /// <summary>
        /// 添加或更新Actor;
        /// 若存在，则更新；
        /// 若不存在，则添加；
        /// </summary>
        /// <param name="id"></param>
        /// <param name="peer"></param>
        /// <returns></returns>
        bool AddOrUpdate(byte typeID, int id, AscensionPeer peer);
        bool HasActor(byte typeID, int id);
    }
}


