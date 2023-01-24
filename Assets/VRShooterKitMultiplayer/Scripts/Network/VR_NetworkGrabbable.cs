using Photon.Pun;
using UnityEngine;
using VRSDK;

namespace VRShooterKit.Multiplayer
{
    public partial class VR_NetworkGrabbable : VR_Grabbable
    {

        private VR_NetworkPlayer lastActiveNetworkPlayer = null;
        private VR_NetworkPlayer activeNetworkPlayer;
        private Vector3 desireLocalPosition = Vector3.zero;
        private Quaternion desireLocalRotation = Quaternion.identity;
        public VR_NetworkPlayer ActiveNetworkPlayer => activeNetworkPlayer;
        public VR_NetworkPlayer LastActiveNetworkPlayer => lastActiveNetworkPlayer;
        
        protected override void Start()
        {
            base.Start();
            onGrabStateChange.AddListener(OnGrabStateChange);
        }

        protected override void Update()
        {
            base.Update();

            if (ShouldEnsureCorrectLocalPositionAndRotation())
            {
                thisTransform.localPosition = desireLocalPosition;
                thisTransform.localRotation = desireLocalRotation;
            }
        }

        private bool ShouldEnsureCorrectLocalPositionAndRotation()
        {
            return !preventDefault && currentGrabState == GrabState.Grab && !LocalUserIsGrabbingThisObject();
        }

        private void OnGrabStateChange(GrabState state)
        {
            //we just worry about sync 2 states Grab and Drop

            if (state == GrabState.Grab)
            {
                if (activeNetworkPlayer != null)
                {
                    return;
                }
                
                photonView.RPC(RPCIds.NetworkGrabPRCId, RpcTarget.All, 
                    VR_NetworkManager.instance.LocalPlayerViewId, 
                    (int)GrabController.ControllerType,
                    thisTransform.localPosition,
                    thisTransform.localRotation);
            }
            else if (state == GrabState.Drop)
            {
                SetLayer(unGrabLayer);
                
                bool localUserIsGrabbing = LocalUserIsGrabbingThisObject();

                lastActiveNetworkPlayer = activeNetworkPlayer;
                activeNetworkPlayer = null;
                if (!localUserIsGrabbing)
                {
                    return;
                }
                
                photonView.RPC(RPCIds.SynchronizeGrabStateRPC, RpcTarget.Others, state);
            }
        }

        [PunRPC]
        private void NetworkGrabPRC(int playerId, int controllerIndex, Vector3 localPosition, Quaternion localRotation)
        {
            SetLayer(grabLayer);
            
            //if this grabbable was grabbed faster by another player
            if (activeNetworkPlayer != null && activeController != null)
            {
                return;
            }
            
            

            VR_NetworkPlayer player = VR_NetworkManager.instance.TryGetNetworkPlayerFromViewId(playerId);
            VR_Controller controller = player.GetNetworkControllerFromIndex(controllerIndex);
            activeController = controller;
            activeNetworkPlayer = player;
           
            if (!preventDefault)
            {
                SetFinalGrabStateNetwork(localPosition, localRotation);
            }

            currentGrabState = GrabState.Grab;
            onGrabStateChange.Invoke(GrabState.Grab);

            desireLocalPosition = localPosition;
            desireLocalRotation = localRotation;
        }

        [PunRPC]
        private void SynchronizeGrabStateRPC(int state)
        {
            currentGrabState = (GrabState) state;
            onGrabStateChange.Invoke(currentGrabState);
        }

        private void SetFinalGrabStateNetwork(Vector3 localPosition, Quaternion localRotation)
        {
            thisTransform.parent = activeController.transform;
            thisTransform.localPosition = localPosition;
            thisTransform.localRotation = localRotation;
            
            SetupFixedJointNetwork();
        }

        public void SetupFixedJointNetwork()
        {
            DestroyCurrentJointNetwork();
            CreateNewGrabJointNetwork();
            
            rb.isKinematic = false;
        }
        
        private void DestroyCurrentJointNetwork()
        {
            FixedJoint joint = activeController.GrabPoint.gameObject.GetComponent<FixedJoint>();
            if (joint != null) Destroy( joint );
        }

        private void CreateNewGrabJointNetwork()
        {
            InitializeGrabPointRB();
            CreateFixedJointNetwork();
        }

        private void InitializeGrabPointRB()
        {
            var grabRB = activeController.GrabPointRB;
            
            grabRB.isKinematic = true;
            grabRB.useGravity = false;
        }

        private void CreateFixedJointNetwork()
        {
            FixedJoint joint = activeController.GrabPoint.gameObject.AddComponent<FixedJoint>();
            
            joint.connectedBody = rb;
            joint.breakForce = Mathf.Infinity;
            joint.breakTorque = Mathf.Infinity;
        }

        public bool LocalUserIsGrabbingThisObject()
        {
            return activeNetworkPlayer != null && activeNetworkPlayer.PlayerId == VR_NetworkManager.instance.LocalPlayerViewId;
        }

        public override void OnGrabSuccess(VR_Controller controller)
        {
            photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            base.OnGrabSuccess(controller);
        }

        protected override void GrabUpdate()
        {
            if (ShouldUpdatePositionAndRotationOffset())
            {
                UpdatePositionAndRotationOffset();
                return;
            }

            base.GrabUpdate();
        }

        protected override void DropUpdate()
        {
            base.DropUpdate();
            gameObject.DontDestroyOnLoad(false);
        }

        private bool ShouldUpdatePositionAndRotationOffset()
        {
            return activeNetworkPlayer != null && activeNetworkPlayer.photonView.ViewID != VR_NetworkManager.instance.LocalPlayerViewId;
        }
    }
}