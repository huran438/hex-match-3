using System.Collections.Generic;
using _Client_.Scripts.Runtime.Components;
using _Client_.Scripts.Runtime.Enums;
using _Client_.Scripts.Runtime.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace _Client_.Scripts.Runtime.Systems
{
    public class PawnsMatchingSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<RawInput>> rawInputFilter;
        private readonly EcsFilterInject<Inc<Board>> _boardFilter;
        private readonly EcsPoolInject<Cell> _cellPool;
        private readonly EcsPoolInject<Pawn> _pawnPool;
        private EcsWorldInject world;

        public void Init(IEcsSystems systems)
        {
            foreach (var i in _boardFilter.Value)
            {
                ref var board = ref _boardFilter.Pools.Inc1.Get(i);
                Match(ref board);
            }
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var i in _boardFilter.Value)
            {
                ref var board = ref _boardFilter.Pools.Inc1.Get(i);

                if (rawInputFilter.Value.GetEntitiesCount() != 0)
                {
                    ref var rawInput = ref rawInputFilter.Pools.Inc1.GetRawDenseItems()[0];

                    if (rawInput.State == RawInputState.Ended)
                    {
                        Match(ref board);
                        return;
                    }
                }

                if (board.Updated)
                {
                    Match(ref board);
                }
            }
        }

        private void Match(ref Board board)
        {
            var result = new List<List<EcsPackedEntity>>();

            foreach (var cellPackedEntity in board.Map)
            {
                if (!cellPackedEntity.HasValue) continue;
                if (!cellPackedEntity.Value.Unpack(world.Value, out var cellEntity)) continue;
                ref var cell = ref _cellPool.Value.Get(cellEntity);
                if (cell.Visited || !cell.PawnEntity.HasValue) continue;
                var chain = FindChain(ref board, cellPackedEntity.Value);
                if (chain.Count > 2)
                {
                    result.Add(chain);
                }
            }


            board.PawnsChains = result;

            foreach (var cellPackedEntity in board.Map)
            {
                if (!cellPackedEntity.HasValue) continue;
                if (!cellPackedEntity.Value.Unpack(world.Value, out var cellEntity)) continue;
                ref var cell = ref _cellPool.Value.Get(cellEntity);
                ref var pawnPackedEntity = ref cell.PawnEntity;
                if (!pawnPackedEntity.HasValue) continue;
                if (!pawnPackedEntity.Value.Unpack(world.Value, out var pawnEntity)) continue;
                ref var pawn = ref _pawnPool.Value.Get(pawnEntity);
                pawn.MustHighlight = false;
            }

            foreach (var t in result)
            {
                foreach (var cellPackedEntity in t)
                {
                    if (!cellPackedEntity.Unpack(world.Value, out var cellEntity)) continue;
                    ref var cell = ref _cellPool.Value.Get(cellEntity);
                    if (!cell.PawnEntity.HasValue) continue;
                    if (!cell.PawnEntity.Value.Unpack(world.Value, out var pawnEntity)) continue;

                    ref var pawn = ref _pawnPool.Value.Get(pawnEntity);
                    pawn.MustHighlight = true;
                }
            }

            foreach (var cellPackedEntity in board.Map)
            {
                if (!cellPackedEntity.HasValue) continue;
                if (!cellPackedEntity.Value.Unpack(world.Value, out var cellEntity)) continue;
                ref var cell = ref _cellPool.Value.Get(cellEntity);
                cell.Visited = false;
            }
        }

        private List<EcsPackedEntity> FindChain(ref Board board, EcsPackedEntity packedEntity)
        {
            if (!packedEntity.Unpack(world.Value, out var entity)) return new List<EcsPackedEntity>(0);
            ref var rootCell = ref _cellPool.Value.Get(entity);
            if(!rootCell.PawnEntity.HasValue) return new List<EcsPackedEntity>(0);
            if(!rootCell.PawnEntity.Value.Unpack(world.Value, out var rootCellPawnEntity)) return new List<EcsPackedEntity>(0);
            ref var rootPawn = ref _pawnPool.Value.Get(rootCellPawnEntity);
            var chain = new List<EcsPackedEntity>();

            chain.Add(packedEntity);
            rootCell.Visited = true;

            for (int i = 0; i < 6; i++)
            {
                var neighborCellHex = rootCell.Hex.Neighbor(i);

                if (!board.HexEntities.ContainsKey(neighborCellHex)) continue;

                var neighborCellPackedEntity = board.HexEntities[neighborCellHex];
                if (!neighborCellPackedEntity.Unpack(world.Value, out var neighborCellEntity)) continue;
                ref var neighborCell = ref _cellPool.Value.Get(neighborCellEntity);

                if (neighborCell.Visited) continue;

                ref var neighborPackedEntity = ref neighborCell.PawnEntity;
                if (!neighborPackedEntity.HasValue) continue;
                if (!neighborPackedEntity.Value.Unpack(world.Value, out var neighborEntity)) continue;
                ref var neighborPawn = ref  _pawnPool.Value.Get(neighborEntity);

                if (neighborPawn.Data.DefaultColor == rootPawn.Data.DefaultColor)
                {
                    PopulateGroup(ref board, ref chain, neighborCellPackedEntity);
                }
            }

            return chain;
        }

        private void PopulateGroup(ref Board board, ref List<EcsPackedEntity> chain, EcsPackedEntity parentCellPackedEntity)
        {
            if (!parentCellPackedEntity.Unpack(world.Value, out var parentCellEntity)) return;
            ref var parentCell = ref _cellPool.Value.Get(parentCellEntity);
            if (!parentCell.PawnEntity.HasValue) return;
            if (!parentCell.PawnEntity.Value.Unpack(world.Value, out var parentCellPawnEntity)) return;
            ref var parentPawn = ref _pawnPool.Value.Get(parentCellPawnEntity);

            chain.Add(parentCellPackedEntity);
            parentCell.Visited = true;

            for (int i = 0; i < 6; i++)
            {
                var neighborCellHex = parentCell.Hex.Neighbor(i);
                ;
                if (!board.HexEntities.ContainsKey(neighborCellHex)) continue;

                var neighborCellPackedEntity = board.HexEntities[neighborCellHex];
                if (!neighborCellPackedEntity.Unpack(world.Value, out var neighborCellEntity)) continue;
                ref var neighborCell = ref _cellPool.Value.Get(neighborCellEntity);

                if (neighborCell.Visited) continue;

                ref var neighborPackedEntity = ref neighborCell.PawnEntity;
                if (!neighborPackedEntity.HasValue) continue;
                if (!neighborPackedEntity.Value.Unpack(world.Value, out var neighborEntity)) continue;
                ref var neighborPawn = ref  _pawnPool.Value.Get(neighborEntity);

                if (neighborPawn.Data.DefaultColor == parentPawn.Data.DefaultColor)
                {
                    PopulateGroup(ref board, ref chain, neighborCellPackedEntity);
                }
            }
        }
    }
}