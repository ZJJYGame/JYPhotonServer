﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    public class UpdateRoleAllianceSubHandler : SyncRoleAllianceSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }


        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string roleallianceJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RoleAlliance));
            var roleallianceObj = Utility.Json.ToObject<RoleAllianceDTO>(roleallianceJson);
            Utility.Debug.LogError("储存的成员" + roleallianceJson);

            NHCriteria nHCriteriaroleAlliances = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleallianceObj.RoleID);
            var roleallianceTemp = ConcurrentSingleton<NHManager>.Instance.CriteriaSelectAsync<RoleAlliance>(nHCriteriaroleAlliances).Result;
            if (roleallianceTemp!=null)
            {
                roleallianceTemp.Reputation += roleallianceObj.Reputation;
                roleallianceTemp.ReputationHistroy += roleallianceObj.ReputationHistroy;
                roleallianceTemp.ReputationMonth += roleallianceObj.ReputationMonth;
                roleallianceTemp.AllianceJob = roleallianceObj.AllianceJob;
                if (roleallianceTemp.Reputation >= 0 && roleallianceTemp.ReputationHistroy >= 0 && roleallianceTemp.ReputationMonth >= 0)
                {
                    ConcurrentSingleton<NHManager>.Instance.UpdateAsync(roleallianceTemp);
                    SetResponseData(() =>
                    {
                        SubDict.Add((byte)ParameterCode.RoleAlliance, Utility.Json.ToJson(roleallianceTemp));
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                    });
                }
                else
                {
                    SetResponseData(() =>
                    {
                        Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                    });
                }
                
            }
            else
            {
                SetResponseData(() =>
                {
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
        }
    }
}