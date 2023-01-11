using System.Collections.Generic;
using _Client_.Scripts.Runtime.Components;
using _Client_.Scripts.Runtime.Settings;
using _Client_.Scripts.Runtime.Structs;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SFramework.Core.Runtime;

namespace _Client_.Scripts.Runtime.Systems
{
    public class CellsGenerationSystem : IEcsInitSystem
    {
        private readonly EcsWorldInject world;
        private readonly EcsCustomInject<GameSettings> gameSettingsInject;
        private readonly EcsPoolInject<Board> _boardPool;
        private readonly EcsPoolInject<Cell> _cellPool;
        public void Init(IEcsSystems systems)
        {
            var gameSettings = gameSettingsInject.Value;
            var boardEntity = world.Value.NewEntity();
            ref var board = ref _boardPool.Value.Add(boardEntity);

            board.Map = new HashSet<EcsPackedEntity?>();
            board.HexEntities = new Dictionary<Hex, EcsPackedEntity>();
            board.PawnsChains = new List<List<EcsPackedEntity>>();
            board.PawnTypesChains = new List<List<EcsPackedEntity>>();
            board.Pawns = new HashSet<EcsPackedEntity>();
            
            for (var r = gameSettings.GridRect.Top; r <= gameSettings.GridRect.Bottom; r++)
            {
                var r_offset = r >> 1;
                for (var q = gameSettings.GridRect.Left - r_offset; q <= gameSettings.GridRect.Right - r_offset; q++)
                {
                    var hex = new Hex(q, r, -q - r);
                    var cellPackedEntity = gameSettings.CellPrefab.SFInstantiate(gameSettings.HexLayout.HexToPixel(hex)).EcsPackedEntity;
                    if (!cellPackedEntity.Unpack(world.Value, out var cellEntity)) continue;
                    ref var cell = ref _cellPool.Value.Get(cellEntity);
                    cell.Hex = hex;
                    board.Map.Add(cellPackedEntity);
                    board.HexEntities[hex] = cellPackedEntity;
                }
            }

            board.Updated = true;
        }
    }
}