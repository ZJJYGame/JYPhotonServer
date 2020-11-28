using AscensionProtocol;
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
    public class SyncDemonicSoulHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncRoleFlyMagicTool; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            var demonicsoulJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleFlyMagicTool));
            var demonicsoulObj = Utility.Json.ToObject<DemonicSoulDTO>(demonicsoulJson);
            NHCriteria nHCriteriaRole = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", demonicsoulObj.RoleID);
            var demonicSoul = NHibernateQuerier.CriteriaSelectAsync<DemonicSoul>(nHCriteriaRole).Result;

            switch (demonicsoulObj.OperateType)
            {
                case DemonicSoulOperateType.Add:
                    GameManager.CustomeModule<DemonicSoulManager>().AddDemonical( demonicsoulObj.RoleID, demonicSoul,demonicsoulObj.CompoundList[0], nHCriteriaRole);
                    break;
                case DemonicSoulOperateType.Compound:

                    GameManager.CustomeModule<DemonicSoulManager>().CompoundDemonical(demonicsoulObj.CompoundList, demonicSoul, demonicsoulObj.RoleID, nHCriteriaRole);
                    break;
                case DemonicSoulOperateType.Get:

                    break;
                default:
                    break;
            }
            return operationResponse;
        }
    }
}
