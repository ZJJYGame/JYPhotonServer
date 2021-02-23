using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AscensionProtocol;
using AscensionProtocol.DTO;
using Cosmos;
namespace AscensionServer
{
    public interface ISecondaryJobManager:IModuleManager
    {
        void OperateProcessing(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole);
        /// <summary>
        /// 获得所有已学配方
        /// </summary>
        /// <param name="secondaryJobDTO"></param>
        /// <param name="nHCriteriarole"></param>
        void GetRoleAlchemy(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole);
        /// <summary>
        /// 学习新配方
        /// </summary>
        /// <param name="secondaryJobDTO"></param>
        /// <param name="nHCriteriarole"></param>
        void UpdateRoleAlchemy(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole);
        void CompoundAlchemy(SecondaryJobDTO secondaryJobDTO, NHCriteria nHCriteriarole);
        void S2CAlchemyMessage(int roleid, string s2cMessage, ReturnCode returnCode);
    }
}


