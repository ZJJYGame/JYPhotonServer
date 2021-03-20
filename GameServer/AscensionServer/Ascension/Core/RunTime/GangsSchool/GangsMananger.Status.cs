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
            //获取所有的宗门列表
            var allianceList = RedisHelper.List.ListRangeAsync<int>(RedisKeyDefine._AlliancePerfix).Result;
            if (allianceList != null)
            {
                List<AllianceStatusDTO> AllianceList = new List<AllianceStatusDTO>();
                if (alliances.Index <= allianceList.Count)
                {
                    if (alliances.AllIndex <= allianceList.Count)
                    {
                        for (int i = alliances.Index; i < alliances.AllIndex; i++)
                        {
                            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, alliances.AllianceList[i].ToString()).Result;
                            if (result)
                            {
                                var alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, alliances.AllianceList[i].ToString()).Result;
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
                            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, alliances.AllianceList[i].ToString()).Result;
                            if (result)
                            {
                                var alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, alliances.AllianceList[i].ToString()).Result;
                                if (alliance != null)
                                {
                                    AllianceList.Add(alliance);
                                }
                            }
                        }
                    }
                }
                alliances.AllianceList = allianceList;
                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                dict.Add((byte)ParameterCode.Alliances, alliances);
                dict.Add((byte)ParameterCode.AllianceStatus, AllianceList);
                RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAlliances, dict);
            }else
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
                    await NHibernateQuerier.InsertAsync(allianceConstruction);
                    AllianceConstructionDTO constructionDTO = new AllianceConstructionDTO();
                    constructionDTO.AllianceID = statusDTO.ID;
                    await RedisHelper.Hash.HashSetAsync<AllianceConstructionDTO>(RedisKeyDefine._AllianceConstructionPerfix, statusDTO.ID.ToString(), constructionDTO);


                    AllianceMember alliance = new AllianceMember();
                    alliance.AllianceID = statusDTO.ID;
                    alliance.Member = Utility.Json.ToJson(new List<int>() { roleID });
                    await NHibernateQuerier.InsertAsync(alliance);
                    AllianceMemberDTO memberDTO = new AllianceMemberDTO();
                    memberDTO.AllianceID = statusDTO.ID;
                    memberDTO.Member = new List<int>() { roleID };
                    await RedisHelper.Hash.HashSetAsync<AllianceMemberDTO>(RedisKeyDefine._AllianceMemberPerfix, statusDTO.ID.ToString(), memberDTO);


                    roleAlliance.AllianceID = statusDTO.ID;
                    roleAlliance.AllianceJob = 1;//需要职位的表
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
                    dict.Add((byte)ParameterCode.AllianceConstruction, constructionDTO);
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.CreatAlliance, dict);
                }
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.CreatAlliance);
        }
        /// <summary>
        /// 获得宗门属性及建设
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="roleID"></param>
        async void GetAllianceConstructionS2C(int ID,int roleID)
        {
            var roleAllianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceConstructionPerfix, ID.ToString()).Result;
            var AllianceExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, ID.ToString()).Result;
            if (roleAllianceExist&& AllianceExist)
            {
                var Construction = RedisHelper.Hash.HashGetAsync<AllianceConstructionDTO>(RedisKeyDefine._AllianceConstructionPerfix, ID.ToString()).Result;
                var Alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, ID.ToString()).Result;
                if (Construction != null&& Alliance!=null)
                {
                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.AllianceStatus, Alliance);
                    dict.Add((byte)ParameterCode.AllianceConstruction, Construction);
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAllianceStatus, dict);
                }
                else
                    GetRoleAliianceConstructionMySql(ID,roleID);
            }
            else
                GetRoleAliianceConstructionMySql(ID,roleID);
        }
        /// <summary>
        /// 获得宗门通告
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="ID"></param>
        async void GetAllianceCallboardS2C(int roleID,int ID,DailyMessageDTO daily)
        {
            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._DailyMessagePerfix, ID.ToString()).Result;
            var dailyList = new List<DailyMessageDTO>();
            if (result)
            {
                var dailyObj= RedisHelper.Hash.HashGetAsync<List<DailyMessageDTO>>(RedisKeyDefine._DailyMessagePerfix, ID.ToString()).Result;
                if (dailyObj != null)
                {
                    if (dailyObj.Count> daily.CurrentIndex)
                    {
                        if ((dailyObj.Count >= daily.TargetIndex))
                        {
                            dailyList = dailyObj.GetRange(daily.CurrentIndex, daily.TargetIndex);
                        }
                        else
                        {
                            dailyList = dailyObj.GetRange(daily.CurrentIndex, dailyObj.Count);
                        }
                    }
                    RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAlliancecallboard, dailyList);
                }
                else
                    RoleStatusFailS2C(roleID, AllianceOpCode.GetAlliancecallboard);
            }
        }
        /// <summary>
        /// 宗門建設升級
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="constructionDTO"></param>
        async void BuildAlllianceS2C(int roleID,AllianceConstructionDTO constructionDTO)
        {
            var result = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AllianceConstructionPerfix, constructionDTO.AllianceID.ToString()).Result;
            if (result)
            {
                var construction= RedisHelper.Hash.HashGetAsync<AllianceConstructionDTO>(RedisKeyDefine._AllianceConstructionPerfix, constructionDTO.AllianceID.ToString()).Result;
                if (construction!=null)
                {

                }
            }

        }
        /// <summary>
        /// 修改宗門名稱
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="statusDTO"></param>
        async void ChangeAllianceNameS2C(int roleID,AllianceStatusDTO statusDTO)
        {
            var allianceExit = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString()).Result;
            if (allianceExit)
            {
                var alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString()).Result;
                if (alliance != null)
                {
                    alliance.AllianceName = statusDTO.AllianceName;
                    ///TODO發送給客戶端
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
        async void ChangeAlliancePurposeS2C(int roleID,AllianceStatusDTO statusDTO)
        {
            var allianceExit = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString()).Result;
            if (allianceExit)
            {
                var alliance = RedisHelper.Hash.HashGetAsync<AllianceStatusDTO>(RedisKeyDefine._AlliancePerfix, statusDTO.ID.ToString()).Result;
                if (alliance != null)
                {
                    alliance.Manifesto = statusDTO.Manifesto;
                    ///TODO發送給客戶端
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
            if (roleAlliance != null)
            {
                RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAllianceStatus, ChangeDataType(roleAlliance));
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.GetAllianceStatus);
        }
        /// <summary>
        /// 获取仙盟建筑信息
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="roleID"></param>
        void GetRoleAliianceConstructionMySql(int ID, int roleID)
        {
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("AllianceID", ID);
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", ID);
            var Construction = NHibernateQuerier.CriteriaSelect<AllianceConstruction>(nHCriteria);
            var Alliance = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteriaAlliance);
            if (Alliance!=null&& Alliance!=null)
            {
                Dictionary<byte, object> dict = new Dictionary<byte, object>();
                dict.Add((byte)ParameterCode.AllianceStatus, Alliance);
                dict.Add((byte)ParameterCode.AllianceConstruction, Construction);
                RoleStatusSuccessS2C(roleID, AllianceOpCode.GetAllianceStatus, dict);
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.GetAllianceStatus);
        }
        /// <summary>
        /// 修改宗門名稱
        /// </summary>
        void ChangeAllianceNameMySQL(int roleID, AllianceStatusDTO statusDTO)
        {
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", statusDTO.ID);
            var alliance = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteriaAlliance);
            if (alliance != null)
            {
                alliance.AllianceName = statusDTO.AllianceName;
                //TODO 發送至客戶端
            }
            else
                RoleStatusFailS2C(roleID,AllianceOpCode.ChangeAllianceName);

        }
        /// <summary>
        /// 修改宗門宗旨
        /// </summary>
        void ChangeAlliancePurposeMySql(int roleID, AllianceStatusDTO statusDTO)
        {
            NHCriteria nHCriteriaAlliance = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", statusDTO.ID);
            var alliance = NHibernateQuerier.CriteriaSelect<AllianceStatus>(nHCriteriaAlliance);
            if (alliance != null)
            {
                alliance.Manifesto = statusDTO.Manifesto;
                //TODO 發送至客戶端
            }
            else
                RoleStatusFailS2C(roleID, AllianceOpCode.ChangeAllianceName);
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
