using _Client_.Scripts.Runtime.Components;
using _Client_.Scripts.Runtime.Settings;
using Leopotam.EcsLite;
using NCalc;
using SFramework.Core.Runtime;
using SFramework.ECS.Runtime;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Client_.Scripts.Runtime.Views
{
    [RequireComponent(typeof(UIDocument))]
    public class UIView : SFView
    {
        [SFInject]
        private ISFWorldsService _worldsService;

        [SFInject]
        private GameSettings _gameSettings;

        private EcsFilter _boardFilter;
        private EcsPool<Board> _boardPool;

        private ProgressBar progress_bar;
        private Expression pawnsExp, pawnsTypeExp;

        protected override void PreInit()
        {
            var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
            progress_bar = rootVisualElement.Q<ProgressBar>("progress_bar");
        }

        protected override void Init()
        {
            _boardFilter = _worldsService.Default.Filter<Board>().End();
            _boardPool = _worldsService.Default.GetPool<Board>();
            progress_bar.value = 0;
            pawnsExp = new Expression(_gameSettings.PawnsMatchingFormula);
            pawnsTypeExp = new Expression(_gameSettings.PawnsTypeMatchingFormula);
            pawnsExp.Parameters["COUNT"] = 1;
            pawnsTypeExp.Parameters["COUNT"] = 1;
        }

        private void Update()
        {
            var pawnsMatchResult = 0;
            var pawnTypesMatchResult = 0;

            foreach (var entity in _boardFilter)
            {
                ref var board = ref _boardPool.Get(entity);

                foreach (var boardChain in board.PawnsChains)
                {
                    if (boardChain.Count == 3)
                    {
                        pawnsMatchResult += 1;
                    }

                    if (boardChain.Count >= 4)
                    {
                        pawnsExp.Parameters["COUNT"] = boardChain.Count;

                        if (pawnsExp.Evaluate(out int r))
                        {
                            pawnsMatchResult += r;
                        }
                    }
                }

                foreach (var boardChain in board.PawnTypesChains)
                {
                    if (boardChain.Count == 2)
                    {
                        pawnTypesMatchResult += 2;
                    }

                    if (boardChain.Count >= 3)
                    {
                        pawnsExp.Parameters["COUNT"] = boardChain.Count;

                        if (pawnsTypeExp.Evaluate(out int r))
                        {
                            pawnTypesMatchResult += r;
                        }
                    }
                }
            }


            progress_bar.title =
                $"{Mathf.Round(pawnsMatchResult + pawnTypesMatchResult)}/{_gameSettings.MaxMoraleAmount}";
            var result = (pawnsMatchResult + pawnTypesMatchResult) / _gameSettings.MaxMoraleAmount;

            progress_bar.value = result;
        }
    }
}