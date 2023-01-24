using System;
using UnityEngine;

namespace VRShooterKit.RootMotion
{
    public class VR_RootMotionBody : MonoBehaviour
    {
        [SerializeField] private IKBone rightFoot = null;
        [SerializeField] private IKBone leftFoot = null;
        [SerializeField] private float feetDistance = 1.0f;

        private void Awake()
        {
            rightFoot.Initialize();
            leftFoot.Initialize();

            Vector3 dir = (rightFoot.BoneRef.position - leftFoot.BoneRef.position).normalized;
           
            rightFoot.BoneRef.position = rightFoot.BoneRef.position + (dir * (feetDistance / 2.0f));
            leftFoot.BoneRef.position = leftFoot.BoneRef.position + (-dir * (feetDistance / 2.0f));
        }
    }
}

