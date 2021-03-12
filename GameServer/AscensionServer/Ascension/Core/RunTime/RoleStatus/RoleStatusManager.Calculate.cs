using AscensionServer.Model;
using Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionServer
{
   public partial class RoleStatusManager
    {
        public  void GetRoleStatus(List<GongFa> gongfaList, List<MishuSkillData> mishuList, RoleStatusDatas roleStatusDatas, out RoleStatus roleStatus)
        {
            var roleStatustemp = CosmosEntry.ReferencePoolManager.Spawn<RoleStatus>();
            roleStatustemp.Clear();
            roleStatus = CosmosEntry.ReferencePoolManager.Spawn<RoleStatus>();
            if (gongfaList.Count > 0)
            {
                for (int i = 0; i < gongfaList.Count; i++)
                {

                }
            }
            if (mishuList.Count > 0)
            {
                for (int i = 0; i < mishuList.Count; i++)
                {

                }
            }
            OutRolestatus(roleStatus, roleStatusDatas, roleStatustemp, out var tempstatus);
            roleStatus = tempstatus;
            // CosmosEntry.ReferencePoolManager.Despawns(roleStatustemp, roleStatus);
        }

        public  void OutRolestatus(RoleStatus roleStatus, RoleStatusDatas roleStatusDatas, RoleStatus roleStatusTemp, out RoleStatus tempstatus)
        {
            tempstatus = new RoleStatus();
           

        }
    }
}
