using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 线程安全的int 类型
    /// </summary>
    public  class ConcurrentInt
    {
        private int roomId;
        public ConcurrentInt(int roomId)
        {
            this.roomId = roomId;
        }

        /// <summary>
        /// 添加并获取
        /// </summary>
        /// <returns></returns>
        public int AddGetRoomId()
        {
            lock (this)
            {
                roomId++;
                return roomId;
            }
        }

        /// <summary>
        /// 减少并获取
        /// </summary>
        /// <returns></returns>
        public int ReduceGetRoomId()
        {
            lock (this)
            {
                roomId--;
                return roomId;
            }
        }
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        public int GetRoomId()
        {
            return roomId;
        }
    }
}
