using System;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Structs
{
    [Serializable]
    public struct ApplicationSettings
    {
        public RuntimePlatform Platform;
        public int TargetFrameRate;
        public SleepTimeout SleepTimeout;
    }
}