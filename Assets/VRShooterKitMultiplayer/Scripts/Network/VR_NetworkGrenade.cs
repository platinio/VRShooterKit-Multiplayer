using Photon.Pun;
using UnityEngine;
using VRSDK;
using VRShooterKit.WeaponSystem;

namespace VRShooterKit.Multiplayer
{
    public partial class VR_NetworkGrenade : VR_Grenade
    {
        [SerializeField] private VR_NetworkGrabbable networkGrabbable = null;
        
        public override void Explode()
        {
            ExplodeNetwork(true);
            photonView.RPC("ExplodeRPC", RpcTarget.Others);
        }

        [PunRPC]
        public void ExplodeRPC()
        {
            ExplodeNetwork(false);
        }

        private void ExplodeNetwork(bool isMine)
        {
            if (explosionPrefab != null)
            {
                var clone =Instantiate( explosionPrefab, transform.position, Quaternion.identity );

                if (clone is NetworkExplosion networkExplosion)
                {
                    networkExplosion.IsMine = isMine;
                }
                
                clone.Explode(GetSender());
            }

            explode = true;
            Destroy(gameObject);
        }

        private GameObject GetSender()
        {
            if (networkGrabbable == null)
            {
                return null;
            }

            //if the grenade is being grabbed get the current player grabbing it
            if (networkGrabbable.CurrentGrabState == GrabState.Grab && networkGrabbable.ActiveNetworkPlayer != null)
            {
                return networkGrabbable.ActiveNetworkPlayer.gameObject;
            }

            //if the grenade was dropped get the last player grabbing it
            if (GrenadeWasDropped() && networkGrabbable.LastActiveNetworkPlayer != null)
            {
                return networkGrabbable.LastActiveNetworkPlayer.gameObject;
            }

            return null;
        }

        private bool GrenadeWasDropped()
        {
            return networkGrabbable.CurrentGrabState == GrabState.Drop || networkGrabbable.CurrentGrabState == GrabState.UnGrab;
        }

    }
}

