using UnityEngine;

namespace UnityAnimation.Runtime.Projects.unity_animation.Scripts.Runtime.Components
{
    [AddComponentMenu(UnityAnimationConstants.Root + "/Scaler")]
    [DisallowMultipleComponent]
    public sealed class Scaler : SimpleAnimator
    {
        #region Inspector Data

        [SerializeField]
        private AnimationCurve scalingCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [SerializeField]
        [Min(0.000000000000001f)]
        private float scalingSpeed = 1f;

        [Space]
        [SerializeField]
        private Vector3 scalingStart = Vector3.one;

        [SerializeField]
        private Vector3 scalingEnd = Vector3.one * 2f;

        #endregion

        protected override void Animate(float deltaTime) =>
            transform.localScale = Vector3.Lerp(scalingStart, scalingEnd, scalingCurve.Evaluate(deltaTime * scalingSpeed));
    }
}