using _Client_.Scripts.Runtime.Components;
using _Client_.Scripts.Runtime.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _Client_.Scripts.Runtime.Systems
{
    public class CellHighlightSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Cell, SpriteRendererRef>> _filter;
        private readonly EcsFilterInject<Inc<Board>> _boardFilter;
        private readonly EcsPoolInject<Pawn> _pawnPool;
        private EcsWorldInject world;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var cell = ref _filter.Pools.Inc1.Get(entity);
                ref var spriteRenderer = ref _filter.Pools.Inc2.Get(entity).SpriteRenderer;

                if (!cell.PawnEntity.HasValue)
                {
                    spriteRenderer.color = spriteRenderer.color.A(0f);
                }
                else
                {
                    spriteRenderer.color = spriteRenderer.color.A(1f);

                    if (!_boardFilter.Pools.Inc1.Has(entity)) continue;
                    ref var board = ref _boardFilter.Pools.Inc1.Get(entity);

                    if (cell.PawnEntity.Value.Unpack(world.Value, out var pawnEntity))
                    {
                        ref var pawn = ref _pawnPool.Value.Get(pawnEntity);

                        if (!string.IsNullOrWhiteSpace(pawn.Type.Sprite))
                        {
                            foreach (var pawnTypesChain in board.PawnTypesChains)
                            {
                                if (pawnTypesChain.Count > 1 && pawnTypesChain.Contains(world.Value.PackEntity(entity)))
                                {
                                    if (string.IsNullOrWhiteSpace(pawn.Type.BackgroundSprite))
                                    {
                                        cell.BackgroundSpriteRenderer.sprite = null;
                                        break;
                                    }

                                    var op = Addressables.LoadAssetAsync<Sprite>(pawn.Type.BackgroundSprite);
                                    cell.BackgroundSpriteRenderer.sprite = op.WaitForCompletion();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            cell.BackgroundSpriteRenderer.sprite = null;
                        }
                    }
                }
            }
        }
    }
}