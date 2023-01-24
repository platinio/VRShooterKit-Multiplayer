using System;
using VRShooterKit.RootMotion;

namespace VRShooterKit.Multiplayer
{
    public partial class VR_IKControllerNetwork : VR_IKController
    {
        protected override void UpdateInputGripValue()
        {
            if (photonView.IsMine)
            {
                base.UpdateInputGripValue();
            }
        }

        protected override void Start()
        {
            base.Start();
            VR_NetworkManager.instance.OnBeforeLeaveRoom += OnBeforePlayerLeaveRoom;
        }

        private void OnDestroy()
        {
            VR_NetworkManager.instance.OnBeforeLeaveRoom -= OnBeforePlayerLeaveRoom;
        }

        private void OnBeforePlayerLeaveRoom()
        {
            //drop whathever we have grabbed or it will be destroyed
            if (currentGrab != null)
            {
                currentGrab.ForceDrop();
            }
        }
    }
}

