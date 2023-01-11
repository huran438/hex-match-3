using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Structs
{
    [Serializable]
    public class HexLayout
    {
        public Orientation Orientation => _orientationIsFlat ? Flat : Pointy;
        public Vector2 Size => _size;
        public Vector2 Origin => _origin;

        [SerializeField]
        private bool _orientationIsFlat = false;

        [SerializeField]
        private Vector2 _size = new Vector2(0.5f, 0.5f);
        
        [SerializeField]
        private Vector2 _origin = new Vector2(0.75f, 0f);
        

        public static Orientation Pointy = new Orientation
        (
            Mathf.Sqrt(3f),
            Mathf.Sqrt(3f) / 2f,
            0f,
            3f / 2f,
            Mathf.Sqrt(3f) / 3f,
            -1f / 3f,
            0f,
            2f / 3f,
            0.5f
        );

        public static Orientation Flat = new Orientation
        (
            3f / 2f,
            0f,
            Mathf.Sqrt(3f) / 2f,
            Mathf.Sqrt(3f),
            2f / 3f,
            0f,
            -1f / 3f,
            Mathf.Sqrt(3f) / 3f,
            0f
        );

        public Vector2 HexToPixel(Hex h)
        {
            var M = Orientation;
            var x = (M.f0 * h.q + M.f1 * h.r) * Size.x;
            var y = (M.f2 * h.q + M.f3 * h.r) * Size.y;
            return new Vector2(x + Origin.x, y + Origin.y);
        }


        public FractionalHex PixelToHex(Vector2 p)
        {
            var M = Orientation;
            var pt = new Vector2((p.x - Origin.x) / Size.x, (p.y - Origin.y) / Size.y);
            var q = M.b0 * pt.x + M.b1 * pt.y;
            var r = M.b2 * pt.x + M.b3 * pt.y;
            return new FractionalHex(q, r, -q - r);
        }


        public Vector2 HexCornerOffset(int corner)
        {
            var M = Orientation;
            var angle = 2f * Mathf.PI * (M.start_angle - corner) / 6f;
            return new Vector2(Size.x * Mathf.Cos(angle), Size.y * Mathf.Sin(angle));
        }

        public List<Vector2> PolygonCorners(Hex h)
        {
            var corners = new List<Vector2> { };
            var center = HexToPixel(h);
            for (var i = 0; i < 6; i++)
            {
                var offset = HexCornerOffset(i);
                corners.Add(new Vector2(center.x + offset.x, center.y + offset.y));
            }

            return corners;
        }
    }
}