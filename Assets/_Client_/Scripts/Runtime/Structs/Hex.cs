using System;

namespace _Client_.Scripts.Runtime.Structs
{
    public readonly struct Hex
    {
        public Hex(int q, int r, int s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
            if (q + r + s != 0) throw new ArgumentException("q + r + s must be 0");
        }

        public readonly int q;
        public readonly int r;
        public readonly int s;

        public static bool operator ==(Hex a, Hex b) => a.q == b.q && a.r == b.r && a.s == b.s;
        public static bool operator !=(Hex a, Hex b) => !(a == b);

        public override bool Equals(object obj) => obj is Hex loc && Equals(loc);

        public bool Equals(Hex other) => q == other.q && r == other.r && s == other.s;

        public override int GetHashCode()
        {
            int hash = 17;
            hash = hash * 23 + q.GetHashCode();
            hash = hash * 23 + r.GetHashCode();
            hash = hash * 23 + s.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return $"({q},{r},{s})";
        }
    }
}