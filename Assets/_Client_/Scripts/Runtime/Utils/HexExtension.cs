using System.Collections.Generic;
using System.Linq;
using _Client_.Scripts.Runtime.Structs;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Utils
{
    public static class HexExtension
    {
        public static List<Hex> Directions = new List<Hex>
        {
            new Hex(1, 0, -1),
            new Hex(1, -1, 0),
            new Hex(0, -1, 1),
            new Hex(-1, 0, 1),
            new Hex(-1, 1, 0),
            new Hex(0, 1, -1)
        };
        
        public static Hex Direction(this Hex a, int direction)
        {
            return Directions[Mathf.Clamp(direction, 0, 5)];
        }

        public static Hex Add(this Hex a, Hex b)
        {
            return new Hex(a.q + b.q, a.r + b.r, a.s + b.s);
        }

        public static Hex Subtract(this Hex a, Hex b)
        {
            return new Hex(a.q - b.q, a.r - b.r, a.s - b.s);
        }

        public static Hex Scale(this Hex a, int k)
        {
            return new Hex(a.q * k, a.r * k, a.s * k);
        }


        public static Hex RotateLeft(this Hex a)
        {
            return new Hex(-a.s, -a.q, -a.r);
        }


        public static Hex RotateRight(this Hex a)
        {
            return new Hex(-a.r, -a.s, -a.q);
        }

        public static Hex Neighbor(this Hex a, int direction)
        {
            return a.Add(a.Direction(direction));
        }

        public static Hex[] Neighbors(this Hex a)
        {
            var neighbors = new Hex[6];

            for (int i = 0; i < 6; i++)
            {
                neighbors[i] = a.Add(a.Direction(i));
            }

            return neighbors;
        }
        
        public static bool HasNeighbour(this Hex a, Hex b)
        {
            var neighbours = a.Neighbors();
            return neighbours.Contains(b);
        }

        public static List<Hex> Diagonals = new List<Hex>
        {
            new Hex(2, -1, -1),
            new Hex(1, -2, 1),
            new Hex(-1, -1, 2),
            new Hex(-2, 1, 1),
            new Hex(-1, 2, -1),
            new Hex(1, 1, -2)
        };

        public static Hex DiagonalNeighbor(this Hex a, int direction)
        {
            return a.Add(Diagonals[direction]);
        }

        public static int Length(this Hex a)
        {
            return (Mathf.Abs(a.q) + Mathf.Abs(a.r) + Mathf.Abs(a.s)) / 2;
        }

        public static int Distance(this Hex a, Hex b)
        {
            return a.Subtract(b).Length();
        }
        
        public  static List<Hex> GetRing(this Hex center, int radius)
        {
            var results = new List<Hex>();
            var hex = center.Add(center.Direction(4).Scale(radius));
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    results.Add(hex);
                    hex = hex.Neighbor(i);
                }
            }

            return results;
        }

        public static List<Hex> GetSpiral(this Hex center, int radius)
        {
            var results = new List<Hex> { center };

            for (int k = 1; k <= radius; k++)
            {
                results.AddRange(GetRing(center, k));
            }

            return results;
        }

        public static Hex Round(this FractionalHex a)
        {
            var qi = (int)(Mathf.Round(a.q));
            var ri = (int)(Mathf.Round(a.r));
            var si = (int)(Mathf.Round(a.s));
            var q_diff = Mathf.Abs(qi - a.q);
            var r_diff = Mathf.Abs(ri - a.r);
            var s_diff = Mathf.Abs(si - a.s);
            if (q_diff > r_diff && q_diff > s_diff)
            {
                qi = -ri - si;
            }
            else if (r_diff > s_diff)
            {
                ri = -qi - si;
            }
            else
            {
                si = -qi - ri;
            }

            return new Hex(qi, ri, si);
        }

        public static FractionalHex HexLerp(this FractionalHex a, FractionalHex b, float t)
        {
            return new FractionalHex(a.q * (1f - t) + b.q * t, a.r * (1f - t) + b.r * t, a.s * (1f - t) + b.s * t);
        }
        
        public static List<Hex> Linedraw(Hex a, Hex b)
        {
            var N = a.Distance(b);
            var a_nudge = new FractionalHex(a.q + 1e-06f, a.r + 1e-06f, a.s - 2e-06f);
            var b_nudge = new FractionalHex(b.q + 1e-06f, b.r + 1e-06f, b.s - 2e-06f);
            var results = new List<Hex> { };
            var step = 1f / Mathf.Max(N, 1);
            for (var i = 0; i <= N; i++)
            {
                results.Add(a_nudge.HexLerp(b_nudge, step * i).Round());
            }

            return results;
        }
    }
}