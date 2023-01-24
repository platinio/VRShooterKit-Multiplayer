using DamageSystem;
using Photon.Pun;
using UnityEngine;
using VRSDK;

namespace VRShooterKit.Multiplayer
{
    public class RagdollNetworkSetup : MonoBehaviour
    {
        [SerializeField] private Transform m_parent = null;
        [SerializeField] private float m_damageMultiplier = 1.0f;
        [SerializeField] private SurfaceDetails m_surfaceDetailsTemplate = null;
        [SerializeField] private float m_teleportIfDistanceIsGreaterThan = 3.0f;
        
        public void SetupRagdoll()
        {
            Rigidbody[] rbArray = m_parent.GetComponentsInChildren<Rigidbody>();

            foreach (var rb in rbArray)
            {
                var damageablePart = rb.gameObject.GetOrAddComponent<DamageablePart>();
                damageablePart.ExternalSetup(m_damageMultiplier, rb);

                var surfaceDetails = rb.gameObject.GetOrAddComponent<SurfaceDetails>();
                surfaceDetails.CopySettings(m_surfaceDetailsTemplate);

                var photonRbViewExtended = rb.gameObject.GetOrAddComponent<PhotonRigidbodyViewExtended>();
                photonRbViewExtended.m_SynchronizeVelocity = true;
                photonRbViewExtended.m_SynchronizeAngularVelocity = true;
                photonRbViewExtended.m_SynchronizeIsKinematic = true;
                photonRbViewExtended.m_SynchronizeUseGravity = true;
                photonRbViewExtended.m_TeleportIfDistanceGreaterThan = 3.0f;
            }
        }
    }
}
