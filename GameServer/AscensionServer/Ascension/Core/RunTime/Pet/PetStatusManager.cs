using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using RedisDotNet;
using AscensionProtocol;

namespace AscensionServer
{
    [Module]
    public partial class PetStatusManager : Cosmos.Module, IPetStatusManager
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncRolePet, ProcessHandlerC2S);
        }
        void ProcessHandlerC2S(int seeionid, OperationData packet)
        {
            var rolepet = new RolePetDTO();
            var role = new RoleDTO();
            var pet = new PetDTO();
            var dict = new Dictionary<byte, object>();
            var useitem = new RoleUseItemDTO();
            PetAbilityPointDTO pointDTO;
            switch ((RolePetOpCode)packet.SubOperationCode)
            {
                case RolePetOpCode.GetRolePet:
                    rolepet = Utility.Json.ToObject<RolePetDTO>(packet.DataMessage.ToString());
                    GetRoleAllPetS2C(rolepet);
                    break;
                case RolePetOpCode.RemovePet:
                    rolepet = Utility.Json.ToObject<RolePetDTO>(packet.DataMessage.ToString());
                    RemoveRolePet(rolepet);
                    break;
                case RolePetOpCode.AddPet:
                    rolepet = Utility.Json.ToObject<RolePetDTO>(packet.DataMessage.ToString());
                    InitPet(rolepet.AddRemovePetID, rolepet.AddPetName, rolepet.RoleID);
                    break;
                case RolePetOpCode.SetBattle:
                    rolepet = Utility.Json.ToObject<RolePetDTO>(packet.DataMessage.ToString());
                    RolePetSetBattle(rolepet);
                    break;
                case RolePetOpCode.ResetPetAbilitySln:
                    Utility.Debug.LogError("重置加点加点收到的数据" + packet.DataMessage.ToString());
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    role = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    pointDTO = Utility.Json.ToObject<PetAbilityPointDTO>(dict[(byte)ParameterCode.PetAbility].ToString());
                    ResetAbilityPoint(role.RoleID, pointDTO);
                    break;
                case RolePetOpCode.ResetPetStatus:
                    useitem = Utility.Json.ToObject<RoleUseItemDTO>(packet.DataMessage.ToString());
                    PetStatusRestoreDefault(useitem.UseItemID,useitem.RoleID, useitem.PetID);
                    break;
                case RolePetOpCode.PetEvolution:
                    useitem = Utility.Json.ToObject<RoleUseItemDTO>(packet.DataMessage.ToString());
                    Utility.Debug.LogError("收到宠物进阶" + Utility.Json.ToJson(useitem));
                    PetEvolution(useitem.RoleID, useitem.PetID, useitem.UseItemID);
                    break;
                case RolePetOpCode.EquipDemonicSoul:
                    useitem = Utility.Json.ToObject<RoleUseItemDTO>(packet.DataMessage.ToString());
                    EquipDemonicSoul(useitem.RoleID, useitem.UseItemID, useitem.PetID);
                    break;
                case RolePetOpCode.PetStudySkill:
                    useitem = Utility.Json.ToObject<RoleUseItemDTO>(packet.DataMessage.ToString());
                    PetStudySkill(useitem.UseItemID, useitem.PetID,useitem.RoleID);
                    break;
                case RolePetOpCode.PetCultivate:
                    useitem = Utility.Json.ToObject<RoleUseItemDTO>(packet.DataMessage.ToString());
                    Utility.Debug.LogError("收到宠物使用丹药"+Utility.Json.ToJson(useitem));
                    PetCultivate(useitem.UseItemID, useitem.PetID, useitem.RoleID);
                    break;
                case RolePetOpCode.GetPetStatus:
                     dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    role = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    pet = Utility.Json.ToObject<PetDTO>(dict[(byte)ParameterCode.Pet].ToString());
                    GetPetAllCompeleteStatus(pet.ID, role.RoleID);
                    break;
                case RolePetOpCode.SwitchPetAbilitySln:
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    role = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    pointDTO = Utility.Json.ToObject<PetAbilityPointDTO>(dict[(byte)ParameterCode.PetAbility].ToString());
                    SwitchPetAbilitySlnS2C(role.RoleID, pointDTO);
                    break;
                case RolePetOpCode.SetAdditionPoint:
                     dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    role = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    pointDTO = Utility.Json.ToObject<PetAbilityPointDTO>(dict[(byte)ParameterCode.PetAbility].ToString());
                    Utility.Debug.LogError("加点收到的数据"+Utility.Json.ToJson(pointDTO));
                    UpdatePointSln(role.RoleID, pointDTO);
                    break;
                case RolePetOpCode.RenamePet:
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    role = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    pet= Utility.Json.ToObject<PetDTO>(dict[(byte)ParameterCode.Pet].ToString());
                    PetRename(role.RoleID, pet);
                    break;
                case RolePetOpCode.RemoveDemonicSoul:
                    useitem = Utility.Json.ToObject<RoleUseItemDTO>(packet.DataMessage.ToString());
                    UnEquipDemonicSoul(useitem.UseItemID, useitem.PetID, useitem.RoleID);
                    break;
                case RolePetOpCode.UnlockPetAbilitySln:
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    role = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    pointDTO = Utility.Json.ToObject<PetAbilityPointDTO>(dict[(byte)ParameterCode.PetAbility].ToString());
                    UlockPointSln(role.RoleID, pointDTO);
                    break;
                case RolePetOpCode.RenamePetAbilitySln:
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    role = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    pointDTO = Utility.Json.ToObject<PetAbilityPointDTO>(dict[(byte)ParameterCode.PetAbility].ToString());
                    RenamePointSln(role.RoleID, pointDTO);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 结果成功返回
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="opcode"></param>
        /// <param name="data"></param>
        void ResultSuccseS2C(int roleID, RolePetOpCode opcode, object data)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncRolePet;
            opData.SubOperationCode = (byte)opcode;
            opData.DataMessage = Utility.Json.ToJson(data);
            opData.ReturnCode = (short)ReturnCode.Success;
            GameEntry.RoleManager.SendMessage(roleID, opData);
            Utility.Debug.LogInfo("YZQ宠物相关发送发送成功" + Utility.Json.ToJson(opData));
        }

        /// <summary>
        /// 失败返回
        /// </summary>
        void ResultFailS2C(int roleID, RolePetOpCode opcode)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncRolePet;
            opData.SubOperationCode = (byte)opcode;
            var dataDict = new Dictionary<byte, object>();
            dataDict.Add((byte)opcode, null);
            opData.ReturnCode = (short)ReturnCode.Fail;
            opData.DataMessage = Utility.Json.ToJson(dataDict);
            GameEntry.RoleManager.SendMessage(roleID, opData);
        }
    }
}


