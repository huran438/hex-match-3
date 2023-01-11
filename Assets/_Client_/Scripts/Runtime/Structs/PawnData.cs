using System;
using _Client_.Scripts.Runtime.Attributes;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Structs
{
    [Serializable]
    public struct PawnData
    {
        public Color DefaultColor => ColorUtility.TryParseHtmlString(defaultColor, out var color) ? color : Color.white;
        public Color ChainColor => ColorUtility.TryParseHtmlString(chainColor, out var color) ? color : Color.white;
        
        [ColorHex]
        [SerializeField]
        private string defaultColor;

        [ColorHex]
        [SerializeField]
        private string chainColor;
    }
}