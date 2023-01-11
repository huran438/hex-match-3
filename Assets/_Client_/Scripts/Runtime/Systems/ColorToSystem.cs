using _Client_.Scripts.Runtime.Components;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SFramework.ECS.Runtime.Tween;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Systems
{
    public class ColorToSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<ColorTo, SpriteRendererRef>> _filter = null;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var colorTo = ref _filter.Pools.Inc1.Get(entity);
                ref var spriteRenderer = ref _filter.Pools.Inc2.Get(entity).SpriteRenderer;

                colorTo.ElapsedTime += colorTo.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                var t = Mathf.Clamp((colorTo.ElapsedTime - colorTo.Cooldown) / colorTo.Duration, 0.0f, 1.0f);

                spriteRenderer.color = SFMathFXHelper.CurvedValueECS(colorTo.AnimationCurve, colorTo.StartValue, colorTo.EndValue, t);

                if (colorTo.ElapsedTime >= colorTo.Cooldown + colorTo.Duration)
                {
                    switch (colorTo.LoopType)
                    {
                        case TweenLoopType.None:
                            spriteRenderer.color = colorTo.EndValue;
                            _filter.Pools.Inc1.Del(entity);
                            break;
                        case TweenLoopType.Repeat:
                            colorTo.ElapsedTime -= colorTo.Duration;
                            break;
                        case TweenLoopType.Continuous:
                            var next = colorTo.EndValue - colorTo.StartValue;
                            colorTo.StartValue = colorTo.EndValue;
                            colorTo.EndValue += next;
                            colorTo.ElapsedTime = 0f;
                            break;
                        case TweenLoopType.YoYo:
                            (colorTo.StartValue, colorTo.EndValue) = (colorTo.EndValue, colorTo.StartValue);
                            colorTo.ElapsedTime = 0f;
                            break;
                    }
                }
            }
        }
    }
}