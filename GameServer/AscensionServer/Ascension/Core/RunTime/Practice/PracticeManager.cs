using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using Protocol;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;

namespace AscensionServer
{
    [Module]
    public partial  class PracticeManager: Cosmos.Module, IPracticeManager
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncPractice, ProcessHandlerC2S);
        }

        void ProcessHandlerC2S(int seeionid ,OperationData packet)
        {
            var dict = Utility.Json.ToObject<Dictionary<byte, object>>(Convert.ToString(packet.DataMessage));
            //Utility.Debug.LogInfo("YZQjueseid为" + Convert.ToString(packet.DataMessage));
            RoleDTO role;
            OnOffLineDTO onOffLine;
            foreach (var item in dict)
            {

                Utility.Debug.LogInfo("YZQ接受数据进来了");
                switch ((PracticeOpcode)item.Key)
                {
                    case PracticeOpcode.GetRoleGongfa:
                        role = Utility.Json.ToObject<RoleDTO>(item.Value.ToString());
                        GetRoleGongFaS2C(role.RoleID);
                        break;
                    case PracticeOpcode.AddGongFa:
                        break;
                    case PracticeOpcode.GetRoleMiShu:
                        role = Utility.Json.ToObject<RoleDTO>(item.Value.ToString());
                        GetRoleMiShuS2C(role.RoleID);
                        break;
                    case PracticeOpcode.AddMiShu:
                        break;
                    case PracticeOpcode.SwitchPracticeType:
                        Utility.Debug.LogInfo("YZQjueseid为" + Convert.ToString(packet.DataMessage));
                        onOffLine = Utility.Json.ToObject<OnOffLineDTO>(item.Value.ToString());
                        SwitchPracticeTypeS2C(onOffLine);
                        break;
                    case PracticeOpcode.UploadingExp:

                        break;
                    case PracticeOpcode.GetOffLineExp:
                        role = Utility.Json.ToObject<RoleDTO>(item.Value.ToString());
                        GetOffLineExpS2C(role.RoleID);
                        break;
                    case PracticeOpcode.TriggerBottleneck:
                        break;
                    case PracticeOpcode.UseBottleneckElixir:
                        break;
                    case PracticeOpcode.UpdateBottleneck:
                        break;
                    case PracticeOpcode.DemonicFail:
                        break;
                    case PracticeOpcode.ThunderRoundFail:
                        break;
                    default:
                        break;
                }
            }








        }

        /// <summary>
        /// 失败返回
        /// </summary>
        void ResultFailS2C(int roleID, PracticeOpcode opcode)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncPractice;
            var dataDict = new Dictionary<byte, object>();
            dataDict.Add((byte)opcode, null);
            opData.DataMessage = Utility.Json.ToJson(dataDict);
            GameEntry.RoleManager.SendMessage(roleID, opData);
        }
        /// <summary>
        /// 结果成功返回
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="opcode"></param>
        /// <param name="data"></param>
        void ResultSuccseS2C(int roleID, PracticeOpcode opcode,object data)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncPractice;
            var dataDict = new Dictionary<byte, object>();
            dataDict.Add((byte)opcode, Utility.Json.ToJson(data));
            opData.DataMessage = Utility.Json.ToJson(dataDict);
            GameEntry.RoleManager.SendMessage(roleID, opData);
            Utility.Debug.LogInfo("yzqjueseid发送成功" + Utility.Json.ToJson(opData));
        }
    }
}
