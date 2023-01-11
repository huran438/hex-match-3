using _Client_.Scripts.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SFramework.ECS.Runtime.Tween;

namespace _Client_.Scripts.Runtime.Systems
{
    public class PawnHighlightSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<Pawn>> pawnFilter;
        private readonly EcsPoolInject<ColorTo> _colorToPool;


        public void Run(IEcsSystems systems)
        {
            foreach (var entity in pawnFilter.Value)
            {
                ref var pawn = ref pawnFilter.Pools.Inc1.Get(entity);

                if (pawn.MustHighlight && !pawn.IsHighlighted)
                {
                    _colorToPool.Value.Add(entity) = new ColorTo
                    {
                        Cooldown = 0,
                        Duration = 0.2f,
                        LoopType = TweenLoopType.None,
                        AnimationCurve = TweenAnimationCurve.EaseInOut,
                        StartValue = pawn.Data.DefaultColor,
                        EndValue = pawn.Data.ChainColor,
                        UnscaledTime = true,
                        ElapsedTime = 0
                    };

                    pawn.IsHighlighted = true;
                }

                if (!pawn.MustHighlight && pawn.IsHighlighted)
                {
                    _colorToPool.Value.Add(entity) = new ColorTo
                    {
                        Cooldown = 0,
                        Duration = 0.2f,
                        LoopType = TweenLoopType.None,
                        AnimationCurve = TweenAnimationCurve.EaseInOut,
                        StartValue = pawn.Data.ChainColor,
                        EndValue = pawn.Data.DefaultColor,
                        UnscaledTime = true,
                        ElapsedTime = 0
                    };

                    pawn.IsHighlighted = false;
                }
            }
        }
    }
}