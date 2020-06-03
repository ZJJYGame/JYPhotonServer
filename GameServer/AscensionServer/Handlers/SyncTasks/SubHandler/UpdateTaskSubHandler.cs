using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using AscensionProtocol;
using AscensionServer.Model;
namespace AscensionServer
{
    public class UpdateTaskSubHandler : SyncTaskSubHandler
    {
        public override void OnInitialization()
        {
            SubOpCode = SubOperationCode.Update;
        }
        public override void Handler(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
           
        }
        /*
        public override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters, AscensionPeer peer)
        {
            string roletask = Convert.ToString(Utility.GetValue(operationRequest.Parameters, (byte)ObjectParameterCode.Role));
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务相关信息：" + roletask + ">>>>>>>>>>>>>>>>>>>>>>");
            //2020-06-02 11:28:54,659 [15] INFO  AscensionServer.AscensionServer - >>>>>>>>>>>>>鎺ュ彈鍒扮殑浠诲姟鐩稿叧淇℃伅锛歿"RoleID":61,"RoleTaskDict":{"0001":{"TaskState":"1","TaskResult":"0002","TaskType":"0"}}}>>>>>>>>>>>>>>>>>>>>>>


            var roletaskobj = Utility.ToObject<RoleTaskProgress>(roletask);
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务：" + roletaskobj.RoleID + ">>>>>>>>>>>>>>>>>>>>>>");
            NHCriteria nHCriteriaRoleID = Singleton<ReferencePoolManager>.Instance.Spawn<NHCriteria>().SetValue("RoleID", roletaskobj.RoleID);
            bool exist = Singleton<NHManager>.Instance.Verify<Role>(nHCriteriaRoleID);
            ResponseData.Clear();
            AscensionServer._Log.Info(">>>>>>>>>>>>>接受到的任务strString：" + roletaskobj.RoleTaskDict + ">>>>>>>>>>>>>>>>>>>>>>");
            OpResponse.OperationCode = operationRequest.OperationCode;
            if (exist)
            {
                Singleton<NHManager>.Instance.Add(new RoleTaskProgress() { RoleTaskDict = roletaskobj.RoleTaskDict });
                var roleTaskProgress = Singleton<NHManager>.Instance.CriteriaGet<RoleTaskProgress>(nHCriteriaRoleID);
                AscensionServer._Log.Info(">>>>>>>>>>>>传回去的" + roleTaskProgress + ">>>>>>>>>>>>");
                ResponseData.Add((byte)ObjectParameterCode.Role, roleTaskProgress);
                OpResponse.Parameters = ResponseData;
                OpResponse.ReturnCode = (short)ReturnCode.Success;

            }
            else
                OpResponse.ReturnCode = (short)ReturnCode.Fail;
            peer.SendOperationResponse(OpResponse, sendParameters);
            Singleton<ReferencePoolManager>.Instance.Despawn(nHCriteriaRoleID);
        }*/
    }
}
