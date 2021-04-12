using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public class FixAnimator
    {
        /// <summary>
        /// 《StringToHash,Value》;
        /// key 为 Animator.StringToHash后的int值；
        /// </summary>
        [Key(0)]
        public Dictionary<int, object> AnimatorHashDict { get; set; }
    }
}
