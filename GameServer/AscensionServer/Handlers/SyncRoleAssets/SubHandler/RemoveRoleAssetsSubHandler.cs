﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
using RedisDotNet;
namespace AscensionServer
{
    public class RemoveRoleAssetsSubHandler : SyncRoleAssetsSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Remove;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            ResetResponseData(operationRequest);
            string roleAssetsJson = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ParameterCode.RoleAssets));

            var roleAssetsObj = Utility.Json.ToObject<RoleAssets>(roleAssetsJson);
            NHCriteria nHCriteriaRoleID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleAssetsObj.RoleID);
            bool roleExist = NHibernateQuerier.Verify<Role>(nHCriteriaRoleID);
            bool roleAssetsExist = NHibernateQuerier.Verify<RoleAssets>(nHCriteriaRoleID);
            long SpiritStonesLow = 0;
            long SpiritStonesHigh = 0;
            long SpiritStonesMedium = 0;
            long XianYu = 0;
            if (roleExist && roleAssetsExist)
            {
               
                var assetsServer = NHibernateQuerier.CriteriaSelect<RoleAssets>(nHCriteriaRoleID);
                SpiritStonesLow = assetsServer.SpiritStonesLow;
                if (roleAssetsObj.SpiritStonesLow > 0&& roleAssetsObj.SpiritStonesLow <=assetsServer.SpiritStonesLow)
                    SpiritStonesLow = assetsServer.SpiritStonesLow- roleAssetsObj.SpiritStonesLow ;
                Utility.Debug.LogInfo(">>>>>>>>>>>>>減少的資產：" + SpiritStonesLow + ">>>>>>>>>>>>>>>>>>>>>>");
                if (roleAssetsObj.XianYu > 0&& roleAssetsObj.XianYu <= assetsServer.XianYu)
                    XianYu = assetsServer.XianYu- roleAssetsObj.XianYu ;
                NHibernateQuerier.Update<RoleAssets>(new RoleAssets() { RoleID = roleAssetsObj.RoleID, SpiritStonesLow = SpiritStonesLow, XianYu = XianYu });
                RedisHelper.Hash.HashSetAsync<RoleAssets>("RoleAssets", roleAssetsObj.RoleID.ToString(), new RoleAssets() { RoleID = roleAssetsObj.RoleID, SpiritStonesLow = SpiritStonesLow, XianYu = XianYu });
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
            }
            else
                Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
            Utility.Debug.LogInfo(">>>>>>>>>>>>>發送囘u去：" + roleAssetsJson + ">>>>>>>>>>>>>>>>>>>>>>");
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRoleID);
        }
    }
}
