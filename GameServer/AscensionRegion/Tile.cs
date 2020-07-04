using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AscensionRegion
{
    /// <summary>
    /// 瓦片单位
    /// </summary>
    public struct Tile : IEquatable<Tile>
    {
        public Tile(float x, float y) : this()
        {
            X = x;
            Y = y;
        }
        public float X { get; set; }
        public float Y { get; set; }
        public static bool operator ==(Tile a, Tile b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Tile a, Tile b)
        {
            return !(a == b);
        }
        public static Tile operator - (Tile a, Tile b)
        {
            return new Tile(a.X - b.X, a.Y - b.Y);
        }
        public static Tile operator +(Tile a, Tile b)
        {
            return new Tile(a.X + b.X, a.Y + b.Y);
        }
        public bool Equals(Tile other)
        {
            return other.X == X && other.Y == Y;
        }
        public override bool Equals(object obj)
        {
            return obj is Tile && Equals((Tile)obj);
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
