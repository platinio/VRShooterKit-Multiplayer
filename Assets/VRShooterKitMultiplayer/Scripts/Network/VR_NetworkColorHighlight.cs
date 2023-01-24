using VRSDK;

namespace VRShooterKit.Multiplayer
{
    public class VR_NetworkColorHighlight : VR_ColorHiglight
    {
        public override void Highlight(VR_Controller controller)
        {
            if (ControllerBelongsToLocalUser(controller))
            {
                base.Highlight(controller);
            }
        }

        private bool ControllerBelongsToLocalUser(VR_Controller controller)
        {
            VR_NetworkPlayer player = VR_NetworkManager.instance.LocalNetworkPlayer;
            var localController = player.GetNetworkControllerFromIndex((int)controller.ControllerType);

            return localController == controller;
        }
    }

}

