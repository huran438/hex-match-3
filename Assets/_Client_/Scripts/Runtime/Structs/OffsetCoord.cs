using System;

namespace _Client_.Scripts.Runtime.Structs
{
    public struct OffsetCoord
    {
        public OffsetCoord(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
        
        public readonly int col;
        public readonly int row;
        public static int EVEN = 1;
        public static int ODD = -1;

        public static OffsetCoord QoffsetFromCube(int offset, Hex h)
        {
            var col = h.q;
            var row = h.r + (int)((h.q + offset * (h.q & 1)) / 2);
            if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }
            return new OffsetCoord(col, row);
        }


        public static Hex QoffsetToCube(int offset, OffsetCoord h)
        {
            var q = h.col;
            var r = h.row - (int)((h.col + offset * (h.col & 1)) / 2);
            var s = -q - r;
            if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }
            return new Hex(q, r, s);
        }


        public static OffsetCoord RoffsetFromCube(int offset, Hex h)
        {
            var col = h.q + (int)((h.r + offset * (h.r & 1)) / 2);
            var row = h.r;
            if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }
            return new OffsetCoord(col, row);
        }


        public static Hex RoffsetToCube(int offset, OffsetCoord h)
        {
            var q = h.col - (int)((h.row + offset * (h.row & 1)) / 2);
            var r = h.row;
            var s = -q - r;
            if (offset != OffsetCoord.EVEN && offset != OffsetCoord.ODD)
            {
                throw new ArgumentException("offset must be EVEN (+1) or ODD (-1)");
            }
            return new Hex(q, r, s);
        }
    }
}