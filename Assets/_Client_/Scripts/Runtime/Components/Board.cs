using System.Collections.Generic;
using _Client_.Scripts.Runtime.Structs;
using Leopotam.EcsLite;
using SFramework.ECS.Runtime;

namespace _Client_.Scripts.Runtime.Components
{
    public struct Board : ISFComponent
    {
        public HashSet<EcsPackedEntity?> Map;
        public Dictionary<Hex, EcsPackedEntity> HexEntities;
        public HashSet<EcsPackedEntity> Pawns;
        public List<List<EcsPackedEntity>> PawnsChains;
        public List<List<EcsPackedEntity>> PawnTypesChains;
        public bool Updated;
    }
}