using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;
using RedisDotNet;
using Cosmos;
using Protocol;
namespace AscensionServer
{
   public partial class SecondaryJobManager
    {
        public void OperateProcessing(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole)
        {
            switch (secondaryJobDTO.AlchemyDTO.JobOperate)
            {
                case SecondaryJobDTO.JobOperateType.Get:
                    GetRoleAlchemy(secondaryJobDTO, nHCriteriarole);
                    break;
                case SecondaryJobDTO.JobOperateType.Update:
                    UpdateRoleAlchemy(secondaryJobDTO, nHCriteriarole);
                    break;
                case SecondaryJobDTO.JobOperateType.Compound:
                    CompoundAlchemy(secondaryJobDTO, nHCriteriarole);
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 获得所有已学配方
        /// </summary>
        /// <param name="secondaryJobDTO"></param>
        /// <param name="nHCriteriarole"></param>
        public void GetRoleAlchemy(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole)
        {
            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlchemyPerfix, secondaryJobDTO.RoleID.ToString()).Result;
            if (result)
            {
                var alchemytemp= RedisHelper.Hash.HashGetAsync<AlchemyDTO>(RedisKeyDefine._AlchemyPerfix, secondaryJobDTO.RoleID.ToString()).Result;

                secondaryJobDTO.AlchemyDTO = alchemytemp;
                S2CAlchemyMessage(secondaryJobDTO.RoleID,Utility.Json.ToJson(secondaryJobDTO),ReturnCode.Success);
            }
            else
            {
                var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteriarole);

                if (alchemy != null)
                {
                    var alchemyObj = GameManager.ReferencePoolManager.Spawn<AlchemyDTO>();
                    alchemyObj.RoleID = alchemy.RoleID;
                    alchemyObj.JobLevelExp = alchemy.JobLevelExp;
                    alchemyObj.Recipe_Array = Utility.Json.ToObject<HashSet<int>>(alchemy.Recipe_Array);
                    alchemyObj. JobLevel= alchemy.JobLevel;

                    secondaryJobDTO.AlchemyDTO = alchemyObj;
                    S2CAlchemyMessage(secondaryJobDTO.RoleID, Utility.Json.ToJson(secondaryJobDTO), ReturnCode.Success);
                }
                else
                {
                    S2CAlchemyMessage(secondaryJobDTO.RoleID, "数据库没找到", ReturnCode.Fail);
                }
            }


        }
        /// <summary>
        /// 学习新配方
        /// </summary>
        /// <param name="secondaryJobDTO"></param>
        /// <param name="nHCriteriarole"></param>
        public async void UpdateRoleAlchemy(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole)
        {
            var ringServer = NHibernateQuerier.CriteriaSelect<RoleRing>(nHCriteriarole);
            var nHCriteriaRingID = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ringServer.RingIdArray);

            if (InventoryManager.VerifyIsExist(secondaryJobDTO.UseItemID, nHCriteriaRingID))
            {
                var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteriarole);
                if (alchemy!=null)
                {
                    var recipe = Utility.Json.ToObject<List<int>>(alchemy.Recipe_Array);
                    if (recipe.Contains(secondaryJobDTO.UseItemID))
                    {
                        recipe.Add(secondaryJobDTO.UseItemID);
                        alchemy.Recipe_Array = Utility.Json.ToJson(recipe);
                        await NHibernateQuerier.UpdateAsync(alchemy);

                        var alchemytemp = GameManager.ReferencePoolManager.Spawn<AlchemyDTO>();
                        alchemytemp.RoleID = alchemy.RoleID;
                        alchemytemp.JobLevel = alchemy.JobLevel;
                        alchemytemp.JobLevelExp = alchemy.JobLevelExp;
                        alchemytemp.Recipe_Array = Utility.Json.ToObject<HashSet<int>>(alchemy.Recipe_Array);
                        await RedisHelper.Hash.HashSetAsync<AlchemyDTO>(RedisKeyDefine._AlchemyPostfix, secondaryJobDTO.RoleID.ToString(), alchemytemp);

                        S2CAlchemyMessage(secondaryJobDTO.RoleID,Utility.Json.ToJson(alchemytemp),ReturnCode.Success);
                        GameManager.ReferencePoolManager.Despawns(alchemytemp);
                    }
                    else
                    { S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail); }
                }
                else
                { S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail); }
            }
            else
            { S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail); }
        }

        public void CompoundAlchemy(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole)
        {
            var alchemy = NHibernateQuerier.CriteriaSelect<Alchemy>(nHCriteriarole);
            if (alchemy!=null)
            {
                GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, FormulaData>>(out var formulaDataDict);
                var recipe = Utility.Json.ToObject<List<int>>(alchemy.Recipe_Array);
                if (recipe.Contains(secondaryJobDTO.UseItemID))
                {
                    formulaDataDict.TryGetValue(secondaryJobDTO.UseItemID, out var formulaData);

                }
                else
                {
                    S2CAlchemyMessage(secondaryJobDTO.RoleID,null,ReturnCode.Fail);
                }
            }
            else
            {
                S2CAlchemyMessage(secondaryJobDTO.RoleID, null, ReturnCode.Fail);
            }

        }

        public void S2CAlchemyMessage(int roleid,string s2cMessage, ReturnCode returnCode)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = s2cMessage;
            opData.OperationCode = (byte)OperationCode.SyncSecondaryJob;
            opData.ReturnCode = (byte)returnCode;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
        }

    }
}
