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
                var allianceSigninDataDict = TransObject<List<AllianceSigninData>>(alliancelevledata);

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

                //GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, MonsterDatas>>(out var set);
                //Utility.Debug.LogInfo("<DataManager> 测试 TryGetValue " + skillGongFaDict[21001].Skill_Describe);
                //Utility.Debug.LogInfo("<DataManager> 测试 TryGetValue " + skillMiShuDict[21009].Skill_Describe);
            }
            catch (Exception e)
            {
                Utility.Debug.LogError(e);
            }
            Utility.Debug.LogInfo("<DataManager> 测试 ConvertData Step0211111111111");
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