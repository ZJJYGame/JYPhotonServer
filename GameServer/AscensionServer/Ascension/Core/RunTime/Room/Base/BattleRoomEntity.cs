using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
    /// <summary>
    /// 战斗房间实体对象；
    /// 此对象由系统进行分配，多客户端战斗时候缓存战斗操作数据，
    /// 由服务器计算后返回给客户端进行展示；
    /// </summary>
    public class BattleRoomEntity:RoomEntity
    {
        ConcurrentBag<RoomBattleInputData> inputCmdQueue = new ConcurrentBag<RoomBattleInputData>();
        public override void Clear()
        {
            base.Clear();
            RoomBattleInputData biC2S;
            do
            {
                inputCmdQueue.TryTake(out biC2S);
            } while (inputCmdQueue.Count > 0);
        }
    }
}
