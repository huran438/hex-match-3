using System;
using SFramework.ECS.Runtime;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Components
{
    [SFGenerateComponent]
    [Serializable]
    public struct SpriteRendererRef : ISFComponent
    {
        public SpriteRenderer SpriteRenderer;
    }
}