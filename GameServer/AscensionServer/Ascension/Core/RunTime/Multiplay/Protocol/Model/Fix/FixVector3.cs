﻿using MessagePack;
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
        public FixVector3(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public static FixVector3 SetVector(Vector3 vector)
        {
            var X = Mathf.FloorToInt(vector.x * 1000);
            var Y = Mathf.FloorToInt(vector.y * 1000);
            var Z = Mathf.FloorToInt(vector.z * 1000);
            return new FixVector3(X, Y, Z);
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
