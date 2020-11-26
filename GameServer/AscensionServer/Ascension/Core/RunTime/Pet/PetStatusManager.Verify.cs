using Protocol;
using Cosmos;
using AscensionProtocol;
using AscensionProtocol.DTO;
using AscensionServer.Model;
using System.Collections.Generic;

namespace AscensionServer
{
   public partial class PetStatusManager
    {
        /// <summary>
        /// 验证宠物当前加点数
        /// </summary>
        /// <param name="pet"></param>
        /// <param name="points"></param>
        public void VerifyPetAbilitypoint(Pet pet,out int points)
        {
            points = 0;          
            GameManager.CustomeModule<DataManager>().TryGetValue<Dictionary<int, PetLevelData>>(out var petLevelDataDict);
            foreach (var item in petLevelDataDict)
            {
                if (pet.PetLevel>= item.Key)
                {
                    points += item.Value.FreeAttributes;
                }
            }
        }



    }
}
