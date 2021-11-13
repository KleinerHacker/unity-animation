using System;
using System.Collections;
using NUnit.Framework;
using UnityAnimation.Runtime.animation.Scripts.Runtime.Utils;
using UnityEngine;
using UnityEngine.TestTools;

namespace UnityAnimation.Test.animation.Scripts.Test
{
    public class AnimationTest
    {
        [UnityTest]
        [Timeout(2000)]
        public IEnumerator TestWait()
        {
            yield return new MonoBehaviourTest<TestWaitBehavior>();
        }
        
        [UnityTest]
        [Timeout(2000)]
        public IEnumerator TestAnimate()
        {
            yield return new MonoBehaviourTest<TestAnimateBehavior>();
        }
        
        [UnityTest]
        [Timeout(5000)]
        public IEnumerator TestMixedWithFinisher()
        {
            yield return new MonoBehaviourTest<TestMixedWithFinisherBehavior>();
        }
        
        [UnityTest]
        [Timeout(5000)]
        public IEnumerator TestMixedWithData()
        {
            yield return new MonoBehaviourTest<TestMixedWithDataBehavior>();
        }

        private sealed class TestWaitBehavior : MonoBehaviour, IMonoBehaviourTest
        {
            public bool IsTestFinished { get; private set; }

            private void OnEnable()
            {
                AnimationBuilder.Create(this)
                    .Wait(1f, () => IsTestFinished = true)
                    .Start();
            }
        }
        
        private sealed class TestAnimateBehavior : MonoBehaviour, IMonoBehaviourTest
        {
            public bool IsTestFinished { get; private set; }

            private void OnEnable()
            {
                var counter = 0f;
                AnimationBuilder.Create(this)
                    .Animate(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 1f,
                        v => counter = v, () => IsTestFinished = counter >= 1f)
                    .Start();
            }
        }
        
        private sealed class TestMixedWithFinisherBehavior : MonoBehaviour, IMonoBehaviourTest
        {
            public bool IsTestFinished => _waiterFinished && _animationFinished && _finisher;

            private bool _waiterFinished, _animationFinished, _finisher;

            private void OnEnable()
            {
                var counter = 0f;
                AnimationBuilder.Create(this)
                    .Wait(1f, () => _waiterFinished = true)
                    .Animate(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 1f,
                        v => counter = v, () => _animationFinished = counter >= 1f)
                    .WithFinisher(() => _finisher = true)
                    .Start();
            }
        }

        private sealed class TestMixedWithDataBehavior : MonoBehaviour, IMonoBehaviourTest
        {
            private const byte WaiterKey = 1;
            private const string WaiterData = "waiter";

            private const byte AnimationKey = 2;
            private const string AnimationValue = "animation";

            private const byte CounterKey = 99;

            public bool IsTestFinished => _waiterFinished && _animationFinished && _finisher;
            
            private bool _waiterFinished, _animationFinished, _finisher;

            private void OnEnable()
            {
                AnimationBuilder.Create(this)
                    .Wait(1f, data =>
                    {
                        data.Set(WaiterKey, WaiterData);
                        _waiterFinished = true;
                    })
                    .Animate(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 1f,
                        (v, data) => data.Set(CounterKey, v),
                        data =>
                        {
                            _animationFinished = data.Get<string>(WaiterKey) == WaiterData && data.Get<float>(CounterKey) >= 1f;
                            data.Set(AnimationKey, AnimationValue);
                        })
                    .WithFinisher(data => _finisher = data.Get<string>(WaiterKey) == WaiterData && data.Get<string>(AnimationKey) == AnimationValue)
                    .Start();
            }
        }
    }
}
