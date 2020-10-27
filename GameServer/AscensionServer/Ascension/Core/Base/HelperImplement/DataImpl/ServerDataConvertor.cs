using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using System.IO;
using log4net.Config;
using AscensionServer.Threads;
using System.Reflection;
using ExitGames.Concurrency.Fibers;
using Cosmos;
using AscensionServer.Model;
using AscensionData;
using System.Threading;

namespace AscensionServer
{
    /// <summary>
    /// 服务器数据转换
    /// </summary>
    [ImplementProvider]
    public class ServerDataConvertor : IDataConvertor
    {
        public void ConvertData()
        {
            //Utility.Debug.LogInfo("<DataManager> 测试 ConvertData");
            try
            {


                #region 宗门藏宝阁/藏经阁/瓶颈/心魔/仙盟等级/仙盟签到/秘术属性
                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(FactionItemData).Name, out var factionitemdata);
                var factionitemDataDict = TransObject<List<FactionItemData>>(factionitemdata).ToDictionary(key => key.FactionItemId, value => value.FactionItem);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(FactionSkillData).Name, out var factionSkilldata);
                var factionSkillDataDict = TransObject<List<FactionSkillData>>(factionSkilldata).ToDictionary(key => key.SchoolID, value => value.FactionSkill);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(BottleneckData).Name, out var bottleneckData);
                var bottleneckDataDict = TransObject<List<BottleneckData>>(bottleneckData).ToDictionary(key => key.Level_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(DemonData).Name, out var demonData);
                var demonDataDict = TransObject<List<DemonData>>(demonData).ToDictionary(key => key.Level_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(AllianceLevleUpData).Name, out var alliancelevledata);
                var alliancelevledataDict = TransObject<List<AllianceLevleUpData>>(alliancelevledata).ToDictionary(key => key.Building_Level, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(AllianceSigninData).Name, out var allianceSigninData);
                var allianceSigninDataDict = TransObject<List<AllianceSigninData>>(allianceSigninData);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(MiShuData).Name, out var mishuData);
                var mishuDataDict = TransObject<List<MiShuData>>(mishuData).ToDictionary(key => key.Mishu_ID, value => value.mishuSkillDatas);
                #endregion


                #region 战斗json


                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(GongFaBook).Name, out var gongfaBookSet);
                var gfbDict = TransObject<List<GongFaBook>>(gongfaBookSet).ToDictionary(key => key.Book_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(GongFa).Name, out var gongfaSet);
                var gfDict = TransObject<List<GongFa>>(gongfaSet).ToDictionary(key => key.Gongfa_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(MonsterDatas).Name, out var monsterDatasSet);
                var monsterDict = TransObject<List<MonsterDatas>>(monsterDatasSet).ToDictionary(key => key.Monster_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(SkillGongFaDatas).Name, out var skillGongFaDatasSet);
                var skillGongFaDict = TransObject<List<SkillGongFaDatas>>(skillGongFaDatasSet).ToDictionary(key => key.Skill_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(SkillMiShuDatas).Name, out var skillMiShuDatasSet);
                var skillMiShuDict = TransObject<List<SkillMiShuDatas>>(skillMiShuDatasSet).ToDictionary(key => key.Skill_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(DrugData).Name, out var drugSet);
                var drugDict = TransObject<List<DrugData>>(drugSet).ToDictionary(key => key.Drug_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(RunesData).Name, out var runesSet);
                var runesDict = TransObject<List<RunesData>>(runesSet).ToDictionary(key => key.Runes_ID, value => value);

                GameManager.CustomeModule<DataManager>().TryGetValue(typeof(MagicWeaponData).Name, out var magicSet);
                var magicDict = TransObject<List<MagicWeaponData>>(magicSet).ToDictionary(key => key.Magic_ID, value => value);
                #endregion

                GameManager.CustomeModule<DataManager>().TryAdd(mishuDataDict);
                GameManager.CustomeModule<DataManager>().TryAdd(allianceSigninDataDict);
                GameManager.CustomeModule<DataManager>().TryAdd(alliancelevledataDict);
                GameManager.CustomeModule<DataManager>().TryAdd(demonDataDict);
                GameManager.CustomeModule<DataManager>().TryAdd(bottleneckDataDict);
                GameManager.CustomeModule<DataManager>().TryAdd(factionSkillDataDict);
                GameManager.CustomeModule<DataManager>().TryAdd(factionitemDataDict);

                GameManager.CustomeModule<DataManager>().TryAdd(gfbDict);
                GameManager.CustomeModule<DataManager>().TryAdd(gfDict);
                GameManager.CustomeModule<DataManager>().TryAdd(monsterDict);
                GameManager.CustomeModule<DataManager>().TryAdd(skillGongFaDict);
                GameManager.CustomeModule<DataManager>().TryAdd(skillMiShuDict);
                GameManager.CustomeModule<DataManager>().TryAdd(drugDict);
                GameManager.CustomeModule<DataManager>().TryAdd(runesDict);
                GameManager.CustomeModule<DataManager>().TryAdd(magicDict);

                //GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MiShuData>>(out var set);
                //Utility.Debug.LogInfo("<DataManager> 测试 TryGetValue " + Utility.Json.ToJson(set));
                //GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MonsterDatas>>(out var set);
                //Utility.Debug.LogInfo("<DataManager> 测试 TryGetValue " + skillGongFaDict[21001].Skill_Describe);
                //Utility.Debug.LogInfo("<DataManager> 测试 TryGetValue " + skillMiShuDict[21009].Skill_Describe);

                //Utility.Debug.LogInfo("<DataManager> 测试 ConvertData Step0211111111111====>" + drugDict.Count);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }

           

            //Utility.Debug.LogInfo("<DataManager> 测试 ConvertData Step0211111111111");
        }

        /// <summary>
        /// 转换为对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public T TransObject <T>(string data)
        {
            return Utility.Json.ToObject<T>(data);
        } 


    }
}