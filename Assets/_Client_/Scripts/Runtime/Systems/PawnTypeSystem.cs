using _Client_.Scripts.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Client_.Scripts.Runtime.Systems
{
    public class PawnTypeSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Pawn>> filter;

        public void Run(IEcsSystems systems)
        {
            foreach (var i in filter.Value)
            {
                ref var pawn = ref filter.Pools.Inc1.Get(i);
                if (string.IsNullOrWhiteSpace(pawn.Type.Sprite))
                {
                    pawn.TypeSpriteRenderer.sprite = null;
                    continue;
                }

                var op = Addressables.LoadAssetAsync<Sprite>(pawn.Type.Sprite);
                pawn.TypeSpriteRenderer.sprite = op.WaitForCompletion();
            }
        }
    }
}