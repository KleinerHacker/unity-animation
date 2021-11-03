using System;
using System.Collections;
using UnityEngine;

namespace UnityAnimation.Runtime.animation.Scripts.Utils
{
    public sealed partial class AnimationBuilder
    {
        public bool IsRunning { get; private set; }

        public AnimationRunner Start(float delayed, Action onFinished = null)
        {
            if (_steps.Count <= 0)
                throw new InvalidOperationException("No animation inside builder");
            if (IsRunning)
                throw new InvalidOperationException("Is already running");

            var animationRunner = new AnimationRunner(_behaviour);
            
            IsRunning = true;
            animationRunner.Coroutine = Run(AnimationUtils.WaitAndRun(delayed, () =>
            {
                onFinished?.Invoke();
                if (animationRunner.IsStopped)
                    return;
                StartNext(0, animationRunner);
            }));

            return animationRunner;
        }

        public AnimationRunner Start()
        {
            if (_steps.Count <= 0)
                throw new InvalidOperationException("No animation inside builder");
            if (IsRunning)
                throw new InvalidOperationException("Is already running");

            var animationRunner = new AnimationRunner(_behaviour);

            IsRunning = true;
            StartNext(0, animationRunner);

            return animationRunner;
        }

        private void StartNext(int stepIndex, AnimationRunner animationRunner)
        {
            if (stepIndex >= _steps.Count)
            {
                _onFinished?.Invoke();
                IsRunning = false;

                return;
            }

            var step = _steps[stepIndex];
            if (step is SubAnimationStep subAnimStep)
            {
                subAnimStep.RunSubAnimation.Invoke(() =>
                {
                    subAnimStep.OnFinished?.Invoke();
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner);
                });
            }
            else if (step is AnimateAnimationStep animStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAnimation(_type, animStep.Curves, animStep.Speed, animStep.Handler, () =>
                {
                    animStep.OnFinished?.Invoke();
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner);
                }));
            }
            else if (step is AnimateConstantAnimationStep animConstStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAnimationConstant(_type, animConstStep.Time, animConstStep.Handler, () =>
                {
                    animConstStep.OnFinished?.Invoke();
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner);
                }));
            }
            else if (step is WaitSecondsAnimationStep waitSecStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.WaitAndRun(_type, waitSecStep.Seconds, () =>
                {
                    waitSecStep.OnFinished?.Invoke();
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner);
                }));
            }
            else if (step is WaitFramesAnimationStep waitFrameStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.WaitAndRun(waitFrameStep.Frames, () =>
                {
                    waitFrameStep.OnFinished?.Invoke();
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner);
                }));
            }
            else if (step is RunAllSecondsAnimationStep runAllSecStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAll(_type, runAllSecStep.Seconds, () =>
                {
                    runAllSecStep.OnFinished?.Invoke();
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner);
                }, runAllSecStep.Actions));
            }
            else if (step is RunAllFramesAnimationStep runAllFramesStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAll(_type, runAllFramesStep.Frames, () =>
                {
                    runAllFramesStep.OnFinished?.Invoke();
                    if (animationRunner.IsStopped)
                        return;
                    StartNext(stepIndex + 1, animationRunner);
                }, runAllFramesStep.Actions));
            }
            else if (step is RunRepeatSecondsAnimationStep runRepeatSecStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAll(
                    runRepeatSecStep.Seconds, runRepeatSecStep.RepeatCount, runRepeatSecStep.Action, _type, () =>
                    {
                        runRepeatSecStep.OnFinished?.Invoke();
                        if (animationRunner.IsStopped)
                            return;
                        StartNext(stepIndex + 1, animationRunner);
                    }
                ));
            }
            else if (step is RunRepeatFramesAnimationStep runRepeatFramesStep)
            {
                animationRunner.Coroutine = Run(AnimationUtils.RunAll(
                    runRepeatFramesStep.Frames, runRepeatFramesStep.RepeatCount, runRepeatFramesStep.Action, _type, () =>
                    {
                        runRepeatFramesStep.OnFinished?.Invoke();
                        if (animationRunner.IsStopped)
                            return;
                        StartNext(stepIndex + 1, animationRunner);
                    }
                ));
            }
            else if (step is ParallelAnimationStep parallelStep)
            {
                var builder = Create(_behaviour, _type);
                builder = parallelStep.BuildAction.Invoke(builder);

                if (!builder.IsRunning)
                {
                    builder.Start();
                }
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