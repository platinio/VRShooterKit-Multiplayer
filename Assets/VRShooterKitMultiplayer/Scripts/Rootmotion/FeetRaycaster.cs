using UnityEngine;

namespace VRShooterKit.RootMotion
{
    public class FeetRaycaster : MonoBehaviour
    {
        [SerializeField] private float raycastOffset = 2.0f;
        [SerializeField] private float raycastMaxDistance = 10.0f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform realBoneIK = null;
        [SerializeField] private float minVerticalDiff = 0.015f;
        [SerializeField] private float positionOffset = 0.25f;
        
        private void Update()
        {
            Vector3 rayPoint = realBoneIK.position + (Vector3.up * raycastOffset);
            if(Physics.Raycast(rayPoint, Vector3.down, out var hitInfo, raycastMaxDistance, groundLayer.value))
            {
                float verticalDiff = hitInfo.point.y - (realBoneIK.position.y + positionOffset);

                if (Mathf.Abs(verticalDiff) > minVerticalDiff)
                {
                    transform.position += Vector3.up * verticalDiff;
                }
            }
        }
    }
}

