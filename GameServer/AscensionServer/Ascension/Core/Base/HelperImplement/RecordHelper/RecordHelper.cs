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
    {/// <summary>
    /// 记录离线时间
    /// </summary>
    /// <param name="roleId">离线账号</param>
    /// <param name="data">新上线账号</param>
        public async void RecordRole(IRoleEntity roleEntity)
        {
            Utility.Debug.LogInfo("yzqData" + "同步离线时间成功" + "原来的角色id为" +roleEntity.RoleId);
            #region 记录离线时间
            if (roleEntity.RoleId== -1)
            {
                Utility.Debug.LogInfo("============AscensionPeer.RecordOnOffLine() : Can't RecordOnOffLine ============");
                return;
            }
            NHCriteria nHCriteriaOnOff = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID",roleEntity.RoleId);
            var obj = NHibernateQuerier.CriteriaSelectAsync<OffLineTime>(nHCriteriaOnOff).Result;
            var roleAllianceobj = NHibernateQuerier.CriteriaSelectAsync<RoleAlliance>(nHCriteriaOnOff).Result;
            if (roleAllianceobj != null)
            {
                NHCriteria nHCriteriaAllianceStatus = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("ID", roleAllianceobj.AllianceID);
                var allianceStatusobj = NHibernateQuerier.CriteriaSelectAsync<AllianceStatus>(nHCriteriaAllianceStatus).Result;
                if (allianceStatusobj != null)
                {
                    allianceStatusobj.OnLineNum--;
                    await NHibernateQuerier.UpdateAsync(allianceStatusobj);
                }
                GameManager.ReferencePoolManager.Despawn(nHCriteriaAllianceStatus);
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
                var offLineTimeTmp = GameManager.ReferencePoolManager.Spawn<OffLineTime>();
                offLineTimeTmp.RoleID = roleEntity.RoleId;
                offLineTimeTmp.OffTime = DateTime.Now.ToString();
                NHibernateQuerier.Insert(offLineTimeTmp);
                GameManager.ReferencePoolManager.Despawn(offLineTimeTmp);
            }
            GameManager.ReferencePoolManager.Despawns(nHCriteriaOnOff);
            //Utility.Debug.LogInfo("yzqData同步离线时间成功"+"原来的角色id为"+ roleId + "新的角色id"+ newrole.RoleID);
            #endregion
        }

       
    }
}
