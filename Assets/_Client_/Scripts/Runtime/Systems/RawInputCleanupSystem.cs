using _Client_.Scripts.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace _Client_.Scripts.Runtime.Systems
{
    public class RawInputCleanupSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<RawInput>> rawInputFilter;

        public void Run(IEcsSystems systems)
        {
            foreach (var i in rawInputFilter.Value)
            {
                rawInputFilter.Pools.Inc1.Del(i);
            }
        }
    }
}