using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Cosmos;
using Protocol;
using AscensionProtocol.DTO;
namespace AscensionServer
{
    [Module]
    public partial class RoleStatusManager:Cosmos.Module, IRoleStatusManager
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncRoleStatus, ProcessHandlerC2S);
        }
        void ProcessHandlerC2S(int seeionid, OperationData packet)
        {
            var pointObj = Utility.Json.ToObject<RoleStatusPointDTO>(packet.DataMessage.ToString());
            Utility.Debug.LogInfo("YZQ加点数据"+packet.DataMessage.ToString());
            switch ((RoleStatusOPcode)packet.SubOperationCode)
            {
                case RoleStatusOPcode.GetStatus:
                    Utility.Debug.LogInfo("YZQ加点数据"+ (RoleStatusOPcode)packet.SubOperationCode);
                    GetRolePointAbilityS2C(pointObj);
                    break;
                case RoleStatusOPcode.UpdateStatus:
                    Utility.Debug.LogInfo("YZQ加点数据" + (RoleStatusOPcode)packet.SubOperationCode);
                    break;
                case RoleStatusOPcode.Rename:
                    Utility.Debug.LogInfo("YZQ加点数据" + (RoleStatusOPcode)packet.SubOperationCode);
                    SetRoleSlnNameS2C(pointObj);
                    break;
                case RoleStatusOPcode.RestartAddPoint:
                    Utility.Debug.LogInfo("YZQ加点数据" + (RoleStatusOPcode)packet.SubOperationCode);
                    RestartPointS2C(pointObj);
                    break;
                case RoleStatusOPcode.SetAddPoint:
                    Utility.Debug.LogInfo("YZQ加点数据" + (RoleStatusOPcode)packet.SubOperationCode);
                    SetRolePointS2C(pointObj);
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// 处理角色属性成功发送
        /// </summary>
        void RoleStatusSuccessS2C(int roleID,RoleStatusOPcode oPcode,object data)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncRoleStatus;
            opData.SubOperationCode = (byte)oPcode;
            opData.DataMessage = Utility.Json.ToJson(data);
            GameEntry.RoleManager.SendMessage(roleID, opData);
            Utility.Debug.LogInfo("YZQ加点数据发送成功" + Utility.Json.ToJson(opData));
        }
        /// <summary>
        /// 处理角色属性失败发送
        /// </summary>
        void RoleStatusFailS2C(int roleID, RoleStatusOPcode oPcode)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncRoleStatus;
            opData.SubOperationCode = (byte)oPcode;
            opData.DataMessage = Utility.Json.ToJson(null);
            GameEntry.RoleManager.SendMessage(roleID, opData);
        }
    }
}
