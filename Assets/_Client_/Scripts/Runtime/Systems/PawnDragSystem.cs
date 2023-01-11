using _Client_.Scripts.Runtime.Components;
using _Client_.Scripts.Runtime.Enums;
using _Client_.Scripts.Runtime.Settings;
using _Client_.Scripts.Runtime.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SFramework.ECS.Runtime;
using SFramework.ECS.Runtime.Tween;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Systems
{
    public class PawnDragSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Pawn, TransformRef>> _pawnsFilter;
        private readonly EcsFilterInject<Inc<Pawn, TransformRef, Drag>> _draggedPawnsFilter;
        private readonly EcsFilterInject<Inc<Cell, TransformRef>> _cellsFilter;
        private readonly EcsCustomInject<GameSettings> gameSettingsInject;
        private readonly EcsWorldInject world;
        private readonly EcsPoolInject<MoveTo> _moveToPool;

        public void Run(IEcsSystems systems)
        {
            var gameSettings = gameSettingsInject.Value;

            if (world.Value.GetPool<RawInput>().GetRawDenseItemsCount() == 0) return;
            ref var rawInput = ref world.Value.GetPool<RawInput>().GetRawDenseItems()[0];
            var touchPosition = Camera.main.ScreenToWorldPoint(rawInput.Position);
            touchPosition.z = 0f;


            foreach (var i in _pawnsFilter.Value)
            {
                ref var pawnTransform = ref _pawnsFilter.Pools.Inc2.Get(i).reference;
                if (rawInput.State != RawInputState.Began) continue;
                if (!touchPosition.IsInsideCircle(pawnTransform.position, gameSettings.DragRadius)) continue;
                _draggedPawnsFilter.Pools.Inc3.Add(i);
                pawnTransform.localScale = gameSettings.DraggedPawnScale;
            }

            foreach (var i in _draggedPawnsFilter.Value)
            {
                ref var dragPawn = ref _draggedPawnsFilter.Pools.Inc1.Get(i);

                if (!dragPawn.CellEntity.Unpack(world.Value, out var dragPawnCellEntity)) continue;
                if (!_cellsFilter.Pools.Inc1.Has(dragPawnCellEntity)) continue;
                ref var dragPawnCell = ref _cellsFilter.Pools.Inc1.Get(dragPawnCellEntity);
                ref var dragPawnTransform = ref _draggedPawnsFilter.Pools.Inc2.Get(i).reference;
                if (rawInput.State != RawInputState.Moved) continue;
                dragPawnTransform.position = touchPosition;

                foreach (var j in _cellsFilter.Value)
                {
                    ref var cell = ref _cellsFilter.Pools.Inc1.Get(j);
                    ref var cellTransform = ref _cellsFilter.Pools.Inc2.Get(j).reference;

                    if (dragPawnTransform.position.IsInsideCircle(cellTransform.position, gameSettings.DragRadius))
                    {
                        if (cell.PawnEntity.HasValue)
                        {
                            if (cell.PawnEntity.Value.Unpack(world.Value, out var cellPawnEntity) && cellPawnEntity != i)
                            {
                                if (dragPawnCell.Hex.HasNeighbour(cell.Hex))
                                {
                                    ref var nearestPawnTransform = ref _pawnsFilter.Pools.Inc2.Get(cellPawnEntity).reference;

                                    _moveToPool.Value.Add(cellPawnEntity) = new MoveTo
                                    {
                                        Cooldown = 0,
                                        Duration = gameSettings.SwapDuration,
                                        LoopType = TweenLoopType.None,
                                        AnimationCurve = TweenAnimationCurve.EaseInOut,
                                        StartValue = nearestPawnTransform.position,
                                        EndValue = gameSettings.HexLayout.HexToPixel(dragPawnCell.Hex),
                                        UnscaledTime = false,
                                        ElapsedTime = 0
                                    };

                                    dragPawnCell.PawnEntity = cell.PawnEntity;

                                    _pawnsFilter.Pools.Inc1.Get(cellPawnEntity).CellEntity = dragPawn.CellEntity;
                                    dragPawn.CellEntity = world.Value.PackEntity(j);
                                    cell.PawnEntity = world.Value.PackEntity(i);
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            // End Drag
            foreach (var i in _draggedPawnsFilter.Value)
            {
                ref var dragPawnTransform = ref _draggedPawnsFilter.Pools.Inc2.Get(i).reference;
                if (rawInput.State != RawInputState.Ended) continue;

                dragPawnTransform.localScale = gameSettings.PawnScale;

                foreach (var j in _cellsFilter.Value)
                {
                    ref var cell = ref _cellsFilter.Pools.Inc1.Get(j);
                    ref var cellTransform = ref _cellsFilter.Pools.Inc2.Get(j).reference;
                    if (!cell.PawnEntity.HasValue) continue;
                    if (!cell.PawnEntity.Value.Unpack(world.Value, out var cellPawnEntity)) continue;
                    if (cellPawnEntity != i) continue;
                    dragPawnTransform.position = cellTransform.position;
                    break;
                }

                _draggedPawnsFilter.Pools.Inc3.Del(i);
            }
        }
    }
}