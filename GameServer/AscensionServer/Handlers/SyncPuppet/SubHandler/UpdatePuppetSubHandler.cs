﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;
using AscensionProtocol.DTO;
namespace AscensionServer
{
    public class UpdatePuppetSubHandler : SyncPuppetSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string puppetJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobPuppet));
            var puppetObj = Utility.Json.ToObject<PuppetDTO>(puppetJson);
            NHCriteria nHCriteriapuppet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", puppetObj.RoleID);
            var puppetTemp =NHibernateQuerier.CriteriaSelect<Puppet>(nHCriteriapuppet);
            int Level = 0;
            int Exp = 0;
            //AscensionServer._Log.Info("传输回去的锻造数据" + forgeJson);
            if (puppetTemp != null)
            {
                if (puppetObj.JobLevel != 0)
                {
                    Level = puppetTemp.JobLevel + puppetObj.JobLevel;
                    puppetObj = new PuppetDTO() { RoleID = puppetTemp.RoleID, JobLevel = Level, JobLevelExp = puppetObj.JobLevelExp, Recipe_Array = Utility.Json.ToObject<HashSet<int>>(puppetTemp.Recipe_Array)  };
                    NHibernateQuerier.Update(puppetObj);

                }
                else
                {
                    Exp = puppetTemp.JobLevelExp + puppetObj.JobLevelExp;
                    puppetObj = new PuppetDTO() { RoleID = puppetTemp.RoleID, JobLevel = puppetTemp.JobLevel, JobLevelExp = Exp, Recipe_Array = Utility.Json.ToObject<HashSet<int>>(puppetTemp.Recipe_Array) };
                    NHibernateQuerier.Update(puppetObj);

                }
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.JobPuppet, Utility.Json.ToJson(puppetObj));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriapuppet);
        }
    }
}
