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
            var allianceConstructionObj = new AllianceConstructionDTO();
            var allianceExchangeGoodsDTO = new AllianceExchangeGoodsDTO();
            var roleAllianceSkillDTO = new RoleAllianceSkillDTO();
            var exchangeObj =new ExchangeDTO();
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
                    #region
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    roleObj = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    allianceConstructionObj = Utility.Json.ToObject<AllianceConstructionDTO>(dict[(byte)ParameterCode.AllianceConstruction].ToString());
                    Utility.Debug.LogInfo("YZQ升级宗门建设成员");
                    BuildAllianceConstructionS2C(allianceConstructionObj.AllianceID, roleObj.RoleID, allianceConstructionObj);
                    #endregion
                    break;
                case AllianceOpCode.GetAllianceMember:
                    roleAllianceObj= Utility.Json.ToObject<RoleAllianceDTO>(packet.DataMessage.ToString());
                    GetAllianceMemberS2C(roleAllianceObj.RoleID, roleAllianceObj.AllianceID);

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
                    roleAllianceSkillDTO = Utility.Json.ToObject<RoleAllianceSkillDTO>(packet.DataMessage.ToString());
                    Utility.Debug.LogInfo("角色宗門技能升级");
                    UpdateAllianceSkillS2C(roleAllianceSkillDTO.RoleID, roleAllianceSkillDTO);
                    break;
                case AllianceOpCode.GetAllianceSkill:
                    #region 
                    roleObj = Utility.Json.ToObject<RoleDTO>(packet.DataMessage.ToString());
                    GetAllianceSkillS2C(roleObj.RoleID);
                    #endregion
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
                    #region
                    exchangeObj = Utility.Json.ToObject<ExchangeDTO>(packet.DataMessage.ToString());
                    Utility.Debug.LogInfo("YZQ兑换宗门丹药");
                    ExchangeElixirS2C(exchangeObj.RoleID, exchangeObj.AllianceID, exchangeObj);
                    #endregion
                    break;
                case AllianceOpCode.ExchangeScripturesPlatform:
                    #region
                    exchangeObj = Utility.Json.ToObject<ExchangeDTO>(packet.DataMessage.ToString());
                    Utility.Debug.LogInfo("YZQ兌換藏經閣數據");
                    ExchangeScripturesPlatformS2C(exchangeObj.RoleID, exchangeObj.AllianceID, exchangeObj);
                    #endregion
                    break;
                case AllianceOpCode.SetExchangeGoods:
                    #region
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    roleObj = Utility.Json.ToObject<RoleDTO>(dict[(byte)ParameterCode.Role].ToString());
                    allianceExchangeGoodsDTO = Utility.Json.ToObject<AllianceExchangeGoodsDTO>(dict[(byte)ParameterCode.ExchangeGoods].ToString());
                    Utility.Debug.LogInfo("角色宗門设置兑换数据");
                    SetExchangeGoodsS2C(roleObj.RoleID, allianceExchangeGoodsDTO);
                    #endregion
                    break;
                case AllianceOpCode.GetAlliancecallboard:
                    #region
                    dict = Utility.Json.ToObject<Dictionary<byte, object>>(packet.DataMessage.ToString());
                    roleAllianceObj = Utility.Json.ToObject<RoleAllianceDTO>(dict[(byte)ParameterCode.RoleAlliance].ToString());
                    var daily = Utility.Json.ToObject<DailyMessageDTO>(dict[(byte)ParameterCode.DailyMessage].ToString());
                    GetAllianceCallboardS2C(roleAllianceObj.RoleID, roleAllianceObj.AllianceID, daily);

                    #endregion
                    break;
                case AllianceOpCode.PreemptDongFu:
                    break;
                case AllianceOpCode.GetDongFuStatus:
                    var Obj = Utility.Json.ToObject<RoleAllianceDTO>(packet.DataMessage.ToString());
                    GetDongFuStatusS2C(Obj.RoleID, Obj.AllianceID);
                    break;
                case AllianceOpCode.GetExchangeGoods:
                    #region
                    roleAllianceObj = Utility.Json.ToObject<RoleAllianceDTO>(packet.DataMessage.ToString());
                    GetExchangeGoodsS2C(roleAllianceObj.RoleID, roleAllianceObj.AllianceID);
                    #endregion
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
