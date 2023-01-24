using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using TMPro;
using VRSDK;
using VRShooterKit.RootMotion;

namespace VRShooterKit.Multiplayer
{
    public partial class VR_NetworkPlayer : MonoBehaviourPun
    {
       
        [SerializeField] private VR_Player player = null;
        [SerializeField] private VR_ScreenFader blackScreenFader = null;
        [SerializeField] private PhotonView bodyPhotonView = null;
        [SerializeField] private SkinnedMeshRenderer bodyRender = null;
        [SerializeField] private DamageableNetworkManager damageableNetworkManager = null;
        [SerializeField] private Transform rightHandBone = null;
        [SerializeField] private Transform rightHandBoneTarget = null;
        [SerializeField] private VR_CharacterController characterController = null;
        [SerializeField] private PhotonDestroyer photonDestroyer = null;
        #if SDK_OCULUS
        [SerializeField] private OVRManager OVRManager = null;
        #endif
        [SerializeField] private int playerId = -1;
        [SerializeField] private float reviveTime = 5.0f;
        [SerializeField] private TextMeshProUGUI nicknameLabel = null;
        [SerializeField] private FeetRaycaster leftFeetRaycaster = null;
        [SerializeField] private FeetRaycaster rightFeetRaycaster = null;
        [SerializeField] private UnityEvent onPlayerRevive = null;
        
        
        public VR_Controller RightController => player.RightController;
        public VR_IKController RightIKController => player.RightController as VR_IKController;
        public VR_Controller LeftController => player.LeftController;
        public VR_IKController LeftIKController => player.LeftController as VR_IKController;
        public VR_CharacterController CharacterController => characterController;
        public VR_Player Player => player;
        public int PlayerId => playerId;
        public int ActorNumber => actorNumber;
        public DamageableNetworkManager DamageableNetworkManager { get => damageableNetworkManager; }

        public string Nickname
        {
            get
            {
                if (!string.IsNullOrEmpty(nickname)) return nickname;

                foreach (var photonPlayer in PhotonNetwork.PlayerList)
                {
                    if (photonPlayer.ActorNumber == actorNumber) return photonPlayer.NickName;
                }

                return null;
            }
        }

        private Transform thisTransform = null;
        private RagdollNetworkHelper ragdollNetwork = null;
        private const float kRepositionTime = 0.5f;
        private string nickname = null;
        private int actorNumber = 0;
        

