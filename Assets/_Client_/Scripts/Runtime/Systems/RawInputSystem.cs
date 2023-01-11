using _Client_.Scripts.Runtime.Components;
using _Client_.Scripts.Runtime.Enums;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace _Client_.Scripts.Runtime.Systems
{
    public class RawInputSystem : IEcsInitSystem, IEcsRunSystem
    {
        private readonly EcsFilterInject<Inc<RawInput>> rawInputFilter;
        private readonly EcsWorldInject world;

        public void Init(IEcsSystems systems)
        {
            EnhancedTouchSupport.Enable();
        }

        public void Run(IEcsSystems systems)
        {
            if (rawInputFilter.Value.GetEntitiesCount() == 0)
            {
                rawInputFilter.Pools.Inc1.Add(world.Value.NewEntity());
            }
            
            ref var rawInput = ref rawInputFilter.Pools.Inc1.GetRawDenseItems()[0];
            
            if (Application.isMobilePlatform)
            {
                var activeTouches = Touch.activeTouches;
                
                if (activeTouches.Count == 0) return;

                var touch = activeTouches[0];
                
                switch (touch.phase)
                {
                    case UnityEngine.InputSystem.TouchPhase.Began:
                        Began(touch.screenPosition, ref rawInput);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Stationary:
                    case UnityEngine.InputSystem.TouchPhase.Moved:
                        Moved(touch.screenPosition, ref rawInput);
                        break;
                    case UnityEngine.InputSystem.TouchPhase.Canceled:
                    case UnityEngine.InputSystem.TouchPhase.Ended:
                        Ended(touch.screenPosition, ref rawInput);
                        break;
                    
                }
            }
            else if (Application.isConsolePlatform)
            {
            }
            else
            {
                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Began(Mouse.current.position.ReadValue(), ref rawInput);
                }
                else if (Mouse.current.leftButton.isPressed)
                {
                    Moved(Mouse.current.position.ReadValue(), ref rawInput);
                }
                else if (Mouse.current.leftButton.wasReleasedThisFrame)
                {
                    Ended(Mouse.current.position.ReadValue(), ref rawInput);
                }
            }
        }

        private void Began(Vector2 position, ref RawInput rawInput)
        {
            rawInput.State = RawInputState.Began;
            rawInput.Position = position;
        }

        private void Moved(Vector2 position, ref RawInput rawInput)
        {
            rawInput.State = RawInputState.Moved;
            rawInput.Position = position;
        }

        private void Ended(Vector2 position, ref RawInput rawInput)
        {
            rawInput.State = RawInputState.Ended;
            rawInput.Position = position;
        }
    }
}