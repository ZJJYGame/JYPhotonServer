using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionRegion
{
    /// <summary>
    /// 自定义的向量
    /// </summary>
    [Serializable]
    public struct Vector3 : IEquatable<Vector3>
    {
        public Vector3(float x, float y,float z) : this()
        {
            X = x;
            Y = y;
            Z = z;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        static Vector3 zero = new Vector3(0, 0, 0);
        public static Vector3 Zero { get { return zero; } }
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }
        public static Vector3 operator - (Vector3 a, Vector3 b)
        {
            return new Vector3(a.X - b.X, a.Y - b.Y,a.Z-b.Z);
        }
        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X + b.X, a.Y + b.Y,a.Z+b.Z);
        }
        public static Vector3 operator *(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X * b.X, a.Y *b.Y, a.Z * b.Z);
        }
        public static Vector3 operator /(Vector3 a, Vector3 b)
        {
            return new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }
        public static Vector3 operator /(Vector3 a, float number)
        {
            return new Vector3(a.X / number, a.Y / number, a.Z / number);
        }
        public static Vector3 operator *(Vector3 a, float number)
        {
            return new Vector3(a.X *number, a.Y *number, a.Z * number);
        }
        public bool Equals(Vector3 other)
        {
            return other.X == X && other.Y == Y&&other.Z==Z;
        }
        public override bool Equals(object obj)
        {
            return obj is Vector3 && Equals((Vector3)obj);
        }
        public void SetValue(float x, float y,float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode()^Z.GetHashCode();
        }
        public override string ToString()
        {
            return "X : " + X + "; Y : " + Y+"Z : "+Z;
        }
    }
}
