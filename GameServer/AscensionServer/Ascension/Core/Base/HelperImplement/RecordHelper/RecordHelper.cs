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
    [TargetHelper]
    public class RecordHelper : IRecordHelper
    {/// <summary>
    /// 记录离线时间
    /// </summary>
    /// <param name="roleId">离线账号</param>
    /// <param name="data">新上线账号</param>
        public void RecordRole(int roleId, object data)
        {
            Utility.Debug.LogInfo("yzqData" + "同步离线时间成功" + "原来的角色id为" + roleId);
            RoleDTO newrole = data as RoleDTO;

            #region 记录离线时间
            if (roleId == -1)
            {
                Utility.Debug.LogInfo("============AscensionPeer.RecordOnOffLine() : Can't RecordOnOffLine ============");
                return;
            }
            NHCriteria nHCriteriaOnOff = GameManager.ReferencePoolManager.Spawn<NHCriteria>().SetValue("RoleID", roleId);
            var obj = NHibernateQuerier.CriteriaSelectAsync<OffLineTime>(nHCriteriaOnOff).Result;
            if (obj != null)
            {
                obj.OffTime = DateTime.Now.ToString();
                obj.RoleID = roleId;
                NHibernateQuerier.Update(obj);
            }
            else
            {
                var offLineTimeTmp = GameManager.ReferencePoolManager.Spawn<OffLineTime>();
                offLineTimeTmp.RoleID = roleId;
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
