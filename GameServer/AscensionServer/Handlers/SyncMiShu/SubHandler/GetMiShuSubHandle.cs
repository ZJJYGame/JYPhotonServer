using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class GetMiShuSubHandle : SyncMiShuSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = ParseSubParameters(operationRequest);
            string roleMSJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.MiShu));
            var roleMiShuObj = Utility.Json.ToObject<RoleMiShu>(roleMSJson);
            NHCriteria nHCriteriamishu = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleMiShuObj.RoleID);
            RoleMiShu roleMiShu = NHibernateQuerier.CriteriaSelect<RoleMiShu>(nHCriteriamishu);
            Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>收到获取秘术的数组" + roleMiShu.MiShuIDArray);
            if ( !string.IsNullOrEmpty(roleMiShu.MiShuIDArray))
            {
                var rMiShuObj = NHibernateQuerier.CriteriaSelect<RoleMiShu>(nHCriteriamishu);
                string rolemishuJson = rMiShuObj.MiShuIDArray;
                Dictionary<int, int> roleIDict;
                List<MiShu> miShuIdList = new List<MiShu>();
                List<NHCriteria> nHCriteriaslist = new List<NHCriteria>();
                if (!string.IsNullOrEmpty(rolemishuJson))
                {
                    roleIDict = new Dictionary<int, int>();
                    roleIDict = Utility.Json.ToObject<Dictionary<int, int>>(rolemishuJson);
                    foreach (var roleid in roleIDict)
                    {
                        NHCriteria tmpcriteria = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", roleid.Key);
                        MiShu miShu = NHibernateQuerier.CriteriaSelect<MiShu>(tmpcriteria);
                        miShuIdList.Add(miShu);
                        nHCriteriaslist.Add(tmpcriteria);
                    }
                }
                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.MiShu, Utility.Json.ToJson(miShuIdList));
                    operationResponse.ReturnCode = (byte)ReturnCode.Success;
                });
                GameManager.ReferencePoolManager.Despawns(nHCriteriaslist);
            }
            else
            {
              SetResponseParamters(() =>
                {
                    Utility.Debug.LogInfo(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>收到获取秘术的id为空");
                    subResponseParameters.Add((byte)ParameterCode.MiShu, Utility.Json.ToJson(new List<string>()));
                    operationResponse.ReturnCode = (byte)ReturnCode.Fail;
                });
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriamishu);
            return operationResponse;
        }
    }
}