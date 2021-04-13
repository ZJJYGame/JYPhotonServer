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
    public partial class PuppetManager : Cosmos.Module, IPuppetManager
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncRoleStatus, ProcessHandlerC2S);
        }
        void ProcessHandlerC2S(int seeionid, OperationData packet)
        {
            switch ((PuppetOpCode)packet.SubOperationCode)
            {
                case PuppetOpCode.GetPuppetStatus:
                    GetPuppetStatusS2C(1);

                    break;
                case PuppetOpCode.SetBattle:
                    var rolepuppet = Utility.Json.ToObject<RolePuppetDTO >(packet.DataMessage.ToString());
                    SetBattleS2C(rolepuppet.RoleID, rolepuppet.IsBattle);
                    break;
                case PuppetOpCode.AbandonPuppet:

                    break;
                default:
                    break;
            }
        }

        void PuppetManagerSuccessS2C(int roleID, PuppetOpCode oPcode, object data)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncPuppet;
            opData.SubOperationCode = (byte)oPcode;
            opData.ReturnCode = (byte)ReturnCode.Success;
            opData.DataMessage = Utility.Json.ToJson(data);
            GameEntry.RoleManager.SendMessage(roleID, opData);

            Utility.Debug.LogInfo("角色副职业数据发送了" + Utility.Json.ToJson(data));
        }

        void PuppetManagerFailS2C(int roleID, PuppetOpCode oPcode)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncPuppet;
            opData.SubOperationCode = (byte)oPcode;
            opData.ReturnCode = (byte)ReturnCode.ItemNotFound;
            opData.DataMessage = Utility.Json.ToJson(null);
            GameEntry.RoleManager.SendMessage(roleID, opData);
        }

    }
}
