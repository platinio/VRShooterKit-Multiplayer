using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

namespace VRShooterKit.Multiplayer
{
    public class LobbyUI : MonoBehaviourPunCallbacks
    {
        [SerializeField] private RoomInfoUI roomInfoPrefab = null;
        [SerializeField] private Transform roomInfoListParent = null;
        [SerializeField] private NetworkSettings networkSettings = null;
        [SerializeField] private Button createRoomButton = null;

        private List<RoomInfoUI> currentRoomItemList = new List<RoomInfoUI>();

        private void Start()
        {
            createRoomButton.gameObject.SetActive(PhotonNetwork.IsConnected);
            
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.JoinLobby();
            }
            else
            {
                PhotonNetwork.AutomaticallySyncScene = false;
                PhotonNetwork.GameVersion = networkSettings.GameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }

            if (createRoomButton != null)
            {
                createRoomButton.onClick.AddListener(OnCreateRoomClick);
            }
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
            createRoomButton.gameObject.SetActive(true);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            CleanCurrentRooms();
            UpdateRoomUI(roomList);
        }

        private void UpdateRoomUI(List<RoomInfo> roomList)
        {
            foreach (RoomInfo roomInfo in roomList)
            {
                if(!roomInfo.IsVisible || !roomInfo.IsOpen) continue;

                RoomInfoUI roomUI = Instantiate(roomInfoPrefab, roomInfoListParent);
                roomUI.Initialize(roomInfo, OnStartLeaveRoom);
                currentRoomItemList.Add(roomUI);
            }
        }

        private void OnStartLeaveRoom()
        {
            gameObject.SetActive(false);
        }

        public override void OnLeftRoom()
        {
            gameObject.SetActive(true);
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogWarning("Join failed: " + message);
        }

        private void CleanCurrentRooms()
        {
            foreach (var roomUI in currentRoomItemList)
            {
                if(roomUI == null) continue; 
                
                Destroy(roomUI.gameObject);
            }

            currentRoomItemList.Clear();
        }

        private void OnCreateRoomClick()
        {
            LobbyController.instance.ShowCreateRoomUI();
        }
    }
}

