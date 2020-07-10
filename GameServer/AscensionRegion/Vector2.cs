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
    public struct Vector2 : IEquatable<Vector2>
    {
        public Vector2(float x, float y) : this()
        {
            X = x;
            Y = y;
        }
        public float X { get; set; }
        public float Y { get; set; }
        static Vector2 zero = new Vector2(0, 0);
        public static Vector2 Zero { get { return zero; } }
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }
        public static Vector2 operator - (Vector2 a, Vector2 b)
        {
            return new Vector2(a.X - b.X, a.Y - b.Y);
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X + b.X, a.Y + b.Y);
        }
        public static Vector2 operator *(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X * b.X, a.Y * b.Y);
        }
        public static Vector2 operator /(Vector2 a, Vector2 b)
        {
            return new Vector2(a.X / b.X, a.Y / b.Y);
        }
        public static Vector2 operator /(Vector2 a, float number)
        {
            return new Vector2(a.X / number, a.Y / number);
        }
        public static Vector2 operator *(Vector2 a, float number)
        {
            return new Vector2(a.X * number, a.Y * number);
        }
        public bool Equals(Vector2 other)
        {
            return other.X == X && other.Y == Y;
        }
        public override bool Equals(object obj)
        {
            return obj is Vector2 && Equals((Vector2)obj);
        }
        public void SetValue(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        public override string ToString()
        {
            return "X : " + X + "; Y : " + Y;
        }
    }
}
