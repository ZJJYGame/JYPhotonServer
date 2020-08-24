using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using EventData = Photon.SocketServer.EventData;

namespace AscensionServer.AlliancelogicManager
{
    public  class AlliancelogicManager:ConcurrentSingleton<AlliancelogicManager>
    {
        /// <summary>
        /// 储存登录盟主的字典，用于派发申请消息
        /// </summary>
        Dictionary<int, AscensionPeer> alliancePoolDict = new Dictionary<int, AscensionPeer>();

        public bool TryGetValue(int id, out AscensionPeer peer)
        {
            peer = null;
            if (!alliancePoolDict.ContainsKey(id))
                return false;
            else
            {
                peer = alliancePoolDict[id];
                return true;
            }
        }

        public bool TryAdd(int id , AscensionPeer peer)
        {

            if (alliancePoolDict.ContainsKey(id))
                return false;
            else
            {
                alliancePoolDict.Add(id,peer);
                return true;
            }
        }

        public bool TryRemove(int id)
        {
            if (!alliancePoolDict.ContainsKey(id))
                return false;
            else
            {
                alliancePoolDict.Remove(id);
                return true;

            }



        }
        /// <summary>
        /// 仙盟申请消息派发
        /// </summary>
        /// <param name="peer"></param>
        public void SendApplyForMessage(AscensionPeer peer, Dictionary<byte, object> date)
        {
            //EventData eventData = new EventData();
            //eventData.Parameters = date;
            //peer.SendEvent(eventData,);
        }

    }
}
