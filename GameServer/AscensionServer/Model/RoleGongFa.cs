using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/* 
 * 
 * Time: 2020.4.28
 * Host:xianrenzhang
 * Content:功法
 */
namespace AscensionServer.Model
{
    public class RoleGongFa
    {
        public virtual int Id { get; set; }
        public virtual string GongFaId { get; set; }
        public virtual byte GongFaInfluenceType { get; set; }
        public virtual byte GongFaInfluenceVaule { get; set; }
        public virtual Role Role_Id { get; set; }
    }
}
