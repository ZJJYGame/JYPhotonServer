using System;
using System.Collections.Generic;
using System.Text;

namespace AscensionServer
{
    [Serializable]
    public class MySqlData : Data
    {
        public string Address { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public override void SetData(object data)
        {
            var dat = data as MySqlData;
            Address = dat.Address;
            Database = dat.Database;
            Username = dat.Username;
            Password = dat.Password;
        }
        public override string ToString()
        {
            return $"Address:{Address};Database:{Database};Username:{Username};Password:{Password}";
        }
    }
}
