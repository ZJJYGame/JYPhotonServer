using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace AscensionServer
{
    [Serializable]
    [MessagePackObject(true)]
    public struct FixVector3
    {
        public static FixVector3 One { get { return new FixVector3(Vector3.one); } }
        public static FixVector3 Zero{ get { return new FixVector3(Vector3.zero); } }
        public FixVector3(Vector3 vector)
        {
            this.X = Mathf.FloorToInt(vector.x * 1000);
            this.Y = Mathf.FloorToInt(vector.y * 1000);
            this.Z = Mathf.FloorToInt(vector.z * 1000);
        }
        public FixVector3(float x, float y, float z)
        {
            this.X = Mathf.FloorToInt(x * 1000);
            this.Y = Mathf.FloorToInt(y * 1000);
            this.Z = Mathf.FloorToInt(z * 1000);
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public static FixVector3 SetVector(Vector3 vector)
        {
            return new FixVector3(vector);
        }
        public Vector3 GetVector()
        {
            return new Vector3((float)X / 1000, (float)Y / 1000, (float)Z / 1000);
        }
        public override string ToString()
        {
            return $"X:{X} ; Y:{Y} ; Z:{Z}";
        }
    }
}
