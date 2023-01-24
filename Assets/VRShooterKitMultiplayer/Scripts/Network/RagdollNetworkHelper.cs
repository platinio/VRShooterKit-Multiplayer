using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using VRSDK;

namespace VRShooterKit.Multiplayer
{
    public partial class RagdollNetworkHelper : RagdollHelper
    {
        [SerializeField] private List<Rigidbody> ignoreRBList = null;
        [SerializeField] private PhotonView bodyPhotonView = null;
        
        
        private PhotonRigidbodyViewExtended[] rigidBodyViewArray = null;
        private TransformCache[] cacheBodyPosition;
        private Collider[] colliderArray = null;

        protected override void Awake()
        {
            base.Awake();

            rigidBodyViewArray = new PhotonRigidbodyViewExtended[rbArray.Length];
            colliderArray = new Collider[rbArray.Length];
            
            for (int n = 0; n < rbArray.Length; n++)
            {
                if (rbArray[n] != null)
                {
                    rigidBodyViewArray[n] = rbArray[n].GetComponent<PhotonRigidbodyViewExtended>();
                    colliderArray[n] = rbArray[n].GetComponent<Collider>();
                }
            }
            
        }

        public void ChangeColliderArrayStateRPC(bool state, RpcTarget target)
        {
            photonView.RPC(RPCIds.ChangeColliderArrayState, target, state);
        }

        [PunRPC]
        private void ChangeColliderArrayState(bool state)
        {
            foreach (var collider in colliderArray)
            {
                if(collider == null) continue;
                
                collider.enabled = state;
            }
        }

        public void DisableRagdoll()
        {
            SetKinematic(true);
        }

        public void CacheBodyTransformInfo()
        {
            cacheBodyPosition = new TransformCache[rbArray.Length];
            for(int n = 0 ; n < rbArray.Length ; n++)
            {
                TransformCache cache = new TransformCache();
                cache.position = rbArray[n].position;
                cache.rotation = rbArray[n].rotation;

                cacheBodyPosition[n] = cache;
            }
        }

        public override void SetKinematic(bool newValue)
        {
            for (int n = 0 ; n < rbArray.Length ; n++)
            {
                if (!ignoreRBList.Contains(rbArray[n]))
                {
                    rbArray[n].isKinematic = newValue;
                }
            }
        }

        public void EnableRagdollAndTransferOwnership()
        {
            bodyPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            StopRagdollSync();
            //StartCoroutine(SetBodyCache());
            //SetBodyCacheInmediatly();
            EnableRagdoll();
            StartCoroutine(WaitUntilOwnerShipTransferAndResumeRagdollSync());
        }

        private IEnumerator SetBodyCache()
        {
            yield return new WaitForFixedUpdate();
            for(int n = 0 ; n < rbArray.Length ; n++)
            {
                if (rbArray[n] != null && cacheBodyPosition[n] != null && cacheBodyPosition.Length < n)
                {
                    rbArray[n].position = cacheBodyPosition[n].position;
                    rbArray[n].rotation = cacheBodyPosition[n].rotation;
                }
            }
        }

        private void SetBodyCacheImmediately()
        {
            for(int n = 0 ; n < rbArray.Length ; n++)
            {
                if (rbArray[n] != null && cacheBodyPosition[n] != null && cacheBodyPosition.Length < n)
                {
                    rbArray[n].position = cacheBodyPosition[n].position;
                    rbArray[n].rotation = cacheBodyPosition[n].rotation;
                }
            }
        }

        private IEnumerator WaitUntilOwnerShipTransferAndResumeRagdollSync()
        {
            while (!photonView.IsMine)
            {
                yield return new WaitForEndOfFrame();
            }
            
            ResumeRagdollSync();
        }

        private void StopRagdollSync()
        {
            foreach (var rbView in rigidBodyViewArray)
            {
                if (rbView != null)
                {
                    rbView.StopSync();
                }
            }
        }

        private void ResumeRagdollSync()
        {
            foreach (var rbView in rigidBodyViewArray)
            {
                if (rbView)
                {
                    rbView.ResumeSync();
                }
            }
        }
    }

    public class TransformCache
    {
        public Vector3 position;
        public Quaternion rotation;
    }
}
