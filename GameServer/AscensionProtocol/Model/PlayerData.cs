using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AscensionProtocol.Model
{
    [Serializable]
    public class PlayerData
    {
        public Vector3Data pos { get; set; }
        public string Username { get; set; }
    }
}
