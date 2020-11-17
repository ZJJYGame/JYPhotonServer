﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using AscensionProtocol.DTO;
using RedisDotNet;

namespace AscensionServer
{
    [CustomeModule]
    public partial class AuctionManager: Module<AuctionManager>
    {
        HashSet<string> occupyGuidHash;
        public override void OnInitialization()
        {
            
        }

        
        public async Task<bool> IsAuctionGoodsExist(string Guid)
        {
            bool isExist;
            isExist = await RedisHelper.Hash.HashExistAsync("AuctionGoodsData_", Guid);
            isExist=((await RedisHelper.String.StringGetAsync("AuctionGoods_" + Guid))!=null)&&isExist;
            return isExist;
        }
        //public async Task<bool> TryGetAuctionAuctionGoods()
        //{

        //}
    }
}