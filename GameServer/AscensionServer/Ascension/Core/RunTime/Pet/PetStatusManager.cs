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
                    break;
                case RolePetOpCode.SetBattle:
                    rolepet = Utility.Json.ToObject<RolePetDTO>(packet.DataMessage.ToString());
                    RolePetSetBattle(rolepet);
                    break;
                case RolePetOpCode.ResetPetAbilitySln:
                    break;
                case RolePetOpCode.ResetPetStatus:
                    break;
                case RolePetOpCode.PetEvolution:
                    break;
                case RolePetOpCode.EquipDemonicSoul:
                    break;
                case RolePetOpCode.PetStudySkill:
                    break;
                case RolePetOpCode.PetCultivate:
                    break;
                case RolePetOpCode.GetPetStatus:
                    var dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    role = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    pet = Utility.Json.ToObject<PetDTO>(dict[(byte)ParameterCode.Pet].ToString());
                    GetPetAllCompeleteStatus(pet.ID, role.RoleID);
                    break;
                case RolePetOpCode.SwitchPetAbilitySln:
                    break;
                case RolePetOpCode.SetAdditionPoint:
                    break;
                case RolePetOpCode.RenamePet:
                    break;
                case RolePetOpCode.RemoveDemonicSoul:
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


