using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using SFramework.ECS.Runtime;
using SFramework.ECS.Runtime.Tween;
using UnityEngine;

namespace _Client_.Scripts.Runtime.Systems
{
    public class RotateToSystem : IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<RotateTo, TransformRef>> _filter = null;

        private EcsWorld world;

        public void Run(IEcsSystems systems)
        {
            foreach (var i in _filter.Value)
            {
                ref var rotateTo = ref _filter.Pools.Inc1.Get(i);
                ref var transform = ref _filter.Pools.Inc2.Get(i).reference;

                rotateTo.ElapsedTime += rotateTo.UnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                var t = Mathf.Clamp((rotateTo.ElapsedTime - rotateTo.Cooldown) / rotateTo.Duration, 0.0f, 1.0f);


                transform.eulerAngles = SFMathFXHelper.CurvedValueECS(rotateTo.AnimationCurve,
                    rotateTo.StartValue,
                    rotateTo.EndValue, t);

                if (rotateTo.ElapsedTime >= rotateTo.Cooldown + rotateTo.Duration)
                {
                    switch (rotateTo.LoopType)
                    {
                        case TweenLoopType.None:
                            transform.eulerAngles = rotateTo.EndValue;
                            _filter.Pools.Inc1.Del(i);
                            break;
                        case TweenLoopType.Repeat:
                            rotateTo.ElapsedTime -= rotateTo.Duration;
                            break;
                        case TweenLoopType.Continuous:
                            var next = rotateTo.EndValue - rotateTo.StartValue;
                            rotateTo.StartValue = rotateTo.EndValue;
                            rotateTo.EndValue = rotateTo.EndValue + next;
                            rotateTo.ElapsedTime = 0f;
                            break;
                        case TweenLoopType.YoYo:
                            (rotateTo.StartValue, rotateTo.EndValue) = (rotateTo.EndValue, rotateTo.StartValue);
                            rotateTo.ElapsedTime = 0f;
                            break;
                    }
                }
            }
        }
    }
}