using AscensionProtocol;
using AscensionProtocol.DTO;
using Cosmos;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 用于发送给客户端
/// </summary>
namespace AscensionServer
{
    public partial class ServerTeamManager
    {

        /// <summary>
        /// 服务器主动给客户端发消息 初始化队伍信息
        /// </summary>
        public void ServerToClientInit(int roleId)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = ServerToClientParams();
            opData.OperationCode = (byte)OperationCode.SyncTeamMessageInit;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleId, opData);
        }
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="roleId"></param>
        public void ServerToClientCreate(int roleId)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = ServerToClientParams();
            opData.OperationCode = (byte)OperationCode.SyncTeamMessageCreate;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleId, opData);
        }
        /// <summary>
        /// 申请加入队伍
        /// </summary>
        /// <param name="roleId"></param>
        public void ServerToClientApply(int teamLeader)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = ServerToClientParams();
            opData.OperationCode = (byte)OperationCode.SyncTeamMessageApply;
            GameManager.CustomeModule<RoleManager>().SendMessage(teamLeader, opData);
        }
        
        /// <summary>
        /// 同意加入队伍
        /// </summary>
        /// <param name="roleId"></param>
        public void ServerToClientJoin(List<RoleDTO> roleDTOs)
        {
            foreach (var ov in roleDTOs)
            {
                OperationData opData = new OperationData();
                opData.DataMessage = ServerToClientParams();
                opData.OperationCode = (byte)OperationCode.SyncTeamMessageJoin;
                GameManager.CustomeModule<RoleManager>().SendMessage(ov.RoleID, opData);
            }
        }

        /// <summary>
        /// 拒绝加入队伍
        /// </summary>
        /// <param name="roleId"></param>
        public void ServerToClientRefused(int leaderId)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = ServerToClientParams();
            opData.OperationCode = (byte)OperationCode.SyncTeamMessageRefused;
            GameManager.CustomeModule<RoleManager>().SendMessage(leaderId, opData);
        }

        /// <summary>
        ///转让队长
        /// </summary>
        /// <param name="roleDTOs"></param>
        public void ServerToClientTransfer(List<RoleDTO> roleDTOs)
        {
            foreach (var ov in roleDTOs)
            {
                OperationData opData = new OperationData();
                opData.DataMessage = ServerToClientParams();
                opData.OperationCode = (byte)OperationCode.SyncTeamMessageTransfer;
                GameManager.CustomeModule<RoleManager>().SendMessage(ov.RoleID, opData);
            }
        }


        /// <summary>
        /// 退出队伍
        /// </summary>
        /// <param name="roleId"></param>
        public void ServerToClientDissolveTeam(List<RoleDTO> roleDTOs)
        {
            foreach (var ov in roleDTOs)
            {
                OperationData opData = new OperationData();
                opData.DataMessage = ServerToClientParams();
                opData.OperationCode = (byte)OperationCode.ExitBattle;
                GameManager.CustomeModule<RoleManager>().SendMessage(ov.RoleID, opData);
            }
        }

    }
}
