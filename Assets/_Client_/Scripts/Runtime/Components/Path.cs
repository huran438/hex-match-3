using System.Collections.Generic;
using Leopotam.EcsLite;
using SFramework.ECS.Runtime;

namespace _Client_.Scripts.Runtime.Components
{
    public struct Path : ISFComponent
    {
        public List<EcsPackedEntity> CellEntities;
    }
}