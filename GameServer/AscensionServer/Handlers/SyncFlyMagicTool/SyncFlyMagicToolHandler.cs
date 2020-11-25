﻿using AscensionProtocol;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol.DTO;
using AscensionServer.Threads;
using EventData = Photon.SocketServer.EventData;
using AscensionServer.Model;
using RedisDotNet;
using Protocol;
namespace AscensionServer
{
    public class SyncFlyMagicToolHandler:Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncRoleFlyMagicTool; } }

        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            var roleFlyMagicToolJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleFlyMagicTool));

            var roleFlyMagicToolObj = Utility.Json.ToObject<FlyMagicToolDTO>(roleFlyMagicToolJson);
            NHCriteria nHCriteriaRole = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleFlyMagicToolObj.RoleID);
            var roleFlyMagicTool= NHibernateQuerier.CriteriaSelectAsync<FlyMagicTool>(nHCriteriaRole).Result;
            var flyMagicToolRedisObj = RedisHelper.Hash.HashGetAsync<FlyMagicToolDTO>(RedisKeyDefine._RoleFlyMagicToolPerfix, roleFlyMagicToolObj.RoleID.ToString()).Result;
            switch (roleFlyMagicToolObj.OprateType)
            {
                case FlyMagicToolDTO.FlyMagicToolType.Add:
                    if (roleFlyMagicTool!=null)
                    {
                        var tempList = Utility.Json.ToObject<List<int>>(roleFlyMagicTool.AllFlyMagicTool);
                        if (!tempList.Contains(roleFlyMagicToolObj.FlyMagicToolID))
                        {
                            tempList.Add(roleFlyMagicToolObj.FlyMagicToolID);
                            roleFlyMagicToolObj.AllFlyMagicTool = tempList;
                            RedisHelper.Hash.HashSet<FlyMagicToolDTO>(RedisKeyDefine._RoleFlyMagicToolPerfix, roleFlyMagicToolObj.RoleID.ToString(), roleFlyMagicToolObj);

                            roleFlyMagicTool.AllFlyMagicTool = Utility.Json.ToJson(tempList);
                            NHibernateQuerier.Update<FlyMagicTool>(roleFlyMagicTool);

                            OperationData opData = new OperationData();
                            opData.DataMessage = Utility.Json.ToJson(roleFlyMagicToolObj);
                            opData.OperationCode = (byte)OperationCode.SyncRoleFlyMagicTool;
                            opData.ReturnCode = (byte)ReturnCode.Success;
                            GameManager.CustomeModule<RoleManager>().SendMessage(flyMagicToolRedisObj.RoleID, opData);
                        }
                        else
                        {
                            OperationData opData = new OperationData();
                            opData.DataMessage = Utility.Json.ToJson(roleFlyMagicToolObj);
                            opData.OperationCode = (byte)OperationCode.SyncRoleFlyMagicTool;
                            opData.ReturnCode = (byte)ReturnCode.Success;
                            GameManager.CustomeModule<RoleManager>().SendMessage(flyMagicToolRedisObj.RoleID, opData);
                        }
                    }
                    break;
                case FlyMagicToolDTO.FlyMagicToolType.Get:
                    if (flyMagicToolRedisObj == null)
                    {
                        if (roleFlyMagicTool!=null)
                        {
                            roleFlyMagicToolObj.AllFlyMagicTool = Utility.Json.ToObject<List<int>>(roleFlyMagicTool.AllFlyMagicTool);
                            OperationData opData = new OperationData();
                            opData.DataMessage = Utility.Json.ToJson(roleFlyMagicToolObj);
                            opData.OperationCode = (byte)OperationCode.SyncRoleFlyMagicTool;
                            opData.ReturnCode = (byte)ReturnCode.Success;
                            GameManager.CustomeModule<RoleManager>().SendMessage(flyMagicToolRedisObj.RoleID, opData);
                        }
                        else
                        {
                            OperationData opData = new OperationData();
                            opData.DataMessage = Utility.Json.ToJson(roleFlyMagicToolObj);
                            opData.OperationCode = (byte)OperationCode.SyncRoleFlyMagicTool;
                            opData.ReturnCode = (byte)ReturnCode.Fail;
                            GameManager.CustomeModule<RoleManager>().SendMessage(flyMagicToolRedisObj.RoleID, opData);
                        }
                    }
                    else
                    {
                        OperationData opData = new OperationData();
                        opData.DataMessage = Utility.Json.ToJson(flyMagicToolRedisObj);
                        opData.OperationCode = (byte)OperationCode.SyncRoleFlyMagicTool;
                        opData.ReturnCode = (byte)ReturnCode.Success;
                        GameManager.CustomeModule<RoleManager>().SendMessage(flyMagicToolRedisObj.RoleID, opData);
                    }
                    break;
                default:
                    break;
            }
            return operationResponse;
        }
    }
}
