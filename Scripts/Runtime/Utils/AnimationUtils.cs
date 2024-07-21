using System;
using System.Collections;
using System.Linq;
using UnityAnimation.Runtime.Projects.unity_animation.Scripts.Runtime.Types;
using UnityEngine;

namespace UnityAnimation.Runtime.Projects.unity_animation.Scripts.Runtime.Utils
{
    internal static class AnimationUtils
    {
        public static IEnumerator RunAnimation(AnimationType type, AnimationCurve[] curves, float speed, bool revert, Action<float[], AnimationData> handler, AnimationData data, Action<AnimationData> onFinished = null)
        {
            return RunAnimation(type, 0f, curves, speed, revert, handler, data, onFinished);
        }

        public static IEnumerator RunAnimation(AnimationType type, float preDelay, AnimationCurve[] curves, float speed, bool revert, Action<float[], AnimationData> handler, AnimationData data, Action<AnimationData> onFinished)
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
                var v = i / speed;
                var values = curves.Select(x => x.Evaluate(revert ? 1f - v : v)).ToArray();
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

            handler(1f, data);

            onFinished?.Invoke(data);
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

        public static IEnumerator RunAllSeconds(float delay, uint repeat, bool inverted, Action<int, AnimationData> handler, AnimationData data, AnimationType type = AnimationType.Scaled, Action<AnimationData> onFinished = null)
        {
            if (inverted)
            {
                for (var i = (int)(repeat - 1); i >= 0; i--)
                {
                    handler.Invoke(i, data);
                    yield return type switch
                    {
                        AnimationType.Scaled => new WaitForSeconds(delay),
                        AnimationType.Unscaled => new WaitForSecondsRealtime(delay),
                        _ => throw new NotImplementedException()
                    };
                }
            }
            else
            {
                for (var i = 0; i < repeat; i++)
                {
                    handler.Invoke(i, data);
                    yield return type switch
                    {
                        AnimationType.Scaled => new WaitForSeconds(delay),
                        AnimationType.Unscaled => new WaitForSecondsRealtime(delay),
                        _ => throw new NotImplementedException()
                    };
                }
            }

            onFinished?.Invoke(data);
        }

        public static IEnumerator RunAllFrames(uint frames, uint repeat, bool inverted, Action<int, AnimationData> handler, AnimationData data, Action<AnimationData> onFinished = null)
        {
            if (inverted)
            {
                for (var i = (int)(repeat - 1); i >= 0; i--)
                {
                    handler.Invoke(i, data);
                    for (var j = 0; j < frames; j++)
                    {
                        yield return null;
                    }
                }
            }
            else
            {
                for (var i = 0; i < repeat; i++)
                {
                    handler.Invoke(i, data);
                    for (var j = 0; j < frames; j++)
                    {
                        yield return null;
                    }
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