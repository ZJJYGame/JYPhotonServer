
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

        public void S2CPetSetBattle(int roleid, string s2cMessage, ReturnCode returnCode)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = s2cMessage;
            opData.OperationCode = (byte)OperationCode.SyncSetPetBattle;
            opData.ReturnCode = (byte)returnCode;
            Utility.Debug.LogInfo("yzqDat设置宠物出战发送了");
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
        }

        public void S2CRemoveRolePet(int roleid, string s2cMessage, ReturnCode returnCode)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = s2cMessage;
            opData.OperationCode = (byte)OperationCode.SyncRemoveRolePet;
            opData.ReturnCode = (byte)returnCode;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
            Utility.Debug.LogInfo("yzqData放生宠物信息发送了");
        }


        #region 宠物数据发送待更换
        public void S2CPetAllStatus(int roleid, string s2cMessage)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = s2cMessage;
            opData.OperationCode = (byte)OperationCode.SyncPetStatus;
            opData.ReturnCode = (byte)ReturnCode.Success;
            Utility.Debug.LogInfo("yzqData获得宠物所有数据信息发送了");
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
        }

        public void S2CPetAbilityPoint(int roleid, string s2cMessage, ReturnCode returnCode)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = s2cMessage;
            opData.OperationCode = (byte)OperationCode.SyncPetStatus;
            opData.ReturnCode = (byte)returnCode;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
            Utility.Debug.LogInfo("yzqData变更宠物加点发送了");
        }

        public void S2CPetStudySkill(int roleid, string s2cMessage, ReturnCode returnCode)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = s2cMessage;
            opData.OperationCode = (byte)OperationCode.SyncPetStatus;
            opData.ReturnCode = (byte)returnCode;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
            Utility.Debug.LogInfo("yzqData宠物技能学习发送了");
        }

        public void S2CPetRename(int roleid, string s2cMessage, ReturnCode returnCode)
        {
            OperationData opData = new OperationData();
            opData.DataMessage = s2cMessage;
            opData.OperationCode = (byte)OperationCode.SyncPetStatus;
            opData.ReturnCode = (byte)returnCode;
            GameManager.CustomeModule<RoleManager>().SendMessage(roleid, opData);
            Utility.Debug.LogInfo("yzqData宠物更改名字发送了");
        }
        #endregion
    }
}
