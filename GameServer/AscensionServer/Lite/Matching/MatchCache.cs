﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 匹配缓存层
    /// </summary>
    public class MatchCache
    {

        /// <summary>
        /// 正在等待的  用户id 和  房间id 的映射
        /// </summary>
        private Dictionary<int, int> uidRoomDict = new Dictionary<int, int>();

        /// <summary>
        /// 正在等待的 房间 id  和 房间的数据模型  的映射
        /// </summary>
        private Dictionary<int, MatchRoom> idModelDict = new Dictionary< int, MatchRoom>();

        /// <summary>
        /// 重用是房间队列
        /// </summary>
        private Queue<MatchRoom> roomQueue = new Queue<MatchRoom>();

        /// <summary>
        /// 房间 的id
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);

        /// <summary>
        /// 进入匹配队列  进入匹配房间
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public MatchRoom Enter(int userId ,AscensionPeer client)
        {
            //遍历一下等待的房间，  看一下  有没有正在等待的  如果有  我们就把这玩家加进去
            foreach (var mr in idModelDict.Values)
            {
                //房间满了 继续
                if (mr.IsFull())
                    continue;
                //没满的话
                mr.Enter(userId, client);
                uidRoomDict.Add(userId, mr.Id);
                return mr;
            }
            //如果走到这里代表 没进去 ？ 因为没有等待的房间了
            //自己开个房
            MatchRoom room = null;
            //是否有重用的房间
            if (roomQueue.Count > 0)
                room = roomQueue.Dequeue();
            else room = new MatchRoom(id.AddGetRoomId());

            room.Enter(userId, client);
            idModelDict.Add(userId, room);
            uidRoomDict.Add(userId, room.Id);
            return room;
        }
        /// <summary>
        /// 离开匹配房间
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public MatchRoom Leave(int userId)
        {
            int roomId = uidRoomDict[userId];
            MatchRoom room = idModelDict[roomId];
            room.Leave(userId);
            //还需要进一步的处理
            uidRoomDict.Remove(userId);
            if (room.IsEmpty())
            {
                //如果房间空了 那就放到重用队列里面
                idModelDict.Remove(roomId);
                roomQueue.Enqueue(room);
            }
            return room;
        }


        /// <summary>
        /// 用户是否在匹配房间内
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsMatching(int userId)
        {
            return uidRoomDict.ContainsKey(userId);
        }


        /// <summary>
        /// 获取玩家所在的等待房间
        /// </summary>
        /// <returns></returns>
        public MatchRoom GetRoom(int userId)
        {
            int roomId = uidRoomDict[userId];
            MatchRoom room = idModelDict[roomId];
            return room;
        }

        /// <summary>
        /// 销毁房间
        /// </summary>
        public void Destroy(MatchRoom room)
        {
            idModelDict.Remove(room.Id);
            foreach (var userId in room.UIdClientDict.Keys)
            {
                uidRoomDict.Remove(userId);
            }
            //清空数据
            room.UIdClientDict.Clear();
            room.ReadyUIdSet.Clear();
            roomQueue.Enqueue(room);
        }
    }
}