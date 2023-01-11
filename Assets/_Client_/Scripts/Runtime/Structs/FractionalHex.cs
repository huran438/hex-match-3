using System;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Structs
{
    public readonly struct FractionalHex
    {
        public FractionalHex(float q, float r, float s)
        {
            this.q = q;
            this.r = r;
            this.s = s;
            if (Mathf.Round(q + r + s) != 0) throw new ArgumentException("q + r + s must be 0");
        }
        
        public readonly float q;
        public readonly float r;
        public readonly float s;
    }
}