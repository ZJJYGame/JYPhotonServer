using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;
namespace AscensionServer
{
    public interface IPracticeManager: IModuleManager
    {
        Task<RoleStatus> RoleAblility(RoleStatusPointDTO pointDTO, RoleStatus roleStatus);

        Task<RoleStatus> RoleEquip(RoleWeaponDTO roleWeapon, RoleEquipmentDTO roleEquipment);
    }
}
