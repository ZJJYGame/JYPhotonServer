/* 
 * Time: 2020.4.28
 * Host:xianrenzhang
 * Content:关联uuid外键
 */
using System;
using System.Collections.Generic;

namespace AscensionServer.Model
{
    [Serializable]
    public class UserRole
    {
        public virtual string UUID { get; set; }
        public virtual string Role_Id_Array { get; set; }
        public override bool Equals(object obj)
        {
            if (!(obj is UserRole))
                return false;
            var tmpRole = obj as UserRole;
            if (this.Role_Id_Array.Equals(tmpRole.Role_Id_Array) && this.UUID.Equals(tmpRole.UUID))
                return true;
            else
                return false;
        }
        public override string ToString()
        {
            return Utility.Text.Format("UUID : " + UUID + ">>" + "Role_Id_Array : " + Role_Id_Array + "\n");
        }
    }
}
