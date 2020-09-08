using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using FluentNHibernate.Visitors;

namespace AscensionServer
{
    public class SceneManager : Module<SceneManager>, ISimpleKeyValue<ushort, ScenePool>
    {
        Dictionary<ushort, ScenePool> sceneDict;
        public override void OnInitialization()
        {
            sceneDict = new Dictionary<ushort, ScenePool>();
        }
        public bool ContainsKey(ushort key)
        {
            return sceneDict.ContainsKey(key);
        }
        public bool TryAdd(ushort  key, ScenePool Value)
        {
            if (!sceneDict.ContainsKey(key))
            {
                sceneDict.Add(key, Value);
                return true;
            }
            else
                return false;
        }
        public bool TryGetValue(ushort key, out ScenePool value)
        {
            value = default;
            if (sceneDict.ContainsKey(key))
            {
                value= sceneDict[key];
                return true;
            }
            return false;
        }
        public bool TryRemove(ushort key)
        {
            return sceneDict.Remove(key);
        }
        //public void ExitAdventureScene(AscensionPeer peer, Action callBack = null)
        //{
        //    var result = addventureScenePeerCache.Remove(peer.PeerCache.Account);
        //    if (result)
        //    {
        //        callBack?.Invoke();
        //        Utility.Debug.LogInfo("---------------------------- AscensionServer.Cache.Logoff() :remove peer addventureScenePeerCache success : " + peer.ToString() + "------------------------------------");
        //    }
        //    else
        //    {
        //        Utility.Debug.LogInfo("---------------------------- AscensionServer.Cache.Logoff() : can't  remove from laddventureScenePeerCache : " + peer.ToString() + "------------------------------------");
        //    }
        //}
        //    public bool IsEnterAdventureScene(AscensionPeer peer)
        //    {
        //        var result = addventureScenePeerCache.IsExists(peer.PeerCache.Account);
        //        return result;
        //    }
    }
}
