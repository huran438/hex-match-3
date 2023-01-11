using _Client_.Scripts.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace _Client_.Scripts.Runtime.Systems
{
    public class BoardCleanupSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Board>> _filter;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var board = ref _filter.Pools.Inc1.Get(entity);
                board.Updated = false;
            }
        }
    }
}