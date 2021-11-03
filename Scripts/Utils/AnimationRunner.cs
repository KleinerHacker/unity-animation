using UnityEngine;

namespace UnityAnimation.Runtime.animation.Scripts.Utils
{
    public sealed class AnimationRunner
    {
        internal Coroutine Coroutine { get; set; }
        public bool IsStopped { get; private set; }
        
        private readonly MonoBehaviour _behaviour;

        internal AnimationRunner(MonoBehaviour behaviour)
        {
            _behaviour = behaviour;
        }

        public void Stop()
        {
            IsStopped = true;
            if (Coroutine != null)
            {
                _behaviour.StopCoroutine(Coroutine);
            }
        }
    }
}