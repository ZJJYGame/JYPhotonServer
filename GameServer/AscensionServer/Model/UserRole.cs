/* 
 * Time: 2020.4.28
 * Host:xianrenzhang
 * Content:关联uuid外键
 */

namespace AscensionServer.Model
{
    public class UserRole
    {
        public virtual string UUID { get; set; }
        public virtual string Role_Id_Array { get; set; }

       
    }
}
