using Photon.Pun;
using UnityEngine;
using VRSDK;

namespace VRShooterKit.Multiplayer
{
    public class ExitAction : MonoBehaviourPun
    {
        [SerializeField] private VR_InputButton exitButton = VR_InputButton.Primary;
        [SerializeField] private float pressTime = 5.0f;

        private VR_Controller rightController = null;
        private VR_Controller leftController = null;
        private float timer = 0.0f;
        private bool isLeaving = false;

        private void Start()
        {
            if (!photonView.IsMine)
            {
                Destroy(gameObject);
            }

            rightController = VR_Manager.instance.Player.RightController;
            leftController = VR_Manager.instance.Player.LeftController;
        }

        private void Update()
        {
            if (rightController == null || leftController == null || isLeaving)
            {
                return;
            }

            if (rightController.Input.GetButton(exitButton) && leftController.Input.GetButton(exitButton))
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = 0.0f;
            }

            if (timer >= pressTime)
            {
                isLeaving = true;
                VR_NetworkManager.instance.ExitRoomAndLoadLobby();
            }
        }
    }
}

