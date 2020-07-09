using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
namespace AscensionData
{
    public  class ResVariable:IReference
    {
        /// <summary>
        /// 全局ID
        /// </summary>
        public int GlobalID { get; set; }
        /// <summary>
        /// 等级/品级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 等级/品级 偏移量；
        /// 创建时候根据随机数进行区间随机
        /// </summary>
        public uint LevelOffset { get; set; }
        /// <summary>
        /// 生成的数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 生成数量的偏移量；
        /// 创建时候根据随机数进行区间随机
        /// </summary>
        public uint CountOffset { get; set; }
        public ResVariable(int globalID, int level, uint levelOffset, int count, uint countOffset)
        {
            GlobalID = globalID;
            Level = level;
            LevelOffset = levelOffset;
            Count = count;
            CountOffset = countOffset;
        }
        public ResVariable(int globalID, int level, int count) : this()
        {
            GlobalID = globalID;
            Level = level;
            Count = count;
        }
        public ResVariable()
        {
            LevelOffset = 0;
            CountOffset = 0;
        }
        public void SetValue(int globalID, int level, uint levelOffset, int count, uint countOffset)
        {
            GlobalID = globalID;
            Level = level;
            LevelOffset = levelOffset;
            Count = count;
            CountOffset = countOffset;
        }
        public void SetValue(int globalID, int level, int count)
        {
            GlobalID = globalID;
            Level = level;
            Count = count;
        }
        public void Clear()
        {
            GlobalID = 0;
            Level = 0;
            LevelOffset = 0;
            Count = 0;
            CountOffset = 0;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ResVariable))
                return false;
            var resObj = obj as ResVariable;
            return resObj.GlobalID == GlobalID && resObj.Level == Level
                && resObj.LevelOffset == LevelOffset && resObj.Count 
                == Count && resObj.CountOffset == CountOffset;
        }
        public override string ToString()
        {
            var str = "GlobalID : " + GlobalID + " Level : " + Level + "LevelOffset : " 
                + LevelOffset + "  Count : " + Count+ " CountOffset : " + CountOffset;
            return str;
        }
    }
}
