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
    public class VerifyTreasureAtticHandler : SyncTreasureatticSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Verify;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string schoolJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.TreasureAttic));
            var schoolObj = Utility.Json.ToObject<School>(schoolJson);
            NHCriteria nHCriteriaSchool = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("ID", schoolObj.ID);
            var schoolTemp = Singleton<NHManager>.Instance.CriteriaSelect<School>(nHCriteriaSchool);
            if (schoolTemp!=null)
            {
                AscensionServer._Log.Info(">>>>>>>>>>>>>>>>>>>数据库贡献点" + schoolTemp.Contribution+"传过来的贡献点" + schoolObj.Contribution);
                if (schoolTemp.Contribution> schoolObj.Contribution)
                {
                    SetResponseData(() =>
                    {

                        SubDict.Add((byte)ParameterCode.TreasureAttic, Utility.Json.ToJson(true));
                        Owner.OpResponse.ReturnCode = (byte)ReturnCode.Success;
                    });
                }
                else
                {
                    SubDict.Add((byte)ParameterCode.TreasureAttic, Utility.Json.ToJson(false));
                    Owner.OpResponse.ReturnCode = (byte)ReturnCode.Fail;
                }
            }

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawns(nHCriteriaSchool);
        }
    }
}
