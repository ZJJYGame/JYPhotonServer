/*
*Author : Don
*Since 	:2020-04-18
*Description  : 请求角色数值处理者，返回角色的status数值；
* 此类数值只包含基础属性，例如血量，蓝量等，不包含门派之类的属性
* 详情参见MySql中的  role_status 表
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
namespace AscensionServer
{
    public class SyncRoleStatusHandler : Handler
    {
        public override void OnInitialization()
        {
            OpCode = OperationCode.SyncRoleStatus;
            base.OnInitialization();
            OnSubHandlerInitialization<SyncRoleStatusSubHandler>();
        }
    }
}
