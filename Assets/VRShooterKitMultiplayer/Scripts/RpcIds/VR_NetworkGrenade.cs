﻿using Photon.Pun;
using UnityEngine;
using VRShooterKit.WeaponSystem;

namespace VRShooterKit.Multiplayer
{
    public partial class VR_NetworkGrenade : VR_Grenade
    {
        #region PhotonView
        private PhotonView pvCache;
        public PhotonView photonView
        {
            get
            {
#if UNITY_EDITOR
                // In the editor we want to avoid caching this at design time, so changes in PV structure appear immediately.
                if (!Application.isPlaying || this.pvCache == null)
                {
                    this.pvCache = PhotonView.Get(this);
                }
#else
                if (this.pvCache == null)
                {
                    this.pvCache = PhotonView.Get(this);
                }
#endif
                return this.pvCache;
            }
        }
        #endregion
    }
}