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
    /// 客户端容器，广播消息时减少服务器压力。
    /// </summary>
    public class ClientPeerContainer : IOperable<AscensionPeer>
    {
        SortedList<string, AscensionPeer> peerDict = new SortedList<string, AscensionPeer>();

        public void Add(AscensionPeer data)
        {
            try
            {
                peerDict.Add(data.Account, data);
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
            throw new NotImplementedException();
        }

        public void Remove(AscensionPeer data)
        {
            throw new NotImplementedException();
        }

        public void Update(AscensionPeer data)
        {
            throw new NotImplementedException();
        }
    }
}
