using System;
using UnityAnimation.Runtime.animation.Scripts.Runtime.Types;
using UnityEditor.PackageManager;
using UnityEngine;

namespace UnityAnimation.Runtime.animation.Scripts.Runtime.Utils
{
    public sealed partial class AnimationBuilder
    {
        private abstract class AnimationStep
        {
            public Action<AnimationData> OnFinished { get; }

            protected AnimationStep(Action<AnimationData> onFinished)
            {
                OnFinished = onFinished;
            }
        }

        private sealed class AnimateConstantAnimationStep : AnimationStep
        {
            public float Time { get; }
            public Action<float, AnimationData> Handler { get; }

            public AnimateConstantAnimationStep(float time, Action<float, AnimationData> handler, Action<AnimationData> onFinished) : base(onFinished)
            {
                Time = time;
                Handler = handler;
            }
        }

        private sealed class SubAnimationStep : AnimationStep
        {
            public Action<Action, AnimationData> RunSubAnimation { get; }

            public SubAnimationStep(Action<Action, AnimationData> runSubAnimation, Action<AnimationData> onFinished) : base(onFinished)
            {
                RunSubAnimation = runSubAnimation;
            }
        }

        private sealed class AnimateAnimationStep : AnimationStep
        {
            public AnimationCurve[] Curves { get; }
            public float Speed { get; }
            public Action<float[], AnimationData> Handler { get; }

            public AnimateAnimationStep(AnimationCurve curve, float speed, Action<float, AnimationData> handler, Action<AnimationData> onFinished) 
                : this(new []{curve}, speed, (values, data) => handler(values[0], data), onFinished)
            {
            }

            public AnimateAnimationStep(AnimationCurve[] curves, float speed, Action<float[], AnimationData> handler, Action<AnimationData> onFinished) : base(onFinished)
            {
                Curves = curves;
                Speed = speed;
                Handler = handler;
            }
        }

        private sealed class WaitSecondsAnimationStep : AnimationStep
        {
            public float Seconds { get; }

            public WaitSecondsAnimationStep(float seconds, Action<AnimationData> onFinished) : base(onFinished)
            {
                Seconds = seconds;
            }
        }
        
        private sealed class WaitFramesAnimationStep : AnimationStep
        {
            public uint Frames { get; }

            public WaitFramesAnimationStep(uint frames, Action<AnimationData> onFinished) : base(onFinished)
            {
                Frames = frames;
            }
        }

        private sealed class RunAllSecondsAnimationStep : AnimationStep
        {
            public float Seconds { get; }
            public Action<AnimationData>[] Actions { get; }

            public RunAllSecondsAnimationStep(float seconds, Action<AnimationData>[] actions, Action<AnimationData> onFinished) : base(onFinished)
            {
                Seconds = seconds;
                Actions = actions;
            }
        }
        
        private sealed class RunAllFramesAnimationStep : AnimationStep
        {
            public uint Frames { get; }
            public Action<AnimationData>[] Actions { get; }

            public RunAllFramesAnimationStep(uint frames, Action<AnimationData>[] actions, Action<AnimationData> onFinished) : base(onFinished)
            {
                Frames = frames;
                Actions = actions;
            }
        }
        
        private sealed class RunRepeatSecondsAnimationStep : AnimationStep
        {
            public float Seconds { get; }
            public uint RepeatCount { get; }
            public Action<int, AnimationData> Action { get; }

            public RunRepeatSecondsAnimationStep(float seconds, uint repeatCount, Action<int, AnimationData> action, Action<AnimationData> onFinished) : base(onFinished)
            {
                Seconds = seconds;
                RepeatCount = repeatCount;
                Action = action;
            }
        }
        
        private sealed class RunRepeatFramesAnimationStep : AnimationStep
        {
            public uint Frames { get; }
            public uint RepeatCount { get; }
            public Action<int, AnimationData> Action { get; }

            public RunRepeatFramesAnimationStep(uint frames, uint repeatCount, Action<int, AnimationData> action, Action<AnimationData> onFinished) : base(onFinished)
            {
                Frames = frames;
                RepeatCount = repeatCount;
                Action = action;
            }
        }

        private sealed class ParallelAnimationStep : AnimationStep
        {
            public Func<AnimationBuilder, AnimationBuilder> BuildAction { get; }

            public ParallelAnimationStep(Func<AnimationBuilder, AnimationBuilder> buildAction, Action<AnimationData> onFinished) : base(onFinished)
            {
                BuildAction = buildAction;
            }
        }

        private sealed class ImmediatelyStep : AnimationStep
        {
            public ImmediatelyStep(Action<AnimationData> onFinished) : base(onFinished)
            {
            }
        }
    }
}