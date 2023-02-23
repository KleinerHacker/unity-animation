using System;
using System.Collections;
using UnityAnimation.Runtime.animation.Scripts.Runtime.Types;
using UnityEngine;

namespace UnityAnimation.Runtime.animation.Scripts.Runtime.Utils
{
    public sealed partial class AnimationBuilder
    {
        public bool IsRunning { get; private set; }

        public AnimationRunner Start(float delayed, Action<AnimationData> onFinished = null)
        {
            if (_steps.Count <= 0)
                throw new InvalidOperationException("No animation inside builder");
            if (IsRunning)
                throw new InvalidOperationException("Is already running");

            var animationRunner = new AnimationRunner(_behaviour);

            IsRunning = true;
            animationRunner.Coroutine = Run(AnimationUtils.WaitAndRun(delayed, new AnimationData(), data =>
            {
                onFinished?.Invoke(data);
                if (animationRunner.IsStopped)
                    return;

                StartNext(0, animationRunner, data);
            }));

            return animationRunner;
        }

        public AnimationRunner Start()
        {
            return Start(new AnimationData());
        }

        private AnimationRunner Start(AnimationData data)
        {
            if (_steps.Count <= 0)
                throw new InvalidOperationException("No animation inside builder");
            if (IsRunning)
                throw new InvalidOperationException("Is already running");

            var animationRunner = new AnimationRunner(_behaviour);

            IsRunning = true;
            StartNext(0, animationRunner, data);

            return animationRunner;
        }

        private void StartNext(int stepIndex, AnimationRunner animationRunner, AnimationData data)
        {
            if (stepIndex >= _steps.Count)
            {
                _onFinished?.Invoke(data);
                IsRunning = false;

                return;
            }

            var step = _steps[stepIndex];
            if (step is SubAnimationStep subAnimStep)
            {
                subAnimStep.RunSubAnimation.Invoke(() =>
                {
                    subAnimStep.OnFinished?.Invoke(data);
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner, data);
                }, data);
            }
            else if (step is AnimateAnimationStep animStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAnimation(_type, animStep.Curves, animStep.Speed, animStep.Revert, animStep.Handler, data, data =>
                {
                    animStep.OnFinished?.Invoke(data);
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner, data);
                }));
            }
            else if (step is AnimateConstantAnimationStep animConstStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAnimationConstant(_type, animConstStep.Time, animConstStep.Handler, data, data =>
                {
                    animConstStep.OnFinished?.Invoke(data);
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner, data);
                }));
            }
            else if (step is WaitSecondsAnimationStep waitSecStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.WaitAndRun(_type, waitSecStep.Seconds, data, data =>
                {
                    waitSecStep.OnFinished?.Invoke(data);
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner, data);
                }));
            }
            else if (step is WaitFramesAnimationStep waitFrameStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.WaitAndRun(waitFrameStep.Frames, data, data =>
                {
                    waitFrameStep.OnFinished?.Invoke(data);
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner, data);
                }));
            }
            else if (step is RunAllSecondsAnimationStep runAllSecStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAll(_type, runAllSecStep.Seconds, data, data =>
                {
                    runAllSecStep.OnFinished?.Invoke(data);
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner, data);
                }, runAllSecStep.Actions));
            }
            else if (step is RunAllFramesAnimationStep runAllFramesStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAll(_type, runAllFramesStep.Frames, data, data =>
                {
                    runAllFramesStep.OnFinished?.Invoke(data);
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner, data);
                }, runAllFramesStep.Actions));
            }
            else if (step is RunRepeatSecondsAnimationStep runRepeatSecStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAllSeconds(
                    runRepeatSecStep.Seconds, runRepeatSecStep.RepeatCount, runRepeatSecStep.Inverted, runRepeatSecStep.Action, data, _type, data =>
                    {
                        runRepeatSecStep.OnFinished?.Invoke(data);
                        if (animationRunner.IsStopped)
                            return;
                        StartNext(stepIndex + 1, animationRunner, data);
                    }
                ));
            }
            else if (step is RunRepeatFramesAnimationStep runRepeatFramesStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAllFrames(
                    runRepeatFramesStep.Frames, runRepeatFramesStep.RepeatCount, runRepeatFramesStep.Inverted, runRepeatFramesStep.Action, data, data =>
                    {
                        runRepeatFramesStep.OnFinished?.Invoke(data);
                        if (animationRunner.IsStopped)
                            return;
                        StartNext(stepIndex + 1, animationRunner, data);
                    }
                ));
            }
            else if (step is ParallelAnimationStep parallelStep)
            {
                var builder = Create(_behaviour, _type);
                builder = parallelStep.BuildAction.Invoke(builder);

                if (!builder.IsRunning)
                {
                    builder.Start(data);
                }
            }
            else if (step is ImmediatelyStep immediatelyStep)
            {
                immediatelyStep.OnFinished?.Invoke(data);
                StartNext(stepIndex + 1, animationRunner, data);
            }
            else
                throw new NotImplementedException(step.GetType().FullName);
        }

        private Coroutine Run(IEnumerator animation)
        {
            return _behaviour.StartCoroutine(animation);
        }
    }
}