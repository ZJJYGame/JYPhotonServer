﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using AscensionServer.Model;
using Cosmos;

namespace AscensionServer
{
    public class UpdateSpiritualRunesSubHandler : SyncSpiritualRuneSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
            base.OnInitialization();
        }

        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string spiritualrunesJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.JobSpiritualRunes));
            var spiritualrunesObj = Utility.Json.ToObject<SpiritualRunesDTO>(spiritualrunesJson);
            NHCriteria nHCriteriaspiritualrunes = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", spiritualrunesObj.RoleID);
            var spiritualrunesTemp = NHibernateQuerier.CriteriaSelect<SpiritualRunes>(nHCriteriaspiritualrunes);
            int Level = 0;
            int Exp = 0;
            Utility.Debug.LogInfo("传输回去的制符数据" + spiritualrunesObj);
            if (spiritualrunesTemp != null)
            {
                if (spiritualrunesObj.JobLevel != 0)
                {
                    Level = spiritualrunesTemp.JobLevel + spiritualrunesObj.JobLevel;
                    spiritualrunesObj = new SpiritualRunesDTO() { RoleID = spiritualrunesTemp.RoleID, JobLevel = Level, JobLevelExp = spiritualrunesObj.JobLevelExp, Recipe_Array = Utility.Json.ToObject < HashSet<int> > (spiritualrunesTemp.Recipe_Array) };
                    NHibernateQuerier.Update(spiritualrunesObj);
                }
                else
                {
                    Exp = spiritualrunesTemp.JobLevelExp + spiritualrunesObj.JobLevelExp;
                    spiritualrunesObj =  new SpiritualRunesDTO() { RoleID = spiritualrunesTemp.RoleID, JobLevel = spiritualrunesTemp.JobLevel, JobLevelExp = Exp, Recipe_Array = Utility.Json.ToObject<HashSet<int>>(spiritualrunesTemp.Recipe_Array) };
                    NHibernateQuerier.Update(spiritualrunesObj);

                }
                SetResponseData(() =>
                {
                    SubDict.Add((byte)ParameterCode.JobSpiritualRunes, Utility.Json.ToJson(spiritualrunesObj));
                    Owner.OpResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
                Owner.OpResponse.ReturnCode = (short)ReturnCode.Fail;

            peer.SendOperationResponse(Owner.OpResponse, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaspiritualrunes);
        }
    }
}
