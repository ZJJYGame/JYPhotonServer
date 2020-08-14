using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// Actor类型为Peer类型
    /// </summary>
    public sealed class ActorManager:ModuleBase<ActorManager>
    {
        Dictionary<int, ActorBase> actorDict = new Dictionary<int, ActorBase>();
        public ActorBase GetActor(int id)
        {
            if (actorDict.ContainsKey(id))
                return actorDict[id];
            else
                return null;
        }
    }
}
