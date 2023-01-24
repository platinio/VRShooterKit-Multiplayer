using UnityEngine;
using VRShooterKit.WeaponSystem;

namespace VRShooterKit.Multiplayer
{
    public class NetworkBullet : Bullet
    {
        [HideInInspector] public bool isMine = false;
        [HideInInspector] public double lagTime = 0.0f;
        
        public void HandleLag(double lag)
        {
            float lagDistance = (float)lag * shootInfo.speed;
            Vector3 futurePosition = transform.position + (shootInfo.dir * lagDistance);
            
            bool hitSomething = HitSomething(transform.position, transform.forward, lagDistance, out var hit);
           
            if (hitSomething)
            {
                HandleHit(hit);
                
                //if we hit something but the bullet just bounce on this surface
                if (!shouldHit)
                {
                    if (hit.distance < lagDistance)
                    {
                        float factor = (hit.distance / lagDistance);
                        //handle the rest of the lag
                        HandleLag(lag * factor);
                    }
                    else
                    {
                        launched = true;
                    }
                }
            }
            else
            {
                transform.position = futurePosition;
                launched = true;
            }
        }

        protected override void HandleBulletLaunch(ShootInfo info)
        {
            shootInfo = info;
            if (!isMine)
            {
                HandleLag(lagTime);
            }
            else
            {
                base.HandleBulletLaunch(info);
            }
        }

        protected override bool TryDoDamage(Collider c)
        {
            if (isMine)
            {
                return base.TryDoDamage(c);
            }

            return true;
        }
    }
}

