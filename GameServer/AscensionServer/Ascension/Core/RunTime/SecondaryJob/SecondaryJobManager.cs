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
            Utility.Debug.LogInfo("YZQ收到的副职业请求" + packet.DataMessage.ToString());
            Utility.Debug.LogInfo("YZQ收到的副职业请求"+ (SecondaryJobOpCode)packet.SubOperationCode);
            switch ((SecondaryJobOpCode)packet.SubOperationCode)
            {
                case SecondaryJobOpCode.GetSecondaryJobStatus:
                    #region
                    var role = Utility.Json.ToObject<RoleDTO>(packet.DataMessage.ToString());
                    GetSecondaryJobStatusS2C(role.RoleID);
                    #endregion
                    break;
                case SecondaryJobOpCode.UpdateAlchemy:
                    #region
                    secondaryJob = Utility.Json.ToObject<SecondaryJobDTO>(packet.DataMessage.ToString());
                    UpdateAlchemyS2C(secondaryJob.RoleID, secondaryJob.UseItemID);
                    #endregion
                    break;
                case SecondaryJobOpCode.CompoundAlchemy:
                    #region
                    secondaryJob = Utility.Json.ToObject<SecondaryJobDTO>(packet.DataMessage.ToString());
                    CompoundAlchemyS2C(secondaryJob.RoleID, secondaryJob.UseItemID);
                    #endregion
                    break;
                case SecondaryJobOpCode.UpdatePuppet:
                    break;
                case SecondaryJobOpCode.CompoundPuppet:
                    break;
                case SecondaryJobOpCode.UpdateForge:
                    break;
                case SecondaryJobOpCode.CompoundForge:
                    break;
                default:
                    break;
            }
        }

        void GetSecondaryJobStatusS2C(int roleID)
        {
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            Dictionary<byte, object> dict = new Dictionary<byte, object>();
            #region
            var alchemyExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlchemyPerfix, roleID.ToString()).Result;
            if (alchemyExist)
            {
                var alchemyObj = RedisHelper.Hash.HashGetAsync<AlchemyDTO>(RedisKeyDefine._AlchemyPerfix, roleID.ToString()).Result;
                if (alchemyObj == null)
                {
                    var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteriaRole);
                    if (alchemy != null)
                    {
                        alchemyObj = ChangeDataType(alchemy);
                        dict.Add((byte)ParameterCode.JobAlchemy, alchemyObj);
                    }
                    else
                    {
                        RoleStatusFailS2C(roleID, SecondaryJobOpCode.GetSecondaryJobStatus);
                        return;
                    }
                }
                else
                    dict.Add((byte)ParameterCode.JobAlchemy, alchemyObj);
            }
            else
            {
                var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteriaRole);
                if (alchemy != null)
                {
                    Utility.Debug.LogInfo("1YZQ收到的副职业请求");
                    dict.Add((byte)ParameterCode.JobAlchemy, ChangeDataType(alchemy));
                }
                else
                {
                    RoleStatusFailS2C(roleID, SecondaryJobOpCode.GetSecondaryJobStatus);
                    return;
                }
            }
            #endregion
            #region
            var forgeExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlchemyPerfix, roleID.ToString()).Result;
            if (forgeExist)
            {
                var forgeObj = RedisHelper.Hash.HashGetAsync<ForgeDTO>(RedisKeyDefine._ForgePerfix, roleID.ToString()).Result;
                if (forgeObj == null)
                {
                    var forge = NHibernateQuerier.CriteriaSelect<Forge>(nHCriteriaRole);
                    if (forge != null)
                    {
                        forgeObj = ChangeDataType(forge);
                        dict.Add((byte)ParameterCode.JobForge, forgeObj);
                    }
                    else
                    {
                        RoleStatusFailS2C(roleID, SecondaryJobOpCode.GetSecondaryJobStatus);
                        return;
                    }
                }
                else
                    dict.Add((byte)ParameterCode.JobForge, forgeObj);
            }
            else
            {
                var forge = NHibernateQuerier.CriteriaSelect<Forge>(nHCriteriaRole);
                if (forge != null)
                {
                    Utility.Debug.LogInfo("2YZQ收到的副职业请求");
                    dict.Add((byte)ParameterCode.JobForge, ChangeDataType(forge));
                }
                else
                {
                    RoleStatusFailS2C(roleID, SecondaryJobOpCode.GetSecondaryJobStatus);
                    return;
                }
            }
            #endregion
            #region
            var puppetExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlchemyPerfix, roleID.ToString()).Result;
            if (puppetExist)
            {
                var puppetObj = RedisHelper.Hash.HashGetAsync<PuppetDTO>(RedisKeyDefine._ForgePerfix, roleID.ToString()).Result;
                if (puppetObj == null)
                {
                    var puppet = NHibernateQuerier.CriteriaSelect<Puppet>(nHCriteriaRole);
                    if (puppet != null)
                    {
                        puppetObj = ChangeDataType(puppet);
                        dict.Add((byte)ParameterCode.JobPuppet, puppetObj);
                    }
                    else
                    {
                        RoleStatusFailS2C(roleID, SecondaryJobOpCode.GetSecondaryJobStatus);
                        return;
                    }
                }
                else
                    dict.Add((byte)ParameterCode.JobPuppet, puppetObj);
            }
            else
            {
                var puppet = NHibernateQuerier.CriteriaSelect<Puppet>(nHCriteriaRole);
                if (puppet != null)
                {
                    Utility.Debug.LogInfo("3YZQ收到的副职业请求");
                    dict.Add((byte)ParameterCode.JobPuppet, ChangeDataType(puppet));
                }
                else
                {
                    RoleStatusFailS2C(roleID, SecondaryJobOpCode.GetSecondaryJobStatus);
                    return;
                }
            }
            #endregion

            RoleStatusSuccessS2C(roleID,SecondaryJobOpCode.GetSecondaryJobStatus,dict);
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
            opData.ReturnCode = (byte)ReturnCode.ItemNotFound;
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


