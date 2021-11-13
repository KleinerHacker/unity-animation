using System;
using System.Collections;
using System.Linq;
using UnityAnimation.Runtime.animation.Scripts.Runtime.Types;
using UnityEngine;

namespace UnityAnimation.Runtime.animation.Scripts.Runtime.Utils
{
    internal static class AnimationUtils
    {
        public static IEnumerator RunAnimation(AnimationCurve curve, float speed, Action<float, AnimationData> handler, AnimationData data, Action<AnimationData> onFinished = null)
        {
            return RunAnimation(AnimationType.Scaled, curve, speed, handler, data, onFinished);
        }

        public static IEnumerator RunAnimation(float preDelay, AnimationCurve curve, float speed, Action<float, AnimationData> handler, AnimationData data, Action<AnimationData> onFinished)
        {
            return RunAnimation(AnimationType.Scaled, preDelay, new[] { curve }, speed, (values, data) => handler(values[0], data), data, onFinished);
        }

        public static IEnumerator RunAnimation(AnimationType type, AnimationCurve curve, float speed, Action<float, AnimationData> handler, AnimationData data, Action<AnimationData> onFinished = null)
        {
            return RunAnimation(type, 0f, new[] { curve }, speed, (values, data) => handler(values[0], data), data, onFinished);
        }

        public static IEnumerator RunAnimation(AnimationCurve[] curves, float speed, Action<float[], AnimationData> handler, AnimationData data, Action<AnimationData> onFinished = null)
        {
            return RunAnimation(AnimationType.Scaled, curves, speed, handler, data, onFinished);
        }

        public static IEnumerator RunAnimation(float preDelay, AnimationCurve[] curves, float speed, Action<float[], AnimationData> handler, AnimationData data, Action<AnimationData> onFinished)
        {
            return RunAnimation(AnimationType.Scaled, preDelay, curves, speed, handler, data, onFinished);
        }

        public static IEnumerator RunAnimation(AnimationType type, AnimationCurve[] curves, float speed, Action<float[], AnimationData> handler, AnimationData data, Action<AnimationData> onFinished = null)
        {
            return RunAnimation(type, 0f, curves, speed, handler, data, onFinished);
        }

