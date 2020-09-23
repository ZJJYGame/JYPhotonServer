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
using RedisDotNet;

namespace AscensionServer
{
    public class UpdateRolePetSubHandler: SyncRolePetSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Update;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = ParseSubParameters(operationRequest);
            string rolepet = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.RolePet));

            var rolepetObj = Utility.Json.ToObject<RolePet>(rolepet);
            NHCriteria nHCriteriaRolePet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", rolepetObj.RoleID);

            var rolepets = NHibernateQuerier.CriteriaSelectAsync<RolePet>(nHCriteriaRolePet).Result;

            if (RedisHelper.Hash.HashExistAsync("RolePet", rolepetObj.RoleID.ToString()).Result&& rolepets != null)
            {
                #region Redis模块
                rolepets.PetIsBattle = rolepetObj.PetIsBattle;
               RedisHelper.Hash.HashSet<RolePet>("RolePet", rolepets.RoleID.ToString(),rolepets);

               NHibernateQuerier.Update(rolepets);
                #endregion

                SetResponseParamters(() =>
                {
                    subResponseParameters.Add((byte)ParameterCode.RolePet, Utility.Json.ToJson(rolepets));
                    operationResponse.ReturnCode = (short)ReturnCode.Success;
                });
            }
            else
            {
                SetResponseParamters(() =>
                {
                    operationResponse.ReturnCode = (short)ReturnCode.Fail;
                });
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriaRolePet);
            return operationResponse;
        }
    }
}
