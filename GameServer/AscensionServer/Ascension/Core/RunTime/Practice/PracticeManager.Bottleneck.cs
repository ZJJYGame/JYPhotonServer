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
    public partial class PracticeManager
    {
        #region Redis模块
        /// <summary>
        /// 触发瓶颈发送
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="level"></param>
        async void TriggerBottleneckS2C(int roleID,int level)
        {
            if (RedisHelper.Hash.HashExist(RedisKeyDefine._RoleBottleneckPostfix, roleID.ToString()))
            {
                var bottleneckRedis = RedisHelper.Hash.HashGet<Bottleneck>(RedisKeyDefine._RoleBottleneckPostfix, roleID.ToString());
                var role =  RedisHelper.Hash.HashGetAsync<RoleDTO>(RedisKeyDefine._RolePostfix, roleID.ToString()).Result;
                if (bottleneckRedis==null)
                {
                    TriggerBottleneck(roleID, level);
                    return;
                }
                if (role==null)
                {
                    TriggerBottleneck(roleID, level);
                    return;
                }
                int count = Utility.Json.ToObject<List<int>>(role.RoleRoot).Count;
                //var bottleneck = JudgeBottleneck(level, count, bottleneckRedis);
                #region 待优化

                #endregion
                //if (bottleneck != null)
                //{
                //    ResultSuccseS2C(roleID, PracticeOpcode.TriggerBottleneck, bottleneck);
                //}
            }
            else
                TriggerBottleneck(roleID,level);
        }
        #endregion
        #region MySql模块
        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="level"></param>
         Bottleneck TriggerBottleneck(int roleID, int level)
        {
            NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
            var bottleneck = NHibernateQuerier.CriteriaSelectAsync<Bottleneck>(nHCriteria).Result;
            if (bottleneck == null)
            {
                ResultFailS2C(roleID, PracticeOpcode.TriggerBottleneck);
                return null;
            }
            var roleObj = NHibernateQuerier.CriteriaSelectAsync<Role>(nHCriteria).Result;
            if (roleObj == null)
            {
                ResultFailS2C(roleID, PracticeOpcode.TriggerBottleneck);
                return null;
            }
            int count = Utility.Json.ToObject<List<int>>(roleObj.RoleRoot).Count;
            List<int> rootPercentNum;
            GameEntry.DataManager.TryGetValue<Dictionary<int, BottleneckData>>(out var bottleneckData);
            GameEntry.DataManager.TryGetValue<Dictionary<int, DemonData>>(out var demonData);
            GetRootPercent(bottleneckData[level], count, out rootPercentNum);
            if (GetPercent(rootPercentNum[0] / (float)100))
            {
                bottleneck.IsBottleneck = true;
                bottleneck.BreakThroughVauleMax = rootPercentNum[1];
                if (bottleneckData[level].IsFinalLevel)
                {
                    bottleneck.IsThunder = true;
                    bottleneck.ThunderRound = bottleneckData[level].Thunder_Round;//获取天劫回合数
                    int demonIndex = GetDemonPercent(demonData[level], bottleneck.CraryVaule);
                    if (GetPercent(demonData[level].Trigger_Chance[demonIndex] / (float)100))
                    {
                        bottleneck.IsDemon = true;
                        bottleneck.DemonID = demonData[level].Demon_ID[demonIndex];
                    }
                }
                ResultSuccseS2C(roleID, PracticeOpcode.TriggerBottleneck, bottleneck);//触发瓶颈结束
                return bottleneck;
            }
            else
                return null;        
        }
        #endregion
        /// <summary>
        /// 判断是否触发瓶颈
        /// </summary>
        //Bottleneck JudgeBottleneck(int level,int roleID)
        //{
        //    NHCriteria nHCriteria = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleID);
        //    var bottleneck = NHibernateQuerier.CriteriaSelectAsync<Bottleneck>(nHCriteria).Result;

        //    if (bottleneck== null)
        //    {
        //        ResultFailS2C(roleID, PracticeOpcode.TriggerBottleneck);
        //        return null;
        //    }
        //    var roleObj = NHibernateQuerier.CriteriaSelectAsync<Role>(nHCriteria).Result;
        //    if (roleObj == null)
        //    {
        //        ResultFailS2C(roleID, PracticeOpcode.TriggerBottleneck);
        //        return null;
        //    }
        //    int count = Utility.Json.ToObject<List<int>>(roleObj.RoleRoot).Count;
        //    List<int> rootPercentNum;
        //    GameEntry.DataManager.TryGetValue<Dictionary<int, BottleneckData>>(out var bottleneckData);
        //    GameEntry.DataManager.TryGetValue<Dictionary<int, DemonData>>(out var demonData);
        //    GetRootPercent(bottleneckData[level], count, out rootPercentNum);
        //    if (GetPercent(rootPercentNum[0] / (float)100))
        //    {
        //        bottleneck.IsBottleneck = true;
        //        bottleneck.BreakThroughVauleMax = rootPercentNum[1];
        //        if (bottleneckData[level].IsFinalLevel)
        //        {
        //            bottleneck.IsThunder = true;
        //            bottleneck.ThunderRound = bottleneckData[level].Thunder_Round;//获取天劫回合数
        //            int demonIndex = GetDemonPercent(demonData[level], bottleneck.CraryVaule);
        //            if (GetPercent(demonData[level].Trigger_Chance[demonIndex] / (float)100))
        //            {
        //                bottleneck.IsDemon = true;
        //                bottleneck.DemonID = demonData[level].Demon_ID[demonIndex];
        //            }
        //        }
        //        return bottleneck;
        //    }
        //    else
        //        return null;
        //}
        //获取概率值
        private bool GetPercent(float num)
        {
            Random random = new Random();
            int randommNum = random.Next(1, 10001);
            float percentNum = num * 10000;
            Utility.Debug.LogInfo("yzqData概率值为" + percentNum + "当前值为" + randommNum);
            if (randommNum <= (int)percentNum)
            {
                return true;
            }
            else
                return false;
        }
        /// <summary>
        /// 获取零灵根概率
        /// </summary>
        /// <param name="bottleneckData"></param>
        /// <param name="rootnum"></param>
        /// <returns></returns>
        private void GetRootPercent(BottleneckData bottleneckData, int rootnum, out List<int> num)
        {
            switch (rootnum)
            {
                case 1:
                    num = bottleneckData.Spiritual_Root_1;
                    break;
                case 2:
                    num = bottleneckData.Spiritual_Root_2;
                    break;
                case 3:
                    num = bottleneckData.Spiritual_Root_3;
                    break;
                case 4:
                    num = bottleneckData.Spiritual_Root_4;
                    break;
                case 5:
                    num = bottleneckData.Spiritual_Root_5;
                    break;
                default:
                    num = null;
                    break;
            }

        }
        /// <summary>
        /// 获取心魔对应数组的下标
        /// </summary>
        /// <param name="demonData"></param>
        /// <param name="CraryVaule"></param>
        /// <returns></returns>
        private int GetDemonPercent(DemonData demonData, int CraryVaule)
        {
            int index = 0;
            for (int i = 0; i < demonData.Crary_Value.Count; i++)
            {
                if (CraryVaule >= demonData.Crary_Value[i] && CraryVaule <= demonData.Crary_Value[i + 1])
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }
}