        public static IEnumerator RunAnimation(AnimationType type, float preDelay, AnimationCurve[] curves, float speed, Action<float[], AnimationData> handler, AnimationData data, Action<AnimationData> onFinished)
        {
            if (preDelay > 0f)
            {
                switch (type)
                {
                    case AnimationType.Scaled:
                        yield return new WaitForSeconds(preDelay);
                        break;
                    case AnimationType.Unscaled:
                        yield return new WaitForSecondsRealtime(preDelay);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            for (var i = 0f; i < speed; i += GetDelta(type))
            {
                var values = curves.Select(x => x.Evaluate(i / speed)).ToArray();
                handler(values, data);

                yield return null;
            }

            var oneArray = new float[curves.Length];
            for (var i = 0; i < oneArray.Length; i++)
            {
                oneArray[i] = 1f;
            }

            handler(oneArray, data);
            onFinished?.Invoke(data);
        }

        public static IEnumerator RunAnimationConstant(float time, Action<float, AnimationData> handler, AnimationData data, Action<AnimationData> onFinished)
        {
            return RunAnimationConstant(AnimationType.Scaled, 0f, time, handler, data, onFinished);
        }

        public static IEnumerator RunAnimationConstant(float preDelay, float time, Action<float, AnimationData> handler, AnimationData data, Action<AnimationData> onFinished)
        {
            return RunAnimationConstant(AnimationType.Scaled, preDelay, time, handler, data, onFinished);
        }

        public static IEnumerator RunAnimationConstant(AnimationType type, float time, Action<float, AnimationData> handler, AnimationData data, Action<AnimationData> onFinished)
        {
            return RunAnimationConstant(type, 0f, time, handler, data, onFinished);
        }

        public static IEnumerator RunAnimationConstant(AnimationType type, float preDelay, float time, Action<float, AnimationData> handler, AnimationData data, Action<AnimationData> onFinished)
        {
            if (preDelay > 0f)
            {
                switch (type)
                {
                    case AnimationType.Scaled:
                        yield return new WaitForSeconds(preDelay);
                        break;
                    case AnimationType.Unscaled:
                        yield return new WaitForSecondsRealtime(preDelay);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            for (var i = 0f; i < time; i += GetDelta(type))
            {
                var value = i / time;
                handler(value, data);

                yield return null;
            }

            onFinished?.Invoke(data);
        }

        public static IEnumerator WaitAndRun(float preDelay, float postDelay, Action<AnimationData> preAction, AnimationData data, Action<AnimationData> postAction)
        {
            return WaitAndRun(preDelay, postDelay, AnimationType.Scaled, data, preAction, postAction);
        }

        public static IEnumerator WaitAndRun(float preDelay, float postDelay, AnimationType type, AnimationData data, Action<AnimationData> preAction, Action<AnimationData> postAction)
        {
            switch (type)
            {
                case AnimationType.Scaled:
                    yield return new WaitForSeconds(preDelay);
                    break;
                case AnimationType.Unscaled:
                    yield return new WaitForSecondsRealtime(preDelay);
                    break;
                default:
                    throw new NotImplementedException();
            }

            preAction.Invoke(data);

            switch (type)
            {
                case AnimationType.Scaled:
                    yield return new WaitForSeconds(postDelay);
                    break;
                case AnimationType.Unscaled:
                    yield return new WaitForSecondsRealtime(postDelay);
                    break;
                default:
                    throw new NotImplementedException();
            }

            postAction.Invoke(data);
        }

        public static IEnumerator WaitAndRun(float delay, AnimationData data, Action<AnimationData> onFinished)
        {
            return WaitAndRun(AnimationType.Scaled, delay, data, onFinished);
        }

        public static IEnumerator WaitAndRun(AnimationType type, float delay, AnimationData data, Action<AnimationData> onFinished)
        {
            switch (type)
            {
                case AnimationType.Scaled:
                    yield return new WaitForSeconds(delay);
                    break;
                case AnimationType.Unscaled:
                    yield return new WaitForSecondsRealtime(delay);
                    break;
                default:
                    throw new NotImplementedException();
            }

            onFinished.Invoke(data);
        }

        public static IEnumerator WaitAndRun(uint frames, AnimationData data, Action<AnimationData> onFinished)
        {
            for (var i = 0; i < frames; i++)
            {
                yield return null;
            }

            onFinished.Invoke(data);
        }

        public static IEnumerator Delegate(Func<IEnumerator> func)
        {
            return func.Invoke();
        }

        public static IEnumerator RunAll(float delay, AnimationData data, params Action<AnimationData>[] actions)
        {
            return RunAll(AnimationType.Scaled, delay, data, null, actions);
        }

        public static IEnumerator RunAll(AnimationType type, float delay, AnimationData data, Action<AnimationData> onFinished, params Action<AnimationData>[] actions)
        {
            foreach (var action in actions)
            {
                action.Invoke(data);
                switch (type)
                {
                    case AnimationType.Scaled:
                        yield return new WaitForSeconds(delay);
                        break;
                    case AnimationType.Unscaled:
                        yield return new WaitForSecondsRealtime(delay);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            onFinished?.Invoke(data);
        }

        public static IEnumerator RunAll(uint frames, AnimationData data, params Action<AnimationData>[] actions)
        {
            return RunAll(frames, data, null, actions);
        }

        public static IEnumerator RunAll(uint frames, AnimationData data, Action<AnimationData> onFinished, params Action<AnimationData>[] actions)
        {
            foreach (var action in actions)
            {
                action.Invoke(data);
                for (var i = 0; i < frames; i++)
                {
                    yield return null;
                }
            }

            onFinished?.Invoke(data);
        }

        public static IEnumerator RunAll(float delay, uint repeat, Action<int, AnimationData> handler, AnimationData data, AnimationType type = AnimationType.Scaled, Action<AnimationData> onFinished = null)
        {
            for (var i = 0; i < repeat; i++)
            {
                handler.Invoke(i, data);
                switch (type)
                {
                    case AnimationType.Scaled:
                        yield return new WaitForSeconds(delay);
                        break;
                    case AnimationType.Unscaled:
                        yield return new WaitForSecondsRealtime(delay);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            onFinished?.Invoke(data);
        }

        public static IEnumerator RunAll(uint frames, uint repeat, Action<int, AnimationData> handler, AnimationData data, Action<AnimationData> onFinished = null)
        {
            for (var i = 0; i < repeat; i++)
            {
                handler.Invoke(i, data);
                for (var j = 0; j < frames; j++)
                {
                    yield return null;
                }
            }

            onFinished?.Invoke(data);
        }

        private static float GetDelta(AnimationType type)
        {
            switch (type)
            {
                case AnimationType.Scaled:
                    return Time.deltaTime;
                case AnimationType.Unscaled:
                    return Time.unscaledDeltaTime;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}