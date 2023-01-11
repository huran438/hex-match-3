using _Client_.Scripts.Runtime.Settings;
using _Client_.Scripts.Runtime.Systems;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SFramework.Core.Runtime;
using SFramework.ECS.Runtime;
using UnityEngine;

namespace _Client_.Scripts
{
    public sealed class ContextRoot : SFContextRoot
    {
        [SerializeField]
        private GameSettings _gameSettings;

        private EcsSystems _systems;
        private EcsWorld _world;

        protected override void PreInit()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        protected override void Setup(SFContainer container)
        {
            container.Bind(_gameSettings);
            var worldsService = new SFWorldsService();
            container.Bind<ISFWorldsService>(worldsService);
            _world = worldsService.Default;
            _systems = new EcsSystems(_world);
        }

        protected override void Init(ISFContainer container)
        {
            _systems
                .Add(new RotateToSystem())
                .Add(new MoveToSystem())
                .Add(new ColorToSystem())
                .Add(new CellsGenerationSystem())
                .Add(new PawnsGenerationSystem())
                .Add(new RawInputSystem())
                .Add(new PawnDragSystem())
                .Add(new PawnsMatchingSystem())
                // .Add(new PawnTypesMatchingSystem())
                .Add(new CellHighlightSystem())
                .Add(new PawnHighlightSystem())
                .Add(new PawnTypeSystem())
                .Add(new RawInputCleanupSystem())
                .Add(new BoardCleanupSystem())
                ;
            
            _systems.Inject(_gameSettings, container);
            _systems.Init();
            

        }

        private void Update()
        {
            _systems.Run();
        }

        private void OnDestroy()
        {
            _systems?.Destroy();
            _systems = null;
        }
    }
}