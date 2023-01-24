using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace VRShooterKit.Multiplayer
{
    public partial class RoomUI : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Transform playerListParent = null;
        [SerializeField] private PlayerItem playerItemPrefab = null;
        [SerializeField] private float updateRoomDelay = 1.0f;
        [SerializeField] private Button startGameButton = null;
        [SerializeField] private Button leaveRoomButton = null;
        [SerializeField] private GameObject lobbyPlayer = null;

        private Photon.Realtime.Player[] playersInRoomArray = null;
        private List<PlayerItem> currentPlayerItemList = new List<PlayerItem>();
        private Coroutine updateRoomRoutine = null;
        private float playerDestroyTime = 1.0f;

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            UpdatePlayerList();
        }

        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            UpdatePlayerList();
        }

        private void UpdatePlayerList()
        {
            CleanPlayerList();
            
            foreach (var player in playersInRoomArray)
            {
                var playerItem = Instantiate(playerItemPrefab, playerListParent);
                playerItem.Initialize(player);
                currentPlayerItemList.Add(playerItem);
            }
        }

        private void CleanPlayerList()
        {
            foreach (var playerItem in currentPlayerItemList)
            {
                Destroy(playerItem.gameObject);
            }
            
            currentPlayerItemList.Clear();
        }

        private void StartGame()
        {
            photonView.RPC(RPCIds.StartGameRPC, RpcTarget.AllViaServer);
        }

        [PunRPC]
        private void StartGameRPC()
        {
            StartCoroutine(StartGameRoutine());
        }

        private IEnumerator StartGameRoutine()
        {
            Destroy(lobbyPlayer);
            yield return new WaitForSeconds(playerDestroyTime);
            VR_NetworkManager.instance.StartGame();
        }

        public override void OnEnable()
        {
            if (updateRoomRoutine != null)
            {
                StopCoroutine(updateRoomRoutine);
                updateRoomRoutine = null;
            }

            updateRoomRoutine = StartCoroutine(UpdateRoomUI());

            if (startGameButton != null)
            {
                startGameButton.enabled = PhotonNetwork.IsMasterClient;
                startGameButton.onClick.AddListener(StartGame);
            }

            if (leaveRoomButton != null)
            {
                leaveRoomButton.onClick.AddListener(LeaveRoom);
            }
        }

        private void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
            gameObject.SetActive(false);
            LobbyController.instance.ShowLobbyUI();
        }

        private IEnumerator UpdateRoomUI()
        {
            while (true)
            {
                playersInRoomArray = PhotonNetwork.PlayerList;
                UpdatePlayerList();
                yield return new WaitForSeconds(updateRoomDelay);
            }
        }
    }
}

