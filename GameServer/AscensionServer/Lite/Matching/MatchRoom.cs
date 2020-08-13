using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Lite

{
    /// <summary>
    /// 匹配房间
    /// </summary>
    public class MatchRoom
    {
     /// <summary>
    /// 唯一标识
    /// </summary>
        public int Id { get; }

        /// <summary>
        /// 在房间内用户id的列表  和  链接对象的 映射关系
        /// </summary>
        public Dictionary<int, AscensionPeer> UIdClientDict { get; private set; }
  

        /// <summary>
        /// 匹配中玩家的id列表
        /// </summary>
        public HashSet<int> ReadyUIdSet { get; private set; }

        public MatchRoom (int id)
        {
            this.Id = id;
            this.UIdClientDict = new Dictionary<int, AscensionPeer>();
            this.ReadyUIdSet = new HashSet<int>();
        }

        /// <summary>
        /// 房间是否满了
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return UIdClientDict.Count == 2;
        }

        /// <summary>
        /// 房间是否空了
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return UIdClientDict.Count == 0;
        }
        /// <summary>
        /// 房间中的玩家是否都在匹配中
        /// </summary>
        /// <returns></returns>
        public bool IsAllReady()
        {
            return ReadyUIdSet.Count == 2;
        }

        public  List<int > GetUIdList()
        {
            return UIdClientDict.Keys.ToList();
        }
        /// <summary>
        /// 进入房间
        /// </summary>
        /// <param name="usrId"></param>
        /// <param name="client"></param>
        public void Enter(int usrId,AscensionPeer client)
        {
            UIdClientDict.Add(usrId, client);
        }

        /// <summary>
        /// 离开房间
        /// </summary>
        /// <param name="userId"></param>
        public void Leave(int userId)
        {
            UIdClientDict.Remove(userId);
        }

        /// <summary>
        /// 玩家开始匹配
        /// </summary>
        /// <param name="userId"></param>
        public void Ready(int userId)
        {
            ReadyUIdSet.Add(userId);
        }
        /// <summary>
        /// 广播房间内的所有玩家信息
        /// </summary>
        public void Brocast()
        {

        }
    
    }
}
