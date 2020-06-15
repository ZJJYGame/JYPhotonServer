using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    [Obsolete("弃用，移步RoleTransformDTO")]

    public class PlayerDataDTO: ProtocolDTO
    {
        public Vector3DataDTO pos { get; set; }
        public string Username { get; set; }
        public override void Clear()
        {
            Username = null;
            pos = null;
        }
    }
}
