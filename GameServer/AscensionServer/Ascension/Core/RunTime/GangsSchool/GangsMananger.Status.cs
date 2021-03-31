using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedisDotNet;
using AscensionProtocol.DTO;
using AscensionProtocol;
using Cosmos;
using AscensionServer.Model;

namespace AscensionServer
{
    public partial class GangsMananger
    {
        #region Redis模块
        async void GetRoleAllianceS2C(int roleID )
        {
            var roleAllianceExist =  RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RoleAlliancePerfix,roleID.ToString()).Result;
            if (roleAllianceExist)
            {
                var roleAlliance = RedisHelper.Hash.HashGetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleID.ToString()).Result;
                if (roleAlliance != null)
                {
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.GetRoleAlliance, roleAlliance);
                }
                else
                    GetRoleAliianceMySql(roleID);
            } else
                GetRoleAliianceMySql(roleID);
        }
        /// <summary>
        /// 获得现有宗门,按照
        /// </summary>
        async void GetAllAllianceS2C(int roleID,AlliancesDTO alliances)
        {
            var alliancesExist = RedisHelper.String.StringGetAsync(RedisKeyDefine._AllianceListPerfix).Result;
            if (alliancesExist != null)
            {
                var allianceList = Utility.Json.ToObject<List<int>>(alliancesExist);
                List<AllianceStatusDTO> AllianceList = new List<AllianceStatusDTO>();
                if (alliances.Index <= allianceList.Count)
                {
                    if (alliances.AllIndex <= allianceList.Count)
                    {
                        for (int i = alliances.Index; i < alliances.AllIndex; i++)
                        {
                            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, allianceList[i].ToString()).Result;
                            if (result)
                            {
                                var alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, allianceList[i].ToString()).Result;
                                if (alliance != null)
                                {
                                    AllianceList.Add(alliance);
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = alliances.Index; i < allianceList.Count; i++)
                        {
                            Utility.Debug.LogInfo("YZQ获取的Redis宗门的列表的元素" + allianceList[i]);
                            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, allianceList[i].ToString()).Result;
                            if (result)
                            {
                                var alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, allianceList[i].ToString()).Result;
                                if (alliance != null)
                                {
                                    AllianceList.Add(alliance);
                                }
                            }
                        }
                        alliances.ISRefresh = false;
                    }
                }
                else
                {
                    alliances.ISRefresh = false;
                }
                alliances.AllianceList = allianceList;
                Utility.Debug.LogInfo("YZQ获取的Redis宗门的列表的元素" + Utility.Json.ToJson(AllianceList));
                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                dict.Add((byte)ParameterCode.Alliances, alliances);
                dict.Add((byte)ParameterCode.AllianceStatus, AllianceList);
                RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAlliances, dict);
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.GetAlliances);
        }
        /// <summary>
        /// 创建宗门
        /// </summary>
        /// <param name="statusDTO"></param>
        async void CreatAllianceS2C(int roleID,AllianceStatus statusDTO)
        {
            GameEntry. DataManager.TryGetValue<Dictionary<int, CreatAllianceData>>(out var CreatAlliance);

            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", 1);
            var roleAssets = NHibernateQuerier.CriteriaSelectAsync<RoleAssets>(nHCriteriaRole).Result;

            var role = NHibernateQuerier.CriteriaSelectAsync<Role>(nHCriteriaRole).Result;

            var roleAlliance = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriaRole).Result;

            var AllAlliance = NHibernateQuerier.CriteriaSelectAsync<Alliances>(nHCriteria).Result;

            if (roleAssets != null && role != null && roleAlliance != null)
            {
                //TODO判断灵石够不够
                if (roleAssets.SpiritStonesLow >= CreatAlliance[1].ScripturesPlatform && role.RoleLevel >= CreatAlliance[1].RoleLevel)
                {
                    statusDTO.AllianceLevel = 1;
                    statusDTO.AllianceNumberPeople = 1;
                    statusDTO.AlliancePeopleMax = 30;
                    statusDTO.Popularity = CreatAlliance[1].Popularity;
                    statusDTO.AllianceMaster = role.RoleName;
                    statusDTO.AllianceNumberPeople = 1;
                    statusDTO.OnLineNum = 1;
                    //TODO判断名字是否重复
                    statusDTO = await NHibernateQuerier.InsertAsync(statusDTO);
                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString(), statusDTO);
                    //初始化宗门数据,宗门建设,
                    if (AllAlliance!=null)
                    {
                        var alliances = Utility.Json.ToObject<List<int>>(AllAlliance.AllianceList);
                        alliances.Add(statusDTO.ID);
                        AllAlliance.AllianceList = Utility.Json.ToJson(alliances);
                        await NHibernateQuerier.UpdateAsync(AllAlliance);
                       await  RedisHelper.String.StringSetAsync(RedisKeyDefine._AllianceListPerfix, alliances);
                    }

                    AllianceConstruction allianceConstruction = new AllianceConstruction();
                    allianceConstruction.AllianceID = statusDTO.ID;
                    allianceConstruction.AllianceAssets = CreatAlliance[1].SpiritStones;
                    await NHibernateQuerier.InsertAsync(allianceConstruction);

                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._AllianceConstructionPerfix, statusDTO.ID.ToString(), allianceConstruction);

                    AllianceExchangeGoods exchangeGoods = new AllianceExchangeGoods();
                    exchangeGoods.AllianceID = statusDTO.ID; ;
                    await NHibernateQuerier.InsertAsync(exchangeGoods);
                    AllianceExchangeGoodsDTO allianceExchange = new AllianceExchangeGoodsDTO();
                    allianceExchange.AllianceID = statusDTO.ID;
                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._AllianceExchangeGoodsPerfix, statusDTO.ID.ToString(), allianceExchange);

                    AllianceMember alliance = new AllianceMember();
                    alliance.AllianceID = statusDTO.ID;
                    alliance.Member = Utility.Json.ToJson(new List<int>() { roleID });
                    await NHibernateQuerier.InsertAsync(alliance);
                    AllianceMemberDTO memberDTO = new AllianceMemberDTO();
                    memberDTO.AllianceID = statusDTO.ID;
                    memberDTO.Member = new List<int>() { roleID };
                    await RedisHelper.Hash.HashSetAsync<AllianceMemberDTO>(RedisKeyDefine._AllianceMemberPerfix, statusDTO.ID.ToString(), memberDTO);


                    roleAlliance.AllianceID = statusDTO.ID;
                    roleAlliance.AllianceJob = 937;//需要职位的表
                    roleAlliance.JoinOffline = DateTime.Now.ToString();
                    roleAlliance.Reputation = 0;
                    roleAlliance.ReputationHistroy = 0;
                    roleAlliance.ReputationMonth = 0;
                    await RedisHelper.Hash.HashSetAsync<RoleAllianceDTO>(RedisKeyDefine._RoleAlliancePerfix, roleID.ToString(), ChangeDataType(roleAlliance));
                    await NHibernateQuerier.UpdateAsync(roleAlliance);

                    //宗门建设信息，个人宗门信息
                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.RoleAlliance, ChangeDataType (roleAlliance));
                    dict.Add((byte)ParameterCode.AllianceStatus, statusDTO);
                    dict.Add((byte)ParameterCode.AllianceConstruction, allianceConstruction);
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.CreatAlliance, dict);
                }
                else
                    RoleStatusFailS2C(roleID, AllianceOpCode.CreatAlliance);
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.CreatAlliance);
        }    
        /// <summary>
        /// 获得宗门通告
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="ID"></param>
        async void GetAllianceCallboardS2C(int roleID,int ID)
        {
            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._DailyMessagePerfix, ID.ToString()).Result;
            if (result)
            {
                var dailyObj= RedisHelper.Hash.HashGetAsync<List<DailyMessageDTO>>(RedisKeyDefine._DailyMessagePerfix, ID.ToString()).Result;
                Utility.Debug.LogInfo("YZQ>>>>>>>>>>>>"+Utility.Json.ToJson(dailyObj));
                if (dailyObj != null)
                {
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAlliancecallboard, dailyObj);
                }
                else
                    RoleStatusFailS2C(roleID, AllianceOpCode.GetAlliancecallboard);
            }
        }

        /// <summary>
        /// 修改宗門名稱
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="statusDTO"></param>
        async void ChangeAllianceNameS2C(int roleID,AllianceStatus statusDTO)
        {
            var allianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString()).Result;
            var constructionExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceConstructionPerfix, statusDTO.ID.ToString()).Result;
            if (allianceExist && constructionExist)
            {
                var alliance = RedisHelper.Hash.HashGetAsync<AllianceStatus>(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString()).Result;
                var construction = RedisHelper.Hash.HashGetAsync<AllianceConstruction>(RedisKeyDefine._AllianceConstructionPerfix, statusDTO.ID.ToString()).Result;
                if (alliance != null && construction!=null)
                {
                    if (construction.AllianceAssets>=500000)
                    {
                        alliance.AllianceName = statusDTO.AllianceName;
                        construction.AllianceAssets -= 500000;
                    }
                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.AllianceConstruction, construction);
                    dict.Add((byte)ParameterCode.AllianceStatus, alliance);
                    RoleStatusSuccessS2C(roleID,AllianceOpCode.ChangeAllianceName, dict);

                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._AllianceConstructionPerfix, statusDTO.ID.ToString(), construction);
                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString(), alliance);
                   await NHibernateQuerier.UpdateAsync(alliance);
                   await NHibernateQuerier.UpdateAsync(construction);
                }
                else
                {
                    ChangeAllianceNameMySQL(roleID, statusDTO);
                }
            }
            else
            {
                ChangeAllianceNameMySQL(roleID, statusDTO);
            }
        }

        /// <summary>
        /// 修改宗門宗旨
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="statusDTO"></param>
        async void ChangeAlliancePurposeS2C(int roleID,AllianceStatus statusDTO)
        {
            var allianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString()).Result;
   
            if (allianceExist)
            {
                var alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString()).Result;
             
                if (alliance != null)
                {
                    alliance.Manifesto = statusDTO.Manifesto;
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.ChangeAlliancePurpose, alliance);

                    await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString(), alliance);
                    await NHibernateQuerier.UpdateAsync(alliance);
                }
                else
                {
                    ChangeAllianceNameMySQL(roleID, statusDTO);
                }
            }
            else
            {
                ChangeAllianceNameMySQL(roleID, statusDTO);
            }
        }

        #endregion
        #region MySql模块
        /// <summary>
        /// 获取角色自身宗门数据
        /// </summary>
        /// <param name="roleID"></param>
        void GetRoleAliianceMySql(int roleID)
        {
            NHCriteria nHCriteriaRole = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var roleAlliance = NHibernateQuerier.CriteriaSelect<RoleAlliance>(nHCriteriaRole);
            Utility.Debug.LogInfo("获得角色宗門数据2" +Utility.Json.ToJson(roleAlliance));
            if (roleAlliance != null)
            {
                RoleStatusSuccessS2C(roleID, AllianceOpCode.GetRoleAlliance, ChangeDataType(roleAlliance));
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.GetRoleAlliance);
        }
        /// <summary>
        /// 修改宗門名稱
        /// </summary>
       async void ChangeAllianceNameMySQL(int roleID, AllianceStatus statusDTO)
        {
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", statusDTO.ID);
            var alliance = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteriaAlliance);
            NHCriteria nHCriteriaconstruction = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", statusDTO.ID);
            var construction = NHibernateQuerier.CriteriaSelect<AllianceConstruction>(nHCriteriaconstruction);
            if (alliance != null&& construction!=null)
            {
                if (construction.AllianceAssets>=500000)
                {
                    alliance.AllianceName = statusDTO.AllianceName;
                    construction.AllianceAssets -= 500000;
                }
                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                dict.Add((byte)ParameterCode.AllianceConstruction, construction);
                dict.Add((byte)ParameterCode.AllianceStatus, alliance);
                RoleStatusSuccessS2C(roleID, AllianceOpCode.ChangeAllianceName, dict);

                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._AllianceConstructionPerfix, statusDTO.ID.ToString(), construction);
                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString(), alliance);
                await NHibernateQuerier.UpdateAsync(alliance);
                await NHibernateQuerier.UpdateAsync(construction);
            }
            else
                RoleStatusFailS2C(roleID,AllianceOpCode.ChangeAllianceName);

        }
        /// <summary>
        /// 修改宗門宗旨
        /// </summary>
        async void ChangeAlliancePurposeMySql(int roleID, AllianceStatus statusDTO)
        {
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", statusDTO.ID);
            var alliance = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteriaAlliance);
            if (alliance != null)
            {
                if (!string.IsNullOrEmpty(statusDTO.Manifesto))
                {
                    alliance.Manifesto = statusDTO.Manifesto;
                }
                RoleStatusSuccessS2C(roleID, AllianceOpCode.ChangeAlliancePurpose, alliance);

                await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString(), alliance);
                await NHibernateQuerier.UpdateAsync(alliance);
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.ChangeAlliancePurpose);
        }
        #endregion


        RoleAllianceDTO ChangeDataType(RoleAlliance roleAlliance)
        {
            RoleAllianceDTO roleAllianceDTO = new RoleAllianceDTO();
            roleAllianceDTO.AllianceID = roleAlliance.AllianceID;
            roleAllianceDTO.AllianceJob = roleAlliance.AllianceJob;
            roleAllianceDTO.ApplyForAlliance = Utility.Json.ToObject<List<int>>(roleAlliance.ApplyForAlliance);
            roleAllianceDTO.JoinOffline = roleAlliance.JoinOffline;
            roleAllianceDTO.JoinTime = roleAlliance.JoinTime;
            roleAllianceDTO.Reputation = roleAlliance.Reputation;
            roleAllianceDTO.ReputationHistroy = roleAlliance.ReputationHistroy;
            roleAllianceDTO.ReputationMonth = roleAlliance.ReputationMonth;
            roleAllianceDTO.RoleID = roleAlliance.RoleID;
            roleAllianceDTO.RoleName = roleAlliance.RoleName;
            return roleAllianceDTO;
        }

        RoleAlliance ChangeDataType(RoleAllianceDTO AllianceDTO)
        {
            RoleAlliance roleAlliance = new RoleAlliance();
            roleAlliance.AllianceID = AllianceDTO.AllianceID;
            roleAlliance.AllianceJob = AllianceDTO.AllianceJob;
            roleAlliance.ApplyForAlliance = Utility.Json.ToJson(AllianceDTO.ApplyForAlliance);
            roleAlliance.JoinOffline = AllianceDTO.JoinOffline;
            roleAlliance.JoinTime = AllianceDTO.JoinTime;
            roleAlliance.Reputation = AllianceDTO.Reputation;
            roleAlliance.ReputationHistroy = AllianceDTO.ReputationHistroy;
            roleAlliance.ReputationMonth = AllianceDTO.ReputationMonth;
            roleAlliance.RoleID = AllianceDTO.RoleID;
            roleAlliance.RoleName = AllianceDTO.RoleName;
            return roleAlliance;
        }
    }
}
