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
namespace AscensionServer
{
    [Module]
    public partial class GangsMananger : Cosmos.Module, IGangsMananger
    {
        public override void OnPreparatory()
        {
            CommandEventCore.Instance.AddEventListener((byte)OperationCode.SyncRoleAlliance, ProcessHandlerC2S);
        }
        void ProcessHandlerC2S(int seeionid, OperationData packet)
        {
            var roleObj=new RoleDTO();
            var dict = new Dictionary<byte,object>();
            var allianceObj = new AlliancesDTO();
            var roleAllianceObj = new RoleAllianceDTO();
            var allianceStatusObj = new AllianceStatus();
            Utility.Debug.LogInfo("角色宗門" + packet.DataMessage.ToString());
            Utility.Debug.LogInfo("角色宗門" + (byte)packet.SubOperationCode);
            switch ((AllianceOpCode)packet.SubOperationCode)
            {
                case AllianceOpCode.CreatAlliance:
                    #region 
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    roleObj = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    allianceStatusObj = Utility.Json.ToObject<AllianceStatus>(dict[(byte)ParameterCode.AllianceStatus].ToString());
                    Utility.Debug.LogInfo("创建宗門进来" + roleObj.RoleID + "" + Utility.Json.ToJson(allianceStatusObj));
                    CreatAllianceS2C(roleObj.RoleID, allianceStatusObj);
                    #endregion
                    break;
                case AllianceOpCode.JoinAlliance:
                    #region
                    roleAllianceObj = Utility.Json.ToObject<RoleAllianceDTO>(packet.DataMessage.ToString());
                    Utility.Debug.LogInfo("申请加入宗門进来" + roleObj.RoleID + "" + Utility.Json.ToJson(roleAllianceObj));
                    ApplyJoinAllianceS2C(roleAllianceObj.RoleID, roleAllianceObj.AllianceID);
                    #endregion
                    break;
                case AllianceOpCode.GetAlliances:
                    #region
                    Utility.Debug.LogInfo("获得角色宗門" + packet.DataMessage.ToString());
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    roleObj = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    allianceObj = Utility.Json.ToObject<AlliancesDTO>(dict[(byte)ParameterCode.Alliances].ToString());
                    GetAllAllianceS2C(roleObj.RoleID, allianceObj);
                    #endregion
                    break;
                case AllianceOpCode.GetAllianceStatus:
                    #region
                    Utility.Debug.LogInfo("获得宗門建设数据" + packet.DataMessage.ToString());
                    roleAllianceObj = Utility.Json.ToObject<RoleAllianceDTO>(packet.DataMessage.ToString());
                    GetAllianceConstructionS2C(roleAllianceObj.AllianceID, roleAllianceObj.RoleID);
                    #endregion
                    break;
                case AllianceOpCode.GetRoleAlliance:
                    roleObj = Utility.Json.ToObject<RoleDTO>(packet.DataMessage.ToString());
                    GetRoleAllianceS2C(roleObj.RoleID);
                    break;

                case AllianceOpCode.BuildAlliance:


                    break;
                case AllianceOpCode.GetAllianceMember:

                    break;
                case AllianceOpCode.QuitAlliance:
                    break;
                case AllianceOpCode.AllianceSignin:
                    break;
                case AllianceOpCode.RefuseApply:
                    break;
                case AllianceOpCode.ConsentApply:
                   // ConsentApplyS2C();
                    break;
                case AllianceOpCode.UpdateAllianceSkill:
                    break;
                case AllianceOpCode.ChangeAllianceName:
                    break;
                case AllianceOpCode.AllianceActivity:
                    break;
                case AllianceOpCode.ChangeAlliancePurpose:
                    break;
                case AllianceOpCode.CareerAdvancement:
                    break;
                case AllianceOpCode.SearchAlliance:
                    break;
                case AllianceOpCode.ExchangeElixir:
                    break;
                case AllianceOpCode.ExchangeGongFa:
                    break;
                case AllianceOpCode.ExchangeMiShu:
                    break;
                case AllianceOpCode.SetExchangeGoods:
                    break;
                case AllianceOpCode.GetAlliancecallboard:
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    roleAllianceObj = Utility.Json.ToObject<RoleAllianceDTO>(dict[(byte)ParameterCode.RoleAlliance].ToString());
                    var daily = Utility.Json.ToObject<DailyMessageDTO>(dict[(byte)ParameterCode.DailyMessage].ToString());
                    GetAllianceCallboardS2C(roleAllianceObj.RoleID, roleAllianceObj.AllianceID, daily);

                    break;
                case AllianceOpCode.PreemptDongFu:
                    break;
                case AllianceOpCode.GetDongFuStatus:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 处理角色宗门成功发送
        /// </summary>
        void RoleStatusSuccessS2C(int roleID, AllianceOpCode oPcode, object data)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncRoleAlliance;
            opData.SubOperationCode = (byte)oPcode;
            opData.DataMessage = Utility.Json.ToJson(data);
            GameEntry.RoleManager.SendMessage(roleID, opData);
            Utility.Debug.LogInfo("角色宗門数据发送了" + Utility.Json.ToJson(data));
        }
        /// <summary>
        /// 处理角色宗门失败发送
        /// </summary>
        void RoleStatusFailS2C(int roleID, AllianceOpCode oPcode)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncRoleAlliance;
            opData.SubOperationCode = (byte)oPcode;
            opData.DataMessage = Utility.Json.ToJson(null);
            GameEntry.RoleManager.SendMessage(roleID, opData);
        }
    }
}
