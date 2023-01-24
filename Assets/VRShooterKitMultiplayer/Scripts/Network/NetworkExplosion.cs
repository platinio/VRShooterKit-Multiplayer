using DamageSystem;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public class NetworkExplosion : Explosion
    {
        public bool IsMine = false;

        protected override void DoDamage(Damageable damageable, DamageInfo info)
        {
            if (IsMine)
            {
                base.DoDamage(damageable, info);
            }
        }

        protected override void ApplyImpactForce(Rigidbody rb)
        {
            if (IsMine)
            {
                base.ApplyImpactForce(rb);
            }
        }
    }
}

