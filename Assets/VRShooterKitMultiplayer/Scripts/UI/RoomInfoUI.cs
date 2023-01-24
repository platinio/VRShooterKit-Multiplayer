using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRShooterKit.Multiplayer
{
    public class RoomInfoUI : MonoBehaviourPunCallbacks
    {
        [SerializeField] private TextMeshProUGUI roomNameLabel = null;
        [SerializeField] private TextMeshProUGUI playerCount = null;
        [SerializeField] private Button joinButton = null;

        private RoomInfo currentRoomInfo = null;
        private Action onStartLeaveRoom = null;
        private bool isJoining = false;
        
        public void Initialize(RoomInfo roomInfo, Action onStartLeaveRoom)
        {
            if(roomInfo == null) return;

            currentRoomInfo = roomInfo;
            this.onStartLeaveRoom = onStartLeaveRoom;
            
            if (roomNameLabel != null)
            {
                roomNameLabel.text = roomInfo.Name;
            }

            if (playerCount != null)
            {
                playerCount.text = roomInfo.PlayerCount.ToString();
            }

            if (joinButton != null)
            {
                joinButton.onClick.RemoveAllListeners();
                joinButton.onClick.AddListener(OnJoinClick);
            }
        }

        private void OnJoinClick()
        {
            isJoining = true;
            PhotonNetwork.JoinRoom(currentRoomInfo.Name);
        }

        public override void OnJoinedRoom()
        {
            if (PhotonNetwork.CurrentRoom.IsOpen)
            {
                LobbyController.instance.ShowRoomUI();
            }
            else
            {
                onStartLeaveRoom?.Invoke();
                PhotonNetwork.LeaveRoom();
                Destroy(gameObject);
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (isJoining)
            {
                Destroy(gameObject);
            }
        }
    }
}

