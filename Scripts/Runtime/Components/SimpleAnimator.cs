using System;
using UnityAnimation.Runtime.Projects.unity_animation.Scripts.Runtime.Types;
using UnityEngine;

namespace UnityAnimation.Runtime.Projects.unity_animation.Scripts.Runtime.Components
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
            var fixedDeltaTime = animationType switch
            {
                AnimationType.Scaled => Time.fixedDeltaTime,
                AnimationType.Unscaled => Time.fixedUnscaledDeltaTime,
                _ => throw new NotImplementedException(animationType.ToString())
            };
            Animate(fixedDeltaTime);
        }

        #endregion

        protected abstract void Animate(float deltaTime);
    }
}