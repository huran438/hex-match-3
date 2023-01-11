using _Client_.Scripts.Runtime.Enums;
using Leopotam.EcsLite;
using SFramework.ECS.Runtime;

namespace _Client_.Scripts.Runtime.Components
{
    public struct Action : ISFComponent
    {
        public ActionType Type;
        public EcsPackedEntity[] CellChain;
        public EcsPackedEntity[] UnitsChain;
    }
}