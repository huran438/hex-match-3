using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SFramework.ECS.Runtime;
using SFramework.ECS.Runtime.Tween;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Systems
{
    public class MoveToSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<MoveTo, TransformRef>> _filter = null;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _filter.Value)
            {
                ref var moveTo = ref _filter.Pools.Inc1.Get(entity);
                ref var transform = ref _filter.Pools.Inc2.Get(entity).reference;

                moveTo.ElapsedTime += moveTo.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                var t = Mathf.Clamp((moveTo.ElapsedTime - moveTo.Cooldown) / moveTo.Duration, 0.0f, 1.0f);


                transform.position = SFMathFXHelper.CurvedValueECS(moveTo.AnimationCurve,
                    moveTo.StartValue,
                    moveTo.EndValue, t);

                if (moveTo.ElapsedTime >= moveTo.Cooldown + moveTo.Duration)
                {
                    switch (moveTo.LoopType)
                    {
                        case TweenLoopType.None:
                            transform.position = moveTo.EndValue;
                            _filter.Pools.Inc1.Del(entity);
                            break;
                        case TweenLoopType.Repeat:
                            moveTo.ElapsedTime -= moveTo.Duration;
                            break;
                        case TweenLoopType.Continuous:
                            var next = moveTo.EndValue - moveTo.StartValue;
                            moveTo.StartValue = moveTo.EndValue;
                            moveTo.EndValue = moveTo.EndValue + next;
                            moveTo.ElapsedTime = 0f;
                            break;
                        case TweenLoopType.YoYo:
                            (moveTo.StartValue, moveTo.EndValue) = (moveTo.EndValue, moveTo.StartValue);
                            moveTo.ElapsedTime = 0f;
                            break;
                    }
                }
            }
        }
    }
}