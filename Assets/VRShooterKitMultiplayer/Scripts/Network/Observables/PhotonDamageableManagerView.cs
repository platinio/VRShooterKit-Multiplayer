using System;
using Photon.Pun;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public class PhotonDamageableManagerView : MonoBehaviourPun, IPunObservable
    {
        [SerializeField] private DamageableNetworkManager damageableNetworkManager = null;
        [SerializeField] private float minSyncDiff = 0.01f;

        private float lastHPValue = 0.0f;

        private void Start()
        {
            lastHPValue = damageableNetworkManager.HP;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                float delta = damageableNetworkManager.HP - lastHPValue;
                if (Math.Abs(delta) > minSyncDiff)
                {
                    stream.SendNext(delta);
                }
            }
            else if(stream.IsReading)
            {
                if (stream.Count > 0)
                {
                    stream.PeekNext();

                    try
                    {
                        float delta = (float)stream.PeekNext();
                        stream.ReceiveNext();
                        damageableNetworkManager.SyncHPNetwork(delta);
                    }
                    catch (Exception e)
                    {
                       //this is no my stream
                       return;
                    }
                }
            }
        }
    }
}