        private void Awake()
        {
            actorNumber = photonView.OwnerActorNr;
            thisTransform = transform;
            ragdollNetwork = GetComponent<RagdollNetworkHelper>();
            VR_NetworkManager.instance.OnAllPlayersReady += OnAllPlayersReady;
            
            if (!photonView.IsMine)
            {
                SetBodyVisualState(false);
                playerId = photonView.ViewID;
                VR_NetworkManager.instance.NotifyNetWorkPlayer(this, photonView.ViewID);
               
                if (photonDestroyer != null)
                {
                    photonDestroyer.DestroyUnnecessaryComponents();
                }
                else
                {
                    Debug.LogError("[VRShooterKit] Photon Destroyer is missing!");
                }

#if SDK_OCULUS
                var manager = FindObjectOfType<OVRManager>();
                OVRManager.instance = manager;
                OVRManager.OVRManagerinitialized = true;
#endif
            }
            else
            {
#if SDK_OCULUS
                OVRManager.instance = OVRManager;
#endif
                if (nicknameLabel != null)
                {
                    nicknameLabel.gameObject.SetActive(false);
                }

                photonView.RPC(RPCIds.SetNickname, RpcTarget.Others, PhotonNetwork.LocalPlayer.NickName);
                
                int id = photonView.ViewID;
                VR_NetworkManager.instance.LocalPlayerViewId = id;
                VR_NetworkManager.instance.NotifyNetWorkPlayer(this, id);
                playerId = id;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnAllPlayersReady()
        {
            if (blackScreenFader != null)
            {
                blackScreenFader.FadeOut(1.0f);
            }

            if (RightController != null)
            {
                RightController.enabled = true;
            }

            if (LeftController != null)
            {
                LeftController.enabled = true;
            }

            if (characterController != null)
            {
                CharacterController.enabled = true;
            }

        }

        [PunRPC]
        private void SetNickname(string nickname)
        {
            if (nicknameLabel != null)
            {
                nicknameLabel.text = nickname;
            }

            this.nickname = nickname;
        }

        public VR_Controller GetNetworkControllerFromIndex(int index)
        {
            if ((int) VR_ControllerType.Right == index) return RightController;
            return LeftController;
        }

        public void SetFeeRaycasterEnableValue(bool value)
        {
            if (rightFeetRaycaster != null)
            {
                rightFeetRaycaster.enabled = value;
            }

            if (leftFeetRaycaster != null)
            {
                leftFeetRaycaster.enabled = value;
            }
        }

        public void Revive()
        {
            if (!photonView.IsMine)
            {
                return;
            }

            StartCoroutine(ReviveRoutine());
        }

        private IEnumerator ReviveRoutine()
        {
            DropGrabObjects();
            ChangeControllersEnable(false);
            yield return new WaitForSeconds(reviveTime);
            yield return RepositionBody();
            ChangeControllersEnable(true);
        }

        private void ChangeControllersEnable(bool state)
        {
            RightController.enabled = state;

            if (RightController.DistanceGrab != null)
            {
                RightController.DistanceGrab.enabled = state;
            }

            LeftController.enabled = state;

            if (LeftController.DistanceGrab != null)
            {
                LeftController.DistanceGrab.enabled = state;
            }
        }

        private IEnumerator RepositionBody()
        {
            //disable this visual for all players
            photonView.RPC(RPCIds.SetBodyVisualState, RpcTarget.All, false);
            //wait ping time in order to disable visual for all players
            float ping = PhotonNetwork.GetPing() / 1000.0f;
            yield return new WaitForSeconds(ping);
            
            bodyPhotonView.TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);
            
            //wait until the transfer is complete
            while (!bodyPhotonView.IsMine)
            {
                yield return new WaitForEndOfFrame();
            }
            
            //disable all colliders
            ragdollNetwork.ChangeColliderArrayStateRPC(false, RpcTarget.All);
            
            //set all rb as kinematic
            ragdollNetwork.SetKinematic(true);
            
            //wait ping time in order to disable visual for all players
            ping = PhotonNetwork.GetPing() / 1000.0f;
            yield return new WaitForSeconds(ping);
            
            //trigger the revive
            photonView.RPC(RPCIds.TriggerOnPlayerRevive, RpcTarget.All);
            
            //wait until the reposition is complete in order to enable visuals
            while (!IsRepositionComplete())
            {
                yield return new WaitForFixedUpdate();
                //ensure everything is kinematic or we will stay in this state forever
                ragdollNetwork.SetKinematic(true);
            }
            
            //set visual enable
            photonView.RPC(RPCIds.SetBodyVisualState, RpcTarget.All, true);
            ragdollNetwork.ChangeColliderArrayStateRPC(true, RpcTarget.All);
        }

        private bool IsRepositionComplete()
        {
            float d = Vector3.Distance(rightHandBone.position, rightHandBoneTarget.position);
            return d < 0.025f;
        }

        [PunRPC]
        public void SetBodyVisualState(bool state)
        {
            bodyRender.enabled = state;
        }

        public void SetBodyVisualViaRPC(bool state)
        {
            photonView.RPC(RPCIds.SetBodyVisualState, RpcTarget.Others, true);
        }

        private void DropGrabObjects()
        {
            if (LeftController.CurrentGrab != null)
            {
                LeftController.CurrentGrab.ForceDrop();
            }

            if (RightController.CurrentGrab != null)
            {
                RightController.CurrentGrab.ForceDrop();
            }
        }

        [PunRPC]
        private void TriggerOnPlayerRevive()
        {
            if (photonView.IsMine && blackScreenFader != null)
            {
                blackScreenFader.FadeOut(1.0f);
            }
            
            onPlayerRevive.Invoke();

            Transform spawnPoint = NetworkTeamManager.instance.GetRandomSpawnPoint();

            thisTransform.position = spawnPoint.position;
            thisTransform.rotation = spawnPoint.rotation;

            ResetControllersPositionAndRotation();

            SetFeeRaycasterEnableValue(true);

            Transform bodyPhotonTransform = bodyPhotonView.transform;

            bodyPhotonTransform.parent = transform;
            StartCoroutine(ResetBodyPositionAndRotation());
        }

        private IEnumerator ResetBodyPositionAndRotation()
        {
            Transform bodyPhotonTransform = bodyPhotonView.transform;
            float timer = kRepositionTime;

            while (timer > 0.0f)
            {
                timer -= Time.deltaTime;
                bodyPhotonTransform.localPosition = Vector3.zero;
                bodyPhotonTransform.localRotation = Quaternion.identity;
                yield return new WaitForEndOfFrame();
            }
            
        }

        private void ResetControllersPositionAndRotation()
        {
            if (RightIKController != null)
            {
                RightIKController.ResetGrabPointPositionAndRotation();
            }
            
            if (LeftIKController != null)
            {
                LeftIKController.ResetGrabPointPositionAndRotation();
            }
            
            RightController.Recenter();
            LeftController.Recenter();
        }

    }
}