using UnityEngine;

namespace VRShooterKit.RootMotion
{
    public class FollowPosition : MonoBehaviour
    {
        [SerializeField] private Transform followTransform = null;

        private void LateUpdate()
        {
            transform.position = followTransform.position;
        }
    }
}