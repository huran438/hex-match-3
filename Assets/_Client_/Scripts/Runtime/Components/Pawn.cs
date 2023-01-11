using System;
using _Client_.Scripts.Runtime.Settings;
using _Client_.Scripts.Runtime.Structs;
using Leopotam.EcsLite;
using SFramework.ECS.Runtime;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Components
{
    [SFGenerateComponent]
    [Serializable]
    public struct Pawn : ISFComponent
    {
        public SpriteRenderer TypeSpriteRenderer;
        
        public EcsPackedEntity CellEntity;
        
        [HideInInspector]
        public PawnData Data;
        
        [HideInInspector]
        public PawnType Type;
        
        [HideInInspector]
        public bool MustHighlight;
        
        [HideInInspector]
        public bool IsHighlighted;
    }
}