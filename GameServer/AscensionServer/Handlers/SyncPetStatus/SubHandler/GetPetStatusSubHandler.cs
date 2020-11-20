using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Photon.SocketServer;
using RedisDotNet;
using AscensionServer.Model;
using Cosmos;
namespace AscensionServer
{
    public class GetPetStatusSubHandler : SyncPetStatusSubHandler
    {
        public override byte SubOpCode { get; protected set; } = (byte)SubOperationCode.Get;

        public override OperationResponse EncodeMessage(OperationRequest operationRequest)
        {
            var dict = operationRequest.Parameters;
            string petCompleteJson = Convert.ToString(Utility.GetValue(dict, (byte)ParameterCode.PetStatus));
            var petCompleteObj = Utility.Json.ToObject<PetCompleteDTO>(petCompleteJson);
            NHCriteria nHCriteriarolepet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", petCompleteObj.PetDTO.ID);
            //var petArray = NHibernateQuerier.CriteriaSelect<RolePet>(nHCriteriarolepet);
            switch (petCompleteObj.PetOrderType)
            {
                case PetCompleteDTO.PetOperationalOrder.None:
                    break;
                case PetCompleteDTO.PetOperationalOrder.PetLevelUP:
                    break;
                case PetCompleteDTO.PetOperationalOrder.PetResetAbilitySln:
                    break;
                case PetCompleteDTO.PetOperationalOrder.PetResetStatus:
                    break;
                case PetCompleteDTO.PetOperationalOrder.PetEvolution:
                    break;
                case PetCompleteDTO.PetOperationalOrder.PetStudtSkill:
                    break;
                case PetCompleteDTO.PetOperationalOrder.PetCultivate:
                    break;
                case PetCompleteDTO.PetOperationalOrder.PetGetStatus:

                    break;
                default:
                    break;
            }
            #region 待删
            //if (petArray != null)
            //{
            //    string petdict = petArray.PetIDDict;
            //    Dictionary<int, int> petIDList;
            //    List<PetStatus> petList = new List<PetStatus>();
            //    List<PetaPtitudeDTO> petaptitudeList = new List<PetaPtitudeDTO>();
            //    List<NHCriteria> nHCriteriasList = new List<NHCriteria>();
            //    if (petdict != null)
            //    {
            //        petIDList = new Dictionary<int, int>();
            //        petIDList = Utility.Json.ToObject<Dictionary<int, int>>(petdict);
            //        foreach (var petid in petIDList)
            //        {
            //            NHCriteria nHCriteriapet = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("PetID", petid.Key);
            //            var petstatusObj = NHibernateQuerier.CriteriaSelect<PetStatus>(nHCriteriapet);
            //            var petaptitudeObj = NHibernateQuerier.CriteriaSelect<PetaPtitude>(nHCriteriapet);
            //            PetaPtitudeDTO petaPtitudeDTO = new PetaPtitudeDTO() { AttackphysicalAptitude = petaptitudeObj.AttackphysicalAptitude, AttackspeedAptitude = petaptitudeObj.AttackspeedAptitude, AttackpowerAptitude = petaptitudeObj.AttackpowerAptitude, DefendphysicalAptitude = petaptitudeObj.DefendphysicalAptitude, DefendpowerAptitude = petaptitudeObj.DefendpowerAptitude, HPAptitude = petaptitudeObj.HPAptitude, Petaptitudecol = petaptitudeObj.Petaptitudecol, PetaptitudeDrug = Utility.Json.ToObject<Dictionary<int, int>>(petaptitudeObj.PetaptitudeDrug), PetID = petaptitudeObj.PetID, SoulAptitude = petaptitudeObj.SoulAptitude };
            //            petaptitudeList.Add(petaPtitudeDTO);
            //            petList.Add(petstatusObj);
            //            nHCriteriasList.Add(nHCriteriapet);
            //        }
            //    }
            //    SetResponseParamters(() =>
            //    {
            //        subResponseParameters.Add((byte)ParameterCode.PetStatus, Utility.Json.ToJson(petList));
            //        subResponseParameters.Add((byte)ParameterCode.PetPtitude, Utility.Json.ToJson(petaptitudeList));
            //        operationResponse.ReturnCode = (short)ReturnCode.Success;
            //    });
            //    GameManager.ReferencePoolManager.Despawns(nHCriteriasList);
            //}
            //else
            //{
            //    SetResponseParamters(() =>
            //    {
            //        Utility.Debug.LogInfo(">>>>>>>>>>>>>》》》》》》》》》》》》>>获得宠物数据失败");
            //        subResponseParameters.Add((byte)ParameterCode.PetStatus, Utility.Json.ToJson(new List<string>()));
            //        operationResponse.ReturnCode = (short)ReturnCode.Fail;
            //    });
            //}
            #endregion]
            GameManager.ReferencePoolManager.Despawns(nHCriteriarolepet);
            return operationResponse;
        }
    }
}
