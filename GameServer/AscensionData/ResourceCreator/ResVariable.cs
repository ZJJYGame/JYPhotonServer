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
        /// 针对怪物数据浮动值
        /// </summary>
        public int FlowValue { get; set; }
        /// <summary>
        /// 针对怪物数据浮动值 偏移量；
        /// 创建时候根据随机数进行区间随机
        /// </summary>
        public uint FlowValueOffset { get; set; }
        /// <summary>
        /// 生成的数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 生成数量的偏移量；
        /// 创建时候根据随机数进行区间随机
        /// </summary>
        public uint CountOffset { get; set; }
     

        public ResVariable(int globalID, int flowValue, uint flowValueOffset, int count, uint countOffset)
        {
            GlobalID = globalID;
            FlowValue = flowValue;
            FlowValueOffset = flowValueOffset;
            Count = count;
            CountOffset = countOffset;
        }
        public ResVariable(int globalID, int flowValue, int count) : this()
        {
            GlobalID = globalID;
            FlowValue = flowValue;
            Count = count;
        }
        public ResVariable()
        {
            FlowValueOffset = 0;
            CountOffset = 0;
        }
        public void SetValue(int globalID, int flowValue, uint flowValueOffset, int count, uint countOffset)
        {
            GlobalID = globalID;
            FlowValue = flowValue;
            FlowValueOffset = flowValueOffset;
            Count = count;
            CountOffset = countOffset;
        }
        public void SetValue(int globalID, int flowValue, int count)
        {
            GlobalID = globalID;
            FlowValue = flowValue;
            Count = count;
        }
        public void Clear()
        {
            GlobalID = 0;
            FlowValue = 0;
            FlowValueOffset = 0;
            Count = 0;
            CountOffset = 0;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is ResVariable))
                return false;
            var resObj = obj as ResVariable;
            return resObj.GlobalID == GlobalID && resObj.FlowValue == FlowValue
                && resObj.FlowValueOffset == FlowValueOffset && resObj.Count 
                == Count && resObj.CountOffset == CountOffset;
        }
        public override string ToString()
        {
            var str = "GlobalID : " + GlobalID + " FlowValue : " + FlowValue + "FlowValueOffset : " 
                + FlowValueOffset + "  Count : " + Count+ " CountOffset : " + CountOffset;
            return str;
        }
    }
}
