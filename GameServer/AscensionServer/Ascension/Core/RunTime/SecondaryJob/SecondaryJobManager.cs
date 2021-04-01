using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using Protocol;
using RedisDotNet;

namespace AscensionServer
{
    [Module]
    public partial class SecondaryJobManager : Cosmos. Module,ISecondaryJobManager
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncSecondaryJob, ProcessHandlerC2S);
        }

        void ProcessHandlerC2S(int seeionid, OperationData packet)
        {
            var secondaryJob = new SecondaryJobDTO();
            switch ((SecondaryJobOpCode)packet.SubOperationCode)
            {
                case SecondaryJobOpCode.GetAlchemyStatus:

                    break;
                case SecondaryJobOpCode.UpdateAlchemy:
                    break;
                case SecondaryJobOpCode.CompoundAlchemy:
                    break;
                case SecondaryJobOpCode.GetPuppetStatus:
                    break;
                case SecondaryJobOpCode.UpdatePuppet:
                    break;
                case SecondaryJobOpCode.CompoundPuppet:
                    break;
                case SecondaryJobOpCode.GetWeaponStatus:
                    break;
                case SecondaryJobOpCode.UpdateWeapon:
                    break;
                case SecondaryJobOpCode.CompoundWeapon:
                    break;
                default:
                    break;
            }
        }

        void RoleStatusSuccessS2C(int roleID, SecondaryJobOpCode oPcode, object data)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncSecondaryJob;
            opData.SubOperationCode = (byte)oPcode;
            opData.ReturnCode = (byte)ReturnCode.Success;
            opData.DataMessage = Utility.Json.ToJson(data);
            GameEntry.RoleManager.SendMessage(roleID, opData);

            Utility.Debug.LogInfo("角色副职业数据发送了" + Utility.Json.ToJson(data));
        }

        void RoleStatusFailS2C(int roleID, SecondaryJobOpCode oPcode)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncRoleAlliance;
            opData.SubOperationCode = (byte)oPcode;
            opData.ReturnCode = (byte)ReturnCode.Fail;
            opData.DataMessage = Utility.Json.ToJson(null);
            GameEntry.RoleManager.SendMessage(roleID, opData);
        }
        /// <summary>
        /// 合成失敗
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="oPcode"></param>
        void RoleStatusCompoundFailS2C(int roleID, SecondaryJobOpCode oPcode, object data)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncSecondaryJob;
            opData.SubOperationCode = (byte)oPcode;
            opData.ReturnCode = (byte)ReturnCode.Fail;
            opData.DataMessage = Utility.Json.ToJson(data);
            GameEntry.RoleManager.SendMessage(roleID, opData);
        }
    }
}


