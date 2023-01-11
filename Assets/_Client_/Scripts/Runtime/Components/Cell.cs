using System;
using _Client_.Scripts.Runtime.Structs;
using Leopotam.EcsLite;
using SFramework.ECS.Runtime;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Components
{
   
    [SFGenerateComponent]
    [Serializable]
    public struct Cell : ISFComponent
    {
        public Hex Hex;
        public SpriteRenderer BackgroundSpriteRenderer;
        public EcsPackedEntity? PawnEntity;
        [HideInInspector]
        public bool Visited;
    }
}