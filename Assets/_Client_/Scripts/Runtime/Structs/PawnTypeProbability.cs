using System;
using _Client_.Scripts.Runtime.Settings;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Structs
{
    [Serializable]
    public struct PawnTypeProbability
    {
        public string Name;
        
        [Range(0f, 1f)]
        public float Probability;

        public PawnType PawnType;
    }
}