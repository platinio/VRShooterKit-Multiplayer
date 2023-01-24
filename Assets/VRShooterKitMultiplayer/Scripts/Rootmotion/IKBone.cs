using UnityEngine;

namespace VRShooterKit.RootMotion
{
    [System.Serializable]
    public class IKBone
    {
        [SerializeField] private Transform realIKBone = null;
        [SerializeField] private Transform boneRef = null;
        [SerializeField] private float updateDistance = 0.15f;

        public Transform BoneRef => boneRef;

        public virtual void Initialize()
        {
            boneRef.position = realIKBone.position;
            boneRef.rotation = realIKBone.rotation;
        }

        protected virtual void Update()
        {
            float d = (realIKBone.position - boneRef.position).sqrMagnitude;
            if (d > updateDistance * updateDistance)
            {
                SetRealBonePosition();
            }
        }

        private void SetRealBonePosition()
        {
            realIKBone.position = boneRef.position;
        }
    }
}