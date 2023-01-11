namespace _Client_.Scripts.Runtime.Structs
{
    public struct DoubledCoord
    {
        public DoubledCoord(int col, int row)
        {
            this.col = col;
            this.row = row;
        }
        public readonly int col;
        public readonly int row;

        public static DoubledCoord QdoubledFromCube(Hex h)
        {
            var col = h.q;
            var row = 2 * h.r + h.q;
            return new DoubledCoord(col, row);
        }


        public Hex QdoubledToCube()
        {
            var q = col;
            var r = (int)((row - col) / 2);
            var s = -q - r;
            return new Hex(q, r, s);
        }


        public static DoubledCoord RdoubledFromCube(Hex h)
        {
            var col = 2 * h.q + h.r;
            var row = h.r;
            return new DoubledCoord(col, row);
        }


        public Hex RdoubledToCube()
        {
            var q = (int)((col - row) / 2);
            var r = row;
            var s = -q - r;
            return new Hex(q, r, s);
        }
    }
}