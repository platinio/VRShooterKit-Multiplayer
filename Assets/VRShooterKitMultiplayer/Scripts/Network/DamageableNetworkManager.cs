using DamageSystem;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public partial class DamageableNetworkManager : DamageableManager
    {
        [SerializeField] private VRIK rootMotionIK = null;
        [SerializeField] private VR_NetworkPlayer networkPlayer = null;
        [SerializeField] private RagdollNetworkHelper ragdollHelper = null;

        private float lastHPValue = 0.0f;
        private DamageInfo lastDamageInfo = null;

        public DamageInfo LastDamageInfo { get => lastDamageInfo; }

        protected override void Awake()
        {
            base.Awake();

            lastHPValue = hp;
            //this a way to sync the local hp value with the local simulation of every player even the owner
            onHPChangeEvent.AddListener(SyncLocalHPValue);
        }

        public void Revive()
        {
            isDead = false;
            hp = MaxHP;
            lastHPValue = MaxHP;
            photonView.RPC(RPCIds.SyncHPNetwork, RpcTarget.Others, MaxHP);
        }

        protected override void DoRegeneration()
        {
            if (photonView.IsMine)
            {
                base.DoRegeneration();
            }
        }

        protected override void TriggerDieEvent()
        {
            if (networkPlayer != null)
            {
                networkPlayer.SetFeeRaycasterEnableValue(false);
            }
            
            if (lastDamageInfo != null && lastDamageInfo.sender != null &&
                lastDamageInfo.sender.GetComponent<VR_NetworkPlayer>() != null)
            {
                var sender = lastDamageInfo.sender.GetComponent<VR_NetworkPlayer>();
                if (VR_NetworkManager.instance.LocalNetworkPlayer == sender)
                {
                    ragdollHelper.CacheBodyTransformInfo();   
                    rootMotionIK.enabled = false;
                    ragdollHelper.EnableRagdollAndTransferOwnership();
                }
            }
            else
            {
                rootMotionIK.enabled = false;
            }

            base.TriggerDieEvent();
            photonView.RPC(RPCIds.TriggerDieEventRPC, RpcTarget.Others);
        }

        private void SyncLocalHPValue(float value)
        {
            float delta = value - lastHPValue;

            //if someone else do damage to this player let others players know about it
            if (!photonView.IsMine && delta < 0.0f)
            {
                lastHPValue = value;
                photonView.RPC(RPCIds.SyncHPNetwork, RpcTarget.Others, delta);
            }
            
            //if this player heal himself let others know about it
            else if(photonView.IsMine && delta > 0.0f)
            {
                lastHPValue = value;
                photonView.RPC(RPCIds.SyncHPNetwork, RpcTarget.Others, delta);
            }
        }
        

        [PunRPC]
        private void TriggerDieEventRPC()
        {
            //this can be called from a RPC so check if we are dead first!
            if (isDead)
            {
                return;
            }
            
            ProcessDieForExternalUser();
        }

        private void ProcessDieForExternalUser()
        {
            base.TriggerDieEvent();
            ragdollHelper.EnableRagdoll();
        }

        [PunRPC]
        public void SyncHPNetwork(float delta)
        {
            ModifyHP(delta, false);
            lastHPValue = hp;
        }

        public override void DoDamage(DamageInfo info, DamageablePart damageable)
        {
            lastDamageInfo = info;
            base.DoDamage(info, damageable);
        }
    }
}

