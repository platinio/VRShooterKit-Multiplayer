using UnityEngine;
using VRSDK;

namespace VRShooterKit.RootMotion
{
    public class VR_IKController : VR_Controller
    {
        private int gripHash;
        private int isGrabbingHashIK;

        private Vector3 grabPointInitialPosition = Vector3.zero;
        private Quaternion grabPointInitialRotation = Quaternion.identity;
        
        protected override void Awake()
        {
            base.Awake();

            grabPointInitialPosition = GrabPoint.localPosition;
            grabPointInitialRotation = GrabPoint.localRotation;
        }

        public void ResetGrabPointPositionAndRotation()
        {
            GrabPoint.localPosition = grabPointInitialPosition;
            GrabPoint.localRotation = grabPointInitialRotation;
        }

        protected override void SetupAnimatorHashes()
        {
            base.SetupAnimatorHashes();
            gripHash = Animator.StringToHash($"{GetHandParameterPrefix()}Grip");
            isGrabbingHashIK = Animator.StringToHash($"{GetHandParameterPrefix()}IsGrabbing");
        }

        protected override void UpdateAnimator()
        {
            if (animator.gameObject.activeInHierarchy)
            {
                animator.SetBool( isGrabbingHashIK, currentGrab != null );
            }

            UpdateInputGripValue();
        }

        protected virtual void UpdateInputGripValue()
        {
            float grip = Input.GetAxis1D(VR_InputButton.Grip);
            animator.SetFloat(gripHash, grip);
        }

        protected override void CreateOverrideAnimator()
        {
            if (animator == null) return;

            if (animator.runtimeAnimatorController is AnimatorOverrideController overrideController)
            {
                overrideAnimator = overrideController;
            }
            else
            {
                //create override animator controller so we can change the grab animations at running time
                overrideAnimator = new AnimatorOverrideController( animator.runtimeAnimatorController );
                animator.runtimeAnimatorController = overrideAnimator;
            }

            currentInteractAnimationName = defaultInteractAnimationClip.name;
        }

        private string GetHandParameterPrefix()
        {
            return ControllerType == VR_ControllerType.Right ? "Right" : "Left";
        }
    }
}

