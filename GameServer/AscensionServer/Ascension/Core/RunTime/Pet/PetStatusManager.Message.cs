
using Protocol;
using Cosmos;
using AscensionProtocol;
using AscensionProtocol.DTO;
namespace AscensionServer
{
   public partial class PetStatusManager
    {
        public void S2CRolePetOperateSuccess(int roleid,string s2cMessage)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = s2cMessage;
            opData.OperationCode = (byte)OperationCode.SyncGetRoleAllPet;
            opData.ReturnCode = (byte)ReturnCode.Success;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
        }

        public void S2CRoleAddPetSuccess(int roleid)
        {
            OperationData opData = new OperationData();
            opData.OperationCode = (byte)OperationCode.SyncAddRolePet;
            opData.ReturnCode = (byte)ReturnCode.Success;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
            Utility.Debug.LogInfo("yzqData增加新宠物发送了");
        }
    }
}
