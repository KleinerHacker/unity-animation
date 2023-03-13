using System;
using UnityAnimation.Runtime.animation.Scripts.Runtime.Types;
using UnityEngine;

namespace UnityAnimation.Runtime.animation.Scripts.Runtime.Components
{
    public abstract class SimpleAnimator : MonoBehaviour
    {
        #region Inspector Data

        [Header("Animation")]
        [SerializeField]
        protected AnimationType animationType = AnimationType.Scaled;

        #endregion

        #region Builtin Methods

        private void FixedUpdate()
        {
            var fixedTime = animationType switch
            {
                AnimationType.Scaled => Time.fixedTime,
                AnimationType.Unscaled => Time.fixedUnscaledTime,
                _ => throw new NotImplementedException(animationType.ToString())
            };
            Animate(fixedTime);
        }

        #endregion

        protected abstract void Animate(float deltaTime);
    }
}