using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol.DTO;
using AscensionProtocol;
using AscensionServer.Model;
using RedisDotNet;
using Cosmos;
using Protocol;

namespace AscensionServer
{
   public partial class SecondaryJobManager
    {
        #region  Redis模块   
        /// <summary>
        /// 学习新锻造配方
        /// </summary>
        void UpdateForgeS2C(int roleID,int useItemID)
        {

        }
        #endregion

        #region MySql

        #endregion




        ForgeDTO ChangeDataType(Forge forge)
        {
            ForgeDTO forgeDTO = new ForgeDTO();
            forgeDTO.RoleID = forge.RoleID;
            forgeDTO.JobLevel = forge.JobLevel;
            forgeDTO.JobLevelExp = forge.JobLevelExp;
            forgeDTO.Recipe_Array =Utility.Json.ToObject<HashSet<int>>(forge.Recipe_Array);
            return forgeDTO;
        }
    }
}
