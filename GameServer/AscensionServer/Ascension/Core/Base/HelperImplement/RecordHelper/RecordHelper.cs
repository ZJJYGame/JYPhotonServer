using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using RedisDotNet;
namespace AscensionServer
{
    [ImplementProvider]
    public class RecordHelper : IRecordHelper
    {
        /// <summary>
        /// 记录离线时间
        /// </summary>
        /// <param name="roleId">离线账号</param>
        /// <param name="data">新上线账号</param>
        public async void RecordRole(RoleEntity roleEntity)
        {
            Utility.Debug.LogInfo("yzqData" + "同步离线时间成功" + "原来的角色id为" + roleEntity.RoleId);
            #region 记录离线时间
            if (roleEntity.RoleId == -1)
            {
                Utility.Debug.LogInfo("============AscensionPeer.RecordOnOffLine() : Can't RecordOnOffLine ============");
                return;
            }
            NHCriteria nHCriteriaOnOff = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleEntity.RoleId);
            var obj = NHibernateQuerier.CriteriaSelectAsync<OffLineTime>(nHCriteriaOnOff).Result;
            var roleAllianceobj = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriaOnOff).Result;
            if (roleAllianceobj != null)
            {
                NHCriteria nHCriteriaAllianceStatus = CosmosEntry.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", roleAllianceobj.AllianceID);
                var allianceStatusobj = NHibernateQuerier.CriteriaSelectAsync<AllianceStatus>(nHCriteriaAllianceStatus).Result;
                if (allianceStatusobj != null)
                {
                    if (allianceStatusobj.OnLineNum > 0)
                    {
                        allianceStatusobj.OnLineNum -= 1;
                    }
                    await NHibernateQuerier.UpdateAsync(allianceStatusobj);
                }
                CosmosEntry.ReferencePoolManager.Despawn(nHCriteriaAllianceStatus);
                roleAllianceobj.JoinOffline = DateTime.Now.ToString();
                await NHibernateQuerier.UpdateAsync(roleAllianceobj);
            }
            if (obj != null)
            {
                obj.OffTime = DateTime.Now.ToString();
                obj.RoleID = roleEntity.RoleId;
                NHibernateQuerier.Update(obj);
            }
            else
            {
                var offLineTimeTmp = CosmosEntry.ReferencePoolManager.Spawn<OffLineTime>();
                offLineTimeTmp.RoleID = roleEntity.RoleId;
                offLineTimeTmp.OffTime = DateTime.Now.ToString();
                NHibernateQuerier.Insert(offLineTimeTmp);
                CosmosEntry.ReferencePoolManager.Despawn(offLineTimeTmp);
            }
            CosmosEntry.ReferencePoolManager.Despawns(nHCriteriaOnOff);
            //Utility.Debug.LogInfo("yzqData同步离线时间成功"+"原来的角色id为"+ roleId + "新的角色id"+ newrole.RoleID);
            #endregion
        }
    }
}


