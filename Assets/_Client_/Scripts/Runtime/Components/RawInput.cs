using _Client_.Scripts.Runtime.Enums;
using SFramework.ECS.Runtime;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Components
{
    public struct RawInput : ISFComponent
    {
        public RawInputState State;
        public Vector2 Position;
    }
}