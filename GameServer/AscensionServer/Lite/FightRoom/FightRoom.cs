using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
namespace AscensionServer.Lite

{
    /// <summary>
    /// 战斗房间
    /// </summary>
    public class FightRoom
    {
        /// <summary>
        /// 房间唯一标识码
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// 存储所有玩家
        /// </summary>
        public HashSet<RoleDTO> PlayerSet { get; set; }

        /// <summary>
        /// 中途退出的玩家id列表
        /// </summary>
        public HashSet<int> LeaveUIdSet { get; set; }
        /// <summary>
        /// 回合管理类
        /// </summary>

        /// <summary>
        /// 做初始化
        /// </summary>
        public FightRoom (int _id,HashSet<int> _uidSet)
        {
            this.Id = _id;
            this.PlayerSet = new HashSet<RoleDTO>();
            foreach (var _uId in _uidSet)
            {
                this.PlayerSet.Add(new RoleDTO() { RoleID  = _uId });
            }
            LeaveUIdSet = new HashSet<int>();
        }

        public void Init(HashSet<int> _uidSet)
        {
            foreach (var _uId in _uidSet)
            {
                this.PlayerSet.Add(new RoleDTO() { RoleID = _uId });
            }
        }


        public bool IsOffline(int _uid)
        {
            return LeaveUIdSet.Contains(_uid);
        }

        public RoleDTO GetPlayerModel(int _useId) 
        {

            foreach (var player in PlayerSet)
            {
                if (player.RoleID == _useId)
                {
                    return player;
                }
            }
            throw new Exception("玩家不存在，数据不存在");
        }
        /// <summary>
        /// 获取房间内第一个玩家的id
        /// </summary>
        /// <returns></returns>
        public int GetFirstUId()
        {
            return PlayerSet.ToList()[0].RoleID;
        }

    }
}


