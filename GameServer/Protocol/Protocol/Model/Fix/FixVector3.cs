//***********************************************************
// 描述：
// 作者：Don  
// 创建时间：2020-11-09 13:46:19
// 版 本：1.0
//***********************************************************
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace Protocol
{
    [Serializable]
    [MessagePackObject]
    public struct FixVector3
    {
        public FixVector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        [Key(0)]
        public int x;
        [Key(1)]
        public int y;
        [Key(2)]
        public int z;
        public static FixVector3 SetVector(Vector3 vector)
        {
            var X = Mathf.FloorToInt(vector.x * 1000);
            var Y = Mathf.FloorToInt(vector.y * 1000);
            var Z = Mathf.FloorToInt(vector.z * 1000);
            return new FixVector3(X, Y, Z);
        }
        public static Vector3 GetVector(FixVector3 fixVector3)
        {
            return new Vector3((float)fixVector3.x / 1000, (float)fixVector3.y / 1000, (float)fixVector3.z / 1000);
        }
    }
}
