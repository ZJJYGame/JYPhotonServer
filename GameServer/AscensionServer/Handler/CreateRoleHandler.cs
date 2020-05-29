﻿/*
*Author : xianrenZhang
*Since 	:2020-04-18
*Description  : 创建角色处理者
*/
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Photon.SocketServer;
using System.Collections.Generic;
using System;
namespace AscensionServer
{
    class CreateRoleHandler : HandlerBase
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.CreateRole;
            base.OnInitialization();
        }
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roleJsonTmp = Convert.ToString( Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            Role roleTmp = Utility.ToObject<Role>(roleJsonTmp);
            NHCriteria nHCriteriaRoleName = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleName", roleTmp.RoleName);
            var isExisted = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleName);
            if (isExisted)
                AscensionServer.log.Info("----------------------------  Role >>Role name:+" + roleTmp.RoleName + " already exist !!!  ---------------------------------");
            Role role = Singleton<NHManager>.Instance.CriteriaGet<Role>(nHCriteriaRoleName);//根据username查询数据
            OpResponse.OperationCode = operationRequest.OperationCode;
            string str_uuid = peer.User.UUID;
            NHCriteria nHCriteriaUUID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("UUID", str_uuid);
            var userRole = Singleton<NHManager>.Instance.CriteriaGet<UserRole>(nHCriteriaUUID);
            string roleJson = userRole.RoleIDArray;
            string roleStatusJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.RoleStatus));
            ResponseData.Clear();
            //如果没有查询到代表角色没被注册过可用
            if (role == null)
            {
                List<string> roleList = new List<string>();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList = Utility.ToObject<List<string>>(roleJson);
                //添加输入的用户进数据库
                role = roleTmp;
                var rolestatus = Utility.ToObject<RoleStatus>(roleStatusJson);
                role = Singleton<NHManager>.Instance.Add<Role>(role);
                string roleId = role.RoleID.ToString();
                if (!string.IsNullOrEmpty(roleJson))
                    roleList.Add(roleId);
                else
                    roleList.Add(roleId);
                rolestatus.RoleID = int.Parse(roleId);
                Singleton<NHManager>.Instance.Add(rolestatus);
                Singleton<NHManager>.Instance.Add(new RoleAssets() { RoleID = rolestatus.RoleID });
                var userRoleJson = Utility.ToJson(roleList);
                Singleton<NHManager>.Instance.Update(new UserRole() { RoleIDArray = userRoleJson, UUID = str_uuid });
                OpResponse.ReturnCode = (short)ReturnCode.Success;
                ResponseData.Add((byte)ObjectParameterCode.Role, Utility.ToJson(role));
                OpResponse.Parameters = ResponseData; ;
            }
            else
            {
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            }
            //把上面的回应给客户端
            peer.SendOperationResponse(OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaUUID,nHCriteriaRoleName);
        }
    }
}
