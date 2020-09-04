using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionServer
{
    /// <summary>
    /// 战斗计算结果
    /// </summary>
    public class BattleResultVariable : Variable
    {
        public override Type Type { get { return this.GetType(); } }
        public override object GetValue()
        {
            throw new NotImplementedException();
        }
        public override void SetValue(object value)
        {
            throw new NotImplementedException();
        }
    }
}
