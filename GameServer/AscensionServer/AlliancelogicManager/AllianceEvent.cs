using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
   public class AllianceEvent: ConcurrentEventCore<string,List<int> , AllianceEvent>
    {
        /// <summary>
        /// 监听仙盟解散的事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public override void AddEventListener(string key, Action<List<int>> handler)
        {
            base.AddEventListener(key, handler);
        }
        /// <summary>
        /// 移除仙盟解散事件的监听
        /// </summary>
        /// <param name="key"></param>
        /// <param name="handler"></param>
        public override void RemoveEventListener(string key, Action<List<int>> handler)
        {
            base.RemoveEventListener(key, handler);
        }
        /// <summary>
        /// 清除自身仙盟数据函数
        /// </summary>
        /// <param name="id">下标0为自身id，下标1为仙盟id</param>
        public void RemoveAllianceStatus(List<int> id)
        {
            Utility.Debug.LogError("监听解散的请求成功");
        }

    }
}
