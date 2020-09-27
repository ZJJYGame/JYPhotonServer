using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionProtocol.DTO
{
    [Serializable]
    ///发送日常消息的DTO，如人物升级获得珍惜物品等记录消息内容等
    public class DailyMessageDTO : DataTransferObject
    {
        public virtual string Name { get; set; }
        public virtual string Describe { get; set; }
        public virtual  string EventContent { get; set; }
        public override void Clear()
        {
            Name = null;
            Describe = null;
            EventContent = null;

        }
    }
}
