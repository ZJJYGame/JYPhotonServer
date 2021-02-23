using AscensionProtocol;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using Cosmos;
using AscensionProtocol.DTO;
using EventData = Photon.SocketServer.EventData;
using AscensionServer.Model;
using RedisDotNet;
using Protocol;

namespace AscensionServer
{
    public class SyncDemonicSoulHandler : Handler
    {
        public override byte OpCode { get { return (byte)OperationCode.SyncDemonical; } }
        protected override OperationResponse OnOperationRequest(OperationRequest operationRequest)
        {
            var demonicsoulJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.DemonicSoul));
            var demonicsoulObj = Utility.Json.ToObject<DemonicSoulDTO>(demonicsoulJson);
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", demonicsoulObj.RoleID);
            var demonicSoul = NHibernateQuerier.CriteriaSelectAsync<DemonicSoul>(nHCriteriaRole).Result;
            Utility.Debug.LogInfo("yzqData"+ demonicsoulJson);
            switch (demonicsoulObj.OperateType)
            {
                case DemonicSoulOperateType.Add:
                    GameEntry.DemonicSoulManager.AddDemonical( demonicsoulObj.RoleID, demonicSoul,demonicsoulObj.CompoundList[0], nHCriteriaRole);
                    break;
                case DemonicSoulOperateType.Compound:

                    GameEntry.DemonicSoulManager.CompoundDemonical(demonicsoulObj.CompoundList, demonicSoul, demonicsoulObj.RoleID, nHCriteriaRole);
                    break;
                case DemonicSoulOperateType.Get:
                    GameEntry.DemonicSoulManager.GetDemonicSoul(demonicsoulObj.RoleID, demonicSoul);
                    break;
                default:
                    break;
            }
            return operationResponse;
        }
    }
}


