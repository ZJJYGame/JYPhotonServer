using System;
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
   public class GetAlchemySubHandler:SyncAlchemySubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            var dict = ParseSubDict(operationRequest);
            string alchemyJson = Convert.ToString(Utility.GetValue(dict,(byte)ParameterCode.JobAlchemy));
            var alchemyObj = Utility.Json.ToObject<AlchemyDTO>(alchemyJson);
            NHCriteria nHCriteriaalchemy = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", alchemyObj.RoleID);
            var alchemytemp = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteriaalchemy);
            if (alchemytemp!=null)
            {
                if (!string.IsNullOrEmpty(alchemytemp.Recipe_Array))
                {
                    SetResponseData(() =>
                    {
                        alchemyObj.JobLevel = alchemytemp.JobLevel;
                        alchemyObj.JobLevelExp = alchemytemp.JobLevelExp;
                        alchemyObj.Recipe_Array = Utility.Json.ToObject<HashSet<int>>(alchemytemp.Recipe_Array);
                        alchemyObj.RoleID = alchemytemp.RoleID;
     
                        SubDict.Add((byte)ParameterCode.JobAlchemy, Utility.Json.ToJson(alchemyObj));
                        Owner.OpResponseData.ReturnCode = (short)ReturnCode.Success;
                    });
                }
            }
            else
            {
                Owner.OpResponseData.ReturnCode = (short)ReturnCode.Fail;
                SubDict.Add((byte)ParameterCode.JobAlchemy, Utility.Json.ToJson(new List<string>()));
            }
            peer.SendOperationResponse(Owner.OpResponseData, sendParameters);
            GameManager.ReferencePoolManager.Despawns(nHCriteriaalchemy);
        }

       
    }
}
