using System.Collections.Generic;
using System.Linq;
using _Client_.Scripts.Runtime.Components;
using _Client_.Scripts.Runtime.Settings;
using _Client_.Scripts.Runtime.Structs;
using _Client_.Scripts.Runtime.Utils;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SFramework.Core.Runtime;
using SFramework.ECS.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Client_.Scripts.Runtime.Systems
{
    public class PawnsGenerationSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Board>> boardFilter;
        private readonly EcsCustomInject<GameSettings> gameSettingsInject;
        private readonly EcsWorldInject world;
        private Vector2Int centerOfGrid;
        private WeightedRandomBag<PawnType> wb;
        private float time;
        private readonly EcsPoolInject<Cell> _cellPool;
        private readonly EcsPoolInject<Pawn> _pawnPool;
        private readonly EcsPoolInject<GameObjectRef> _gameObjectRefPool;
        private readonly EcsPoolInject<SpriteRendererRef> _spriteRendererRefPool;


        public void Init(IEcsSystems systems)
        {
            var gameSettings = gameSettingsInject.Value;
            
            wb = new WeightedRandomBag<PawnType>();

            foreach (var pawnTypeProbability in gameSettings.PawnTypeProbabilities)
            {
                wb.AddEntry(pawnTypeProbability.PawnType, pawnTypeProbability.Probability);
            }

            foreach (var i in boardFilter.Value)
            {
                ref var board = ref boardFilter.Pools.Inc1.Get(i);

                var radius = Mathf.CeilToInt((-3 + Mathf.Sqrt(9 + 12 * gameSettings.InitialPawns)) / 6);

                var spiral = new Hex(0, 0, 0).GetSpiral(radius);

                foreach (var hex in spiral)
                {
                    if (board.Pawns.Count >= gameSettings.InitialPawns) break;

                    if (board.HexEntities.TryGetValue(hex, out EcsPackedEntity cellEntity))
                    {
                        CreatePawn(ref board, ref cellEntity);
                    }
                }
            }

            time = gameSettings.PawnGenerationTimeout;
        }

        public void Run(IEcsSystems systems)
        {
            var gameSettings = gameSettingsInject.Value;
            
            if (time > 0f)
            {
                time -= Time.deltaTime;
            }
            else
            {
                foreach (var i in boardFilter.Value)
                {
                    ref var board = ref boardFilter.Pools.Inc1.Get(i);

                    if (board.Pawns.Count == board.Map.Count) continue;

                    var nextCount = board.Pawns.Count + 1;

                    var radius = 1;
                    var maxRadius = Mathf.CeilToInt((-3 + Mathf.Sqrt(9 + 12 * board.Map.Count)) / 6);
                    var emptyHexes = new List<Hex>();

                    while (radius < maxRadius + 10) // temp solution
                    {
                        if (board.Pawns.Count >= nextCount) break;

                        if (emptyHexes.Count == 0)
                        {
                            var hexRing = new Hex(0, 0, 0).GetRing(radius);

                            foreach (var hex in hexRing)
                            {
                                if (!board.HexEntities.TryGetValue(hex, out EcsPackedEntity cellPackedEntity)) continue;
                                if(!cellPackedEntity.Unpack(world.Value, out var cellEntity)) continue;
                                ref var boardCell = ref _cellPool.Value.Get(cellEntity);
                                if (boardCell.PawnEntity.HasValue) continue;
                                emptyHexes.Add(hex);
                            }

                            radius++;
                        }
                        else
                        {
                            var randomHex = emptyHexes.First();
                            var cellEntity = board.HexEntities[randomHex];
                            CreatePawn(ref board, ref cellEntity);
                            time = gameSettings.PawnGenerationTimeout;
                        }
                    }
                }
            }
        }

        private void CreatePawn(ref Board board, ref EcsPackedEntity boardCellPackedEntity)
        {
            var gameSettings = gameSettingsInject.Value;
            
            if (boardCellPackedEntity.Unpack(world.Value, out var boardCellEntity))
            {
                ref var boardCell = ref _cellPool.Value.Get(boardCellEntity);
                var pawnData = gameSettings.PawnsData[Random.Range(0, gameSettings.PawnsData.Length)];
                var worldPosition = gameSettings.HexLayout.HexToPixel(boardCell.Hex);
                var pawnPackedEntity = gameSettings.PawnPrefab.SFInstantiate(worldPosition).EcsPackedEntity;

                if (pawnPackedEntity.Unpack(world.Value, out var pawnEntity))
                {
                    ref var pawn = ref _pawnPool.Value.Get(pawnEntity);
                    ref var spriteRenderer = ref _spriteRendererRefPool.Value.Get(pawnEntity).SpriteRenderer;
                    ref var gameObject = ref _gameObjectRefPool.Value.Get(pawnEntity).reference;
                    spriteRenderer.color = pawnData.DefaultColor;
                    gameObject.transform.localScale = gameSettings.PawnScale;
                    pawn.Data = pawnData;
                    pawn.MustHighlight = false;
                    pawn.CellEntity = boardCellPackedEntity;
                    pawn.Type = wb.GetRandom();
                    boardCell.PawnEntity = pawnPackedEntity;
                    board.Pawns.Add(pawnPackedEntity);
                    board.Updated = true;
                }
            }
        }
    }
}