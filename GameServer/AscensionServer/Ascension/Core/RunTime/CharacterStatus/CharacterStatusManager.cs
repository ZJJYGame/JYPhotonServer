using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    [Module]
    public class CharacterStatusManager:Cosmos. Module,ICharacterStatusManager
    {
        public List<CultivationMethodDTO> GetRoleGongFa(int roleId)
        {
            var gongFaList = new List<CultivationMethodDTO>();
            var gongFaIdArray = NHibernateQuerier.Query<RoleGongFa>("RoleID", roleId);
            return null;
        }
    }
}
