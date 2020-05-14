using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.DTO
{
    [Serializable]
    public class PlayerDataDTO: ProtocolDTO
    {
        public Vector3DataDTO pos { get; set; }
        public string Username { get; set; }


    }
}
