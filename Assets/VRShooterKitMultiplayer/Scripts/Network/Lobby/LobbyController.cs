using Platinio;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public class LobbyController : Singleton<LobbyController>
    {
        [SerializeField] private LoginUI loginUI = null;
        [SerializeField] private LobbyUI lobbyUI = null;
        [SerializeField] private CreateRoomUI createRoomUI = null;
        [SerializeField] private RoomUI roomUI = null;

        protected override void Awake()
        {
            base.Awake();
            loginUI.gameObject.SetActive(true);
        }

        public void ShowLobbyUI()
        {
            loginUI.gameObject.SetActive(false);
            lobbyUI.gameObject.SetActive(true);
        }

        public void ShowCreateRoomUI()
        {
            lobbyUI.gameObject.SetActive(false);
            createRoomUI.gameObject.SetActive(true);
        }

        public void ShowRoomUI()
        {
            lobbyUI.gameObject.SetActive(false);
            createRoomUI.gameObject.SetActive(false);
            roomUI.gameObject.SetActive(true);
        }
    }
}

