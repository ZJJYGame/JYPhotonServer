using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
   public class AllianceEvent: ConcurrentEventCore<int, AscensionPeer, AllianceEvent>
    {
        /// <summary>
        /// 监听仙盟解散的事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public override void AddEventListener(int key, Action<AscensionPeer> handler)
        {
            base.AddEventListener(key, handler);
        }
        /// <summary>
        /// 移除仙盟解散事件的监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public override void RemoveEventListener(int key, Action<AscensionPeer> handler)
        {
            base.RemoveEventListener(key, handler);
        }
        /// <summary>
        /// 清除自身仙盟数据函数
        /// </summary>
        /// <param name="id">下标0为自身id</param>
        public void RemoveAllianceStatus(int id)
        {

            //var roleObj = AlliancelogicManager.instance.GetNHCriteria<RoleAlliance>("RoleID", peer.PeerCache.RoleID);
            //roleObj.AllianceID = 0;
            //ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleObj);

            //Utility.Debug.LogError("监听解散的请求成功,这个人的id为"+peer.PeerCache.RoleID);
            //var applyList = Utility.Json.ToObject<List<int>>(roleObj.ApplyForAlliance);

        }
        //public void RemoveAllianceStatus(List<int> id)
        //{

        //    var roleObj = AlliancelogicManager.instance.GetNHCriteria<RoleAlliance>("RoleID", id[0]);
        //    var applyList = Utility.Json.ToObject<List<int>>(roleObj.ApplyForAlliance);
        //    applyList.Remove(id[1]);
        //    roleObj.ApplyForAlliance = Utility.Json.ToJson(applyList);
        //    ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleObj);

        //    Utility.Debug.LogError("监听解散的请求成功,这个人的id为", id[0]);

        //}
    }
}
