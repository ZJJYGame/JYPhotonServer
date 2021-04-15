using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using Cosmos;
using RedisDotNet;
namespace AscensionServer
{
   public partial class PuppetManager
    {
        #region Redis模块
        void GetPuppetStatusS2C(int roleid)
        {
            var rolepuppetExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RolePuppetPerfix, roleid.ToString()).Result;
            Dictionary<int, PuppetIndividualDTO> puppetDict = new Dictionary<int, PuppetIndividualDTO>();
            if (rolepuppetExist)
            {
                var rolepuppet = RedisHelper.Hash.HashGetAsync<RolePuppetDTO>(RedisKeyDefine._RolePuppetPerfix, roleid.ToString()).Result;
                if (rolepuppet != null)
                {
                    foreach (var item in rolepuppet.PuppetDict)
                    {
                        var puppetExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._PuppetIndividualPerfix, item.Key.ToString()).Result;
                        if (puppetExist)
                        {
                            var puppetObj = RedisHelper.Hash.HashGetAsync<PuppetIndividualDTO>(RedisKeyDefine._PuppetIndividualPerfix, item.Key.ToString()).Result;
                            if (puppetObj != null)
                            {
                                puppetDict.Add(item.Key, puppetObj);
                            }
                            else
                                GetPuppetStatusMySql(roleid);
                        }
                        else
                            GetPuppetStatusMySql(roleid);
                    }

                    Dictionary<byte, object> dict = new Dictionary<byte, object>();
                    dict.Add((byte)ParameterCode.RolePuppet, rolepuppet);
                    dict.Add((byte)ParameterCode.GetPuppetIndividual, puppetDict);
                    PuppetManagerSuccessS2C(roleid,PuppetOpCode.GetPuppetStatus,dict);
                }
                else
                    GetPuppetStatusMySql(roleid);
            }
            else
                GetPuppetStatusMySql(roleid);
        }

       async void SetBattleS2C(int roleid,int id)
        {
            var rolepuppetExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RolePuppetPerfix, roleid.ToString()).Result;
            Dictionary<int, PuppetIndividualDTO> puppetDict = new Dictionary<int, PuppetIndividualDTO>();
            if (rolepuppetExist)
            {
                var rolepuppet = RedisHelper.Hash.HashGetAsync<RolePuppetDTO>(RedisKeyDefine._RolePuppetPerfix, roleid.ToString()).Result;
                if (rolepuppet != null)
                {
                    rolepuppet.IsBattle = id;
                    PuppetManagerSuccessS2C(roleid,PuppetOpCode.SetBattle,rolepuppet);
                   await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RolePuppetPerfix, roleid.ToString(), rolepuppet);
                   await NHibernateQuerier.UpdateAsync(ChangeDataType(rolepuppet));
                }
            }
        }

        async void AbandonPuppetS2C(int roleid,int id)
        {
            var rolepuppetExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._RolePuppetPerfix, roleid.ToString()).Result;
            var puppetIndividualExist = RedisHelper.Hash.HashExistAsync(RedisKeyDefine._PuppetIndividualPerfix, id.ToString()).Result;
            if (rolepuppetExist&&puppetIndividualExist)
            {
                var rolepuppet = RedisHelper.Hash.HashGetAsync<RolePuppetDTO>(RedisKeyDefine._RolePuppetPerfix, roleid.ToString()).Result;
                var puppetIndividual = RedisHelper.Hash.HashGetAsync<PuppetIndividualDTO>(RedisKeyDefine._PuppetIndividualPerfix, id.ToString()).Result;
                if (rolepuppet != null&& puppetIndividual!=null)
                {
                    if (rolepuppet.PuppetDict.ContainsKey(id))
                    {
                        if (rolepuppet.IsBattle != id)
                        {
                            rolepuppet.PuppetDict.Remove(id);
                            Dictionary<byte, object> dict = new Dictionary<byte, object>();
                            dict.Add((byte)ParameterCode.RolePuppet, rolepuppet);
                            dict.Add((byte)ParameterCode.GetPuppetIndividual, puppetIndividual);
                            PuppetManagerSuccessS2C(roleid, PuppetOpCode.AbandonPuppet, dict);

                            await RedisHelper.Hash.HashDeleteAsync(RedisKeyDefine._PuppetIndividualPerfix, id.ToString());
                            await RedisHelper.Hash.HashSetAsync(RedisKeyDefine._RolePuppetPerfix, roleid.ToString(), rolepuppet);
                            await NHibernateQuerier.DeleteAsync(ChangeDataType(puppetIndividual));
                        }
                        else
                            PuppetManagerFailS2C(roleid, PuppetOpCode.AbandonPuppet,"无法丢弃出战傀儡");
                    }
                    else
                        PuppetManagerFailS2C(roleid, PuppetOpCode.AbandonPuppet);
                }
            }
        }

        #endregion

        #region MySql
        void GetPuppetStatusMySql(int roleid)
        {

        }
        #endregion

        RolePuppet ChangeDataType(RolePuppetDTO puppetDTO)
        {
            RolePuppet rolePuppet = new RolePuppet();
            rolePuppet.RoleID = puppetDTO.RoleID;
            rolePuppet.PuppetDict = Utility.Json.ToJson(puppetDTO.PuppetDict);
            rolePuppet.IsBattle = puppetDTO.IsBattle;
            return rolePuppet;
        }

        PuppetIndividual ChangeDataType(PuppetIndividualDTO individualDTO)
        {
            PuppetIndividual puppetIndividual = new PuppetIndividual();
            puppetIndividual.AttackPhysical = individualDTO.AttackPhysical;
            puppetIndividual.AttackPower = individualDTO.AttackPower;
            puppetIndividual.AttackSpeed = individualDTO.AttackSpeed;
            puppetIndividual.HP = individualDTO.HP;
            puppetIndividual.ID = individualDTO.ID;
            puppetIndividual.MP = individualDTO.MP;
            puppetIndividual.PuppetDurable = individualDTO.PuppetDurable;
            puppetIndividual.Skills = Utility.Json.ToJson(individualDTO.Skills);
            return puppetIndividual;
        }
    }
}
