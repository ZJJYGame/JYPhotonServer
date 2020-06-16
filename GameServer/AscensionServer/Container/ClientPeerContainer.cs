/*
*Author : Don
*Since 	:2020-05-13
*Description  : Handler基类
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    //TODO  ClientPeerContainer容器类完善
    /// <summary>
    /// 客户普通端容器，广播消息时减少服务器压力。
    /// </summary>
    public class ClientPeerContainer 
    {
        Dictionary<string, AscensionPeer> peerDict = new Dictionary<string, AscensionPeer>();
        public void Add(AscensionPeer data)
        {
            try
            {
                peerDict.Add(data.PeerCache. Account, data);
            }
            catch (Exception)
            {
                throw new Exception("Data is already exist>>" + data.ToString());
            }
        }
        void AddData<T>(T data)
        {
            peerDict.Add(typeof(T).ToString(), data as AscensionPeer);
        }
        public AscensionPeer Get<K>(K dataKey) where K : IComparable
        {
            try
            {
                var dataKeyObj = dataKey as object;
                return peerDict[dataKeyObj as string];
            }
            catch (Exception)
            {
                throw new Exception("Data not  exist>>" + dataKey.ToString());
            }
        }

        public void Remove(AscensionPeer data)
        {
            try
            {
                peerDict.Remove(data.PeerCache. Account);
            }
            catch (Exception)
            {
                throw new Exception("Data not  exist>>" + data.ToString());
            }
        }

        public void Update(AscensionPeer data)
        {
            try
            {
                peerDict[data.PeerCache. Account] = data;
            }
            catch (Exception)
            {
                throw new Exception("Data not  exist>>" + data.ToString());
            }
        }
    }
}
