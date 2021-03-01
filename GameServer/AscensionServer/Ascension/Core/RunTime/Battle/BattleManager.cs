using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    [Module]
    /// <summary>
    /// 战斗模块；
    /// 此模块用于转发客户端发送过来的消息到具体战斗容器中；
    /// 此模块非逻辑层，若处理逻辑，则在具体的战斗容器中处理；
    /// </summary>
    public  class BattleManager :Cosmos. Module,IBattleManager
    {
        IBattleAlgorithmProvider algorithmProvider;
        Dictionary<int, RoomEntity> roomDict = new Dictionary<int, RoomEntity>();
        public override void OnInitialization()
        {
            try
            {
                //algorithmProvider = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IBattleAlgorithmProvider>();
                //if (algorithmProvider == null)
                //    Utility.Debug.LogError($"{this.GetType()} has no helper instance ,base type: {typeof(IBattleAlgorithmProvider)}");
            }
            catch (Exception)
            {
                //throw;
            }
        }
        ///// <summary>
        ///// 转发战斗消息
        ///// </summary>
        ///// <param name="rbiCmd">房间战斗消息命令</param>
        ///// <returns>战斗系统是否能够成功转发消息</returns>
        ////public bool ForwardingBattleCmd(RoomBattleInputC2S rbiCmd)
        ////{
        ////    if (rbiCmd == null)
        ////        return false;
        ////    RoomEntity rc;
        ////    var result = roomDict.TryGetValue(rbiCmd.RoomID, out rc);
        ////    return result;
        ////}
        /// <summary>
        /// 压入战斗数据；
        /// 包含指令集等；
        /// </summary>
        public void EnqueueMessage(int peerId,object data)
        {

        }
    }
}


