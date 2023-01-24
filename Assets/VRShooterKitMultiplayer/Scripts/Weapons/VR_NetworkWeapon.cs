using Photon.Pun;
using UnityEngine;
using VRShooterKit.WeaponSystem;
using Random = UnityEngine.Random;

namespace VRShooterKit.Multiplayer
{
    public partial class VR_NetworkWeapon : VR_Weapon
    {
       
        public VR_NetworkGrabbable NetworkGrabbable => grabbable as VR_NetworkGrabbable;
       
        private double currentNetworkLag = 0.0f;
        private Vector3 currentPosition = Vector3.zero;
        private Quaternion currentRotation = Quaternion.identity;
        private Vector3 currentDir = Vector3.zero;
        
        protected override void OnShootComplete(Bullet bullet, Vector3 dir)
        {
            if (photonView.IsMine)
            {
                Transform bulletTransform = bullet.transform;
                photonView.RPC(RPCIds.Fire, RpcTarget.Others, 
                    PhotonNetwork.Time, 
                    bulletTransform.position,
                    bulletTransform.rotation,
                    dir);
                base.OnShootComplete(bullet, dir);
            }
        }

        protected override ShootInfo CreateShootInfo()
        {
            if (photonView.IsMine)
            {
                cacheShootInfo.dir = shootPoint.forward;
            }
            else
            {
                cacheShootInfo.dir = currentDir;
            }

            cacheShootInfo.dmg = dmg;
            cacheShootInfo.canDismember = canDismember;
            cacheShootInfo.range = range;
            cacheShootInfo.speed = bulletSpeed;
            cacheShootInfo.maxBounceCount = maxBulletBounceCount;
            cacheShootInfo.hitForce = Random.Range( minHitForce, maxHitForce );
            cacheShootInfo.hitLayer = hitLayer;
            cacheShootInfo.hitEffect = hitEffect;

            if (NetworkGrabbable != null && NetworkGrabbable.ActiveNetworkPlayer != null)
            {
                cacheShootInfo.sender = NetworkGrabbable.ActiveNetworkPlayer.gameObject;
            }
            else if(grabbable != null && grabbable.GrabController != null)
            {
                cacheShootInfo.sender = grabbable.GrabController.transform.root.gameObject;
            }
            else
            {
                cacheShootInfo.sender = null;
            }

            return cacheShootInfo;
        }

        protected override void UpdateMuzzleFlash(bool shootThisFrame)
        {
            if (m_muzzleFlash == null) return;
            
           if (!NetworkGrabbable.LocalUserIsGrabbingThisObject()) return;
           if (!m_muzzleFlash.IsVisible) photonView.RPC(RPCIds.DisableMuzzleFlash, RpcTarget.Others);
        }

        [PunRPC]
        private void FireRPC(double time, Vector3 position, Quaternion rotation, Vector3 dir)
        {
            currentNetworkLag = PhotonNetwork.Time - time;
            currentPosition = position;
            currentRotation = rotation;
            currentDir = dir;
           
            base.Fire();
        }

        protected override Bullet InstantiateBullet(Vector3 position, Quaternion rotation)
        {
            //if I am controlling this weapon just shoot as normal
            //otherwise use the position and rotation send by the owner
            if (photonView.IsMine)
            {
                Bullet bullet = base.InstantiateBullet(position, rotation);
                
                if(bullet is NetworkBullet networkBullet)
                {
                    networkBullet.isMine = true;
                    networkBullet.lagTime = 0.0f;
                }

                return bullet;
            }
            else
            {
                Bullet bullet = base.InstantiateBullet(currentPosition, currentRotation);
                
                if(bullet is NetworkBullet networkBullet)
                {
                    networkBullet.isMine = false;
                    networkBullet.lagTime = currentNetworkLag;
                }

                return bullet;
            }
        }

        [PunRPC]
        private void DisableMuzzleFlashRPC()
        {
            if (m_muzzleFlash == null) return;
            m_muzzleFlash.SetVisibility(false);
        }

    }
}

