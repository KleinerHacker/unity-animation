using UnityEngine;

namespace UnityAnimation.Runtime.Projects.unity_animation.Scripts.Runtime.Components
{
    [AddComponentMenu(UnityAnimationConstants.Root + "/Rotator")]
    [DisallowMultipleComponent]
    public sealed class Rotator : SimpleAnimator
    {
        #region Inspector Data

        [SerializeField]
        private AnimationCurve rotationCurve = AnimationCurve.Constant(0f, 1f, 1f);

        [SerializeField]
        private Vector3 eulerRotation;

        #endregion

        protected override void Animate(float deltaTime) =>
            transform.rotation *= Quaternion.Euler(eulerRotation * (deltaTime * rotationCurve.Evaluate(deltaTime)));
    }
}