using System;
using System.Collections.Generic;
using System.Linq;
using UnityAnimation.Runtime.animation.Scripts.Runtime.Types;
using UnityEngine;

namespace UnityAnimation.Runtime.animation.Scripts.Runtime.Utils
{
    public sealed partial class AnimationBuilder
    {
        public static AnimationBuilder Create(MonoBehaviour behaviour, AnimationType type = AnimationType.Scaled)
        {
            return new AnimationBuilder(behaviour, type);
        }

        private readonly MonoBehaviour _behaviour;
        private readonly AnimationType _type;
        private readonly IList<AnimationStep> _steps = new List<AnimationStep>();

        private Action<AnimationData> _onFinished;

        private AnimationBuilder(MonoBehaviour behaviour, AnimationType type)
        {
            _behaviour = behaviour;
            _type = type;
        }

        public AnimationBuilder SubAnimation(Action<Action> runSubAnimation) => SubAnimation(runSubAnimation, null);

        public AnimationBuilder SubAnimation(Action<Action> runSubAnimation, Action onFinished) => 
            SubAnimation((finisher, _) => runSubAnimation?.Invoke(finisher), _ => onFinished?.Invoke());

        public AnimationBuilder SubAnimation(Action<Action, AnimationData> runSubAnimation) => SubAnimation(runSubAnimation, null);

        public AnimationBuilder SubAnimation(Action<Action, AnimationData> runSubAnimation, Action<AnimationData> onFinished)
        {
            _steps.Add(new SubAnimationStep(runSubAnimation, onFinished));
            return this;
        }

        public AnimationBuilder Animate(AnimationCurve curve, float speed, Action<float> handler) => Animate(curve, speed, handler, null); 

        public AnimationBuilder Animate(AnimationCurve curve, float speed, Action<float> handler, Action onFinished) => 
            Animate(curve, speed, (v, _) => handler?.Invoke(v), _ => onFinished?.Invoke());

        public AnimationBuilder Animate(AnimationCurve curve, float speed, Action<float, AnimationData> handler) => Animate(curve, speed, handler, null);
        
        public AnimationBuilder Animate(AnimationCurve curve, float speed, Action<float, AnimationData> handler, Action<AnimationData> onFinished)
        {
            _steps.Add(new AnimateAnimationStep(curve, speed, handler, onFinished));
            return this;
        }

        public AnimationBuilder Animate(AnimationCurve curve, Action<float> handler) => Animate(curve, handler, null);

        public AnimationBuilder Animate(AnimationCurve curve, Action<float> handler, Action onFinished) => 
            Animate(curve, (v, _) => handler?.Invoke(v), _ => onFinished?.Invoke());

        public AnimationBuilder Animate(AnimationCurve curve, Action<float, AnimationData> handler) => Animate(curve, handler, null);

        public AnimationBuilder Animate(AnimationCurve curve, Action<float, AnimationData> handler, Action<AnimationData> onFinished) => 
            Animate(curve, 1f, handler, onFinished);

        public AnimationBuilder AnimateConstant(float time, Action<float> handler) => AnimateConstant(time, handler, null);

        public AnimationBuilder AnimateConstant(float time, Action<float> handler, Action onFinished) => 
            AnimateConstant(time, (v, _) => handler?.Invoke(v), _ => onFinished?.Invoke());

        public AnimationBuilder AnimateConstant(float time, Action<float, AnimationData> handler) => AnimateConstant(time, handler, null);

        public AnimationBuilder AnimateConstant(float time, Action<float, AnimationData> handler, Action<AnimationData> onFinished)
        {
            _steps.Add(new AnimateConstantAnimationStep(time, handler, onFinished));
            return this;
        }

        public AnimationBuilder Wait(float seconds) => Wait(seconds, (Action) null);

        public AnimationBuilder Wait(float seconds, Action onFinished) => Wait(seconds, _ => onFinished?.Invoke());

        public AnimationBuilder Wait(float seconds, Action<AnimationData> onFinished)
        {
            _steps.Add(new WaitSecondsAnimationStep(seconds, onFinished));
            return this;
        }

        public AnimationBuilder Wait(uint frames) => Wait(frames, (Action)null);

        public AnimationBuilder Wait(uint frames, Action onFinished) => Wait(frames, _ => onFinished?.Invoke());

        public AnimationBuilder Wait(uint frames, Action<AnimationData> onFinished)
        {
            _steps.Add(new WaitFramesAnimationStep(frames, onFinished));
            return this;
        }

        public AnimationBuilder RunAll(float seconds, Action[] actions) => RunAll(seconds, actions, null);

        public AnimationBuilder RunAll(float seconds, Action[] actions, Action onFinished) => 
            RunAll(seconds, actions.Select(x => (Action<AnimationData>) (_ => x?.Invoke())).ToArray(), _ => onFinished?.Invoke());

        public AnimationBuilder RunAll(float seconds, Action<AnimationData>[] actions) => RunAll(seconds, actions, null);

        public AnimationBuilder RunAll(float seconds, Action<AnimationData>[] actions, Action<AnimationData> onFinished)
        {
            _steps.Add(new RunAllSecondsAnimationStep(seconds, actions, onFinished));
            return this;
        }

        public AnimationBuilder RunAll(uint frames, Action[] actions) => RunAll(frames, actions, null);

        public AnimationBuilder RunAll(uint frames, Action[] actions, Action onFinished) => 
            RunAll(frames, actions.Select(x => (Action<AnimationData>)(_ => x?.Invoke())).ToArray(), _ => onFinished?.Invoke());

        public AnimationBuilder RunAll(uint frames, Action<AnimationData>[] actions) => RunAll(frames, actions, null);

        public AnimationBuilder RunAll(uint frames, Action<AnimationData>[] actions, Action<AnimationData> onFinished)
        {
            _steps.Add(new RunAllFramesAnimationStep(frames, actions, onFinished));
            return this;
        }

        public AnimationBuilder RunRepeated(float seconds, uint repeatCount, Action<int> action) => RunRepeated(seconds, repeatCount, action, null);

        public AnimationBuilder RunRepeated(float seconds, uint repeatCount, Action<int> action, Action onFinished) => 
            RunRepeated(seconds, repeatCount, (i, _) => action?.Invoke(i), _ => onFinished?.Invoke());

        public AnimationBuilder RunRepeated(float seconds, uint repeatCount, Action<int, AnimationData> action) => RunRepeated(seconds, repeatCount, action, null);

        public AnimationBuilder RunRepeated(float seconds, uint repeatCount, Action<int, AnimationData> action, Action<AnimationData> onFinished)
        {
            _steps.Add(new RunRepeatSecondsAnimationStep(seconds, repeatCount, action, onFinished));
            return this;
        }

        public AnimationBuilder RunRepeated(uint frames, uint repeatCount, Action<int> action) => RunRepeated(frames, repeatCount, action, null);

        public AnimationBuilder RunRepeated(uint frames, uint repeatCount, Action<int> action, Action onFinished) => 
            RunRepeated(frames, repeatCount, (i, _) => action?.Invoke(i), _ => onFinished?.Invoke());

        public AnimationBuilder RunRepeated(uint frames, uint repeatCount, Action<int, AnimationData> action) => RunRepeated(frames, repeatCount, action, null);

        public AnimationBuilder RunRepeated(uint frames, uint repeatCount, Action<int, AnimationData> action, Action<AnimationData> onFinished)
        {
            _steps.Add(new RunRepeatFramesAnimationStep(frames, repeatCount, action, onFinished));
            return this;
        }

        /// <summary>
        /// <b>Experimental</b>
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public AnimationBuilder Parallel(Func<AnimationBuilder, AnimationBuilder> action) => Parallel(action, (Action) null);
        
        /// <summary>
        /// <b>Experimental</b>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public AnimationBuilder Parallel(Func<AnimationBuilder, AnimationBuilder> action, Action onFinished) => 
            Parallel(action, _ => onFinished?.Invoke());

        /// <summary>
        /// <b>Experimental</b>
        /// </summary>
        /// <param name="action"></param>
        /// <param name="onFinished"></param>
        /// <returns></returns>
        public AnimationBuilder Parallel(Func<AnimationBuilder, AnimationBuilder> action, Action<AnimationData> onFinished)
        {
            _steps.Add(new ParallelAnimationStep(action, onFinished));
            return this;
        }

        public AnimationBuilder Immediately(Action onFinished) => Immediately(_ => onFinished?.Invoke());

        public AnimationBuilder Immediately(Action<AnimationData> onFinished)
        {
            _steps.Add(new ImmediatelyStep(onFinished));
            return this;
        }

        public AnimationBuilder WithFinisher(Action onFinished) => WithFinisher(_ => onFinished?.Invoke());

        public AnimationBuilder WithFinisher(Action<AnimationData> onFinished)
        {
            _onFinished = onFinished;
            return this;
        }
    }
}