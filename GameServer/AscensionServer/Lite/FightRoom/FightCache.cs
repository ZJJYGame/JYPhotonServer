using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer.Lite

{
    /// <summary>
    /// 战斗的缓存层
    /// </summary>
    public class FightCache
    {
        /// <summary>
        /// 用户id 对应的 房间id
        /// </summary>
        private Dictionary<int, int> uidRoomIDict = new Dictionary<int, int>();
        /// <summary>
        /// 房间id  对应的  房间模型对象
        /// </summary>
        private Dictionary<int, FightRoom> idRoomDict = new Dictionary<int, FightRoom>();
        /// <summary>
        /// 重用房间队列
        /// </summary>
        private Queue<FightRoom> roomQueue = new Queue<FightRoom>();

        /// <summary>
        /// 房间的id
        /// </summary>
        private ConcurrentInt roomId = new ConcurrentInt(-1);

        /// <summary>
        /// 创建战斗房间
        /// </summary>
        /// <param name="uidSet"></param>
        /// <returns></returns>
        public FightRoom Create(HashSet<int> uidSet)
        {
            FightRoom room = null;
            //检测可重用的房间
            if (roomQueue.Count > 0)
            {
                room = roomQueue.Dequeue();
                room.Init(uidSet);
            }
            else // 没有就直接创建
                room = new FightRoom(roomId.AddGetRoomId(), uidSet);
            //绑定映射关系
            foreach (var uid in uidSet)
            {
                uidRoomIDict.Add(uid, room.Id);
            }
            idRoomDict.Add(room.Id, room);
            return room;
        }
        
        /// <summary>
        /// 获取房间
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FightRoom GetRoom(int id)
        {
            if (!idRoomDict.ContainsKey(id))
                throw new Exception("不存在这个房间");
            FightRoom room = idRoomDict[id];
            return room;
        }

        /// <summary>
        /// 是否存在当前房间的id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsFighting(int userId)
        {
            return uidRoomIDict.ContainsKey(userId);
        }
        /// <summary>
        /// 根据用户id获取所在的房间
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public FightRoom GetRoomByUId(int uid)
        {
            if (!uidRoomIDict.ContainsKey(uid))
                throw new Exception("当前用户不在房间");
            int roomId =  uidRoomIDict[uid];
            FightRoom room = GetRoom(roomId);
            return room;
        }

        /// <summary>
        /// 摧毁房间
        /// </summary>
        /// <param name="room"></param>
        public void Destroy(FightRoom room)
        {
            //移除映射关系
            idRoomDict.Remove(room.Id);
            foreach (var player in room.PlayerSet)
            {
                uidRoomIDict.Remove(player.RoleID);
            }
            //初始化房间数据
            room.PlayerSet.Clear();
            room.LeaveUIdSet.Clear();
            //TODO 需要完善
            //添加到重用队列里面等待重用
            roomQueue.Enqueue(room);
        }
    }
}
