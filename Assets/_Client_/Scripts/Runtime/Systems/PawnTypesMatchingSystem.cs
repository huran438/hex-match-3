// using System.Collections.Generic;
// using _Client_.Scripts.Runtime.Components;
// using _Client_.Scripts.Runtime.Enums;
// using _Client_.Scripts.Runtime.Utils;
// using Leopotam.EcsLite;
// using Leopotam.EcsLite.Di;
//
// namespace _Client_.Scripts.Runtime.Systems
// {
//     public class PawnTypesMatchingSystem : IEcsInitSystem, IEcsRunSystem
//     {
//         private readonly EcsFilterInject<Inc<RawInput>> rawInputFilter;
//         private readonly EcsFilterInject<Inc<Board>> _boardFilter;
//         private readonly EcsPoolInject<Cell> _cellPool;
//         private readonly EcsPoolInject<Pawn> _pawnPool;
//         private EcsWorldInject world;
//
//         public void Init(IEcsSystems systems)
//         {
//             foreach (var i in _boardFilter.Value)
//             {
//                 ref var board = ref _boardFilter.Pools.Inc1.Get(i);
//                 Match(ref board);
//             }
//         }
//
//         public void Run(IEcsSystems systems)
//         {
//             foreach (var i in _boardFilter.Value)
//             {
//                 ref var board = ref _boardFilter.Pools.Inc1.Get(i);
//
//                 if (rawInputFilter.Value.GetEntitiesCount() != 0)
//                 {
//                     ref var rawInput = ref rawInputFilter.Pools.Inc1.GetRawDenseItems()[0];
//
//                     if (rawInput.State == RawInputState.Ended)
//                     {
//                         Match(ref board);
//                         return;
//                     }
//                 }
//
//                 if (board.Updated)
//                 {
//                     Match(ref board);
//                 }
//             }
//         }
//
//         private void Match(ref Board board)
//         {
//             var result = new List<List<EcsPackedEntity>>();
//
//             foreach (var cellPackedEntity in board.Map)
//             {
//                 if(!cellPackedEntity.HasValue) continue;
//                 if (!cellPackedEntity.Value.Unpack(world.Value, out var cellEntity)) continue;
//                 ref var cell = ref _cellPool.Value.Get(cellEntity);
//                 if (cell.Visited || !cell.PawnEntity.HasValue) continue;
//                 var chain = FindChain(ref board, cellPackedEntity.Value);
//                 if (chain.Count > 1)
//                 {
//                     result.Add(chain);
//                 }
//             }
//
//             board.PawnTypesChains = result;
//
//             foreach (var cellPackedEntity in board.Map)
//             {
//                 if(!cellPackedEntity.HasValue) continue;
//                 if (!cellPackedEntity.Value.Unpack(world.Value, out var cellEntity)) continue;
//                 ref var cell = ref _cellPool.Value.Get(cellEntity);
//                 cell.Visited = false;
//             }
//         }
//
//         private List<EcsPackedEntity> FindChain(ref Board board, EcsPackedEntity packedEntity)
//         {
//             if (!packedEntity.Unpack(world.Value, out var entity)) return new List<EcsPackedEntity>(0);
//             ref var rootCell = ref _cellPool.Value.Get(entity);
//             ref var rootPawn = ref rootCell.PawnEntity.Get<Pawn>();
//             
//             
//             var chain = new List<EcsPackedEntity> { packedEntity };
//
//             rootCell.Visited = true;
//
//             for (int i = 0; i < 6; i++)
//             {
//                 var neighborCellHex = rootCell.Hex.Neighbor(i);
//                 ;
//
//                 if (!board.HexEntities.ContainsKey(neighborCellHex)) continue;
//
//                 var neighborCellEntity = board.HexEntities[neighborCellHex];
//                 ref var neighborCell = ref neighborCellEntity.Get<Cell>();
//
//                 if (neighborCell.Visited) continue;
//
//                 ref var neighborEntity = ref neighborCell.PawnEntity;
//
//                 if (neighborEntity.IsNull()) continue;
//
//                 ref var neighborPawn = ref neighborEntity.Get<Pawn>();
//
//                 if (!string.IsNullOrWhiteSpace(rootPawn.Type.Sprite) && neighborPawn.Type.Sprite == rootPawn.Type.Sprite)
//                 {
//                     PopulateGroup(ref board, ref chain, neighborCellEntity);
//                 }
//             }
//
//             return chain;
//         }
//
//         private void PopulateGroup(ref Board board, ref List<EcsPackedEntity> chain, EcsPackedEntity parentCellPackedEntity)
//         {
//             if (!parentCellPackedEntity.Unpack(world.Value, out var parentCellEntity)) return;
//             ref var parentCell = ref _cellPool.Value.Get(parentCellEntity); // parentCellEntity.Get<Cell>();
//             ref var parentPawn = ref parentCell.PawnEntity.Get<Pawn>();
//
//             chain.Add(parentCellPackedEntity);
//             parentCell.Visited = true;
//
//             for (int i = 0; i < 6; i++)
//             {
//                 var neighborCellHex = parentCell.Hex.Neighbor(i);
//                 ;
//                 if (!board.HexEntities.ContainsKey(neighborCellHex)) continue;
//
//                 var neighborCellEntity = board.HexEntities[neighborCellHex];
//                 ref var neighborCell = ref neighborCellEntity.Get<Cell>();
//
//                 if (neighborCell.Visited) continue;
//
//                 ref var neighborEntity = ref neighborCell.PawnEntity;
//                 if (neighborEntity.IsNull()) continue;
//                 ref var neighborPawn = ref neighborEntity.Get<Pawn>();
//
//                 if (!string.IsNullOrWhiteSpace(parentPawn.Type.Sprite) &&
//                     neighborPawn.Type.Sprite == parentPawn.Type.Sprite && neighborCell.Visited == false)
//                 {
//                     PopulateGroup(ref board, ref chain, neighborCellEntity);
//                 }
//             }
//         }
//     }
// }