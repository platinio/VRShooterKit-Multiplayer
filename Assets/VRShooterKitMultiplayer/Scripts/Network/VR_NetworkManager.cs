using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using VRSDK;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace VRShooterKit.Multiplayer
{
    public class VR_NetworkManager : PhotonSingleton<VR_NetworkManager>, IMatchmakingCallbacks, IPunOwnershipCallbacks, IInRoomCallbacks
    {
        private Dictionary<int, VR_NetworkPlayer> networkPlayersPool = new Dictionary<int, VR_NetworkPlayer>();
        public int LocalPlayerViewId = 0;
        
        private const float KSceneLoadLagDelay = 0.5f;
        private const float kRePositionTime = 1.0f;

        private bool timerIsRunning = false;
        private double startTime = 0.0f;
        private double gameTime = 0.0f;
        private bool allPlayersReadyTriggered = false;
        private VR_NetworkPlayer localNetworkPlayer = null;
        private bool shouldLoadLobbyOnLeftRoom = false;
        
        private const string kAllPlayersReadyRPC = "TriggerAllPlayersReady";
       
        
        public event Action OnAllPlayersReady = null;
        public event Action OnGameStart = null;
        public event Action<VR_NetworkPlayer, int> OnNotifyNetworkPlayer = null;
        public event Action OnBeforeLeaveRoom = null;

        /// <summary>
        /// Current game time in seconds
        /// </summary>
        public double GameTime => gameTime;

        public Dictionary<int, VR_NetworkPlayer> NetworkPlayersPool => networkPlayersPool;

        public VR_NetworkPlayer LocalNetworkPlayer
        {
            get
            {
                if (localNetworkPlayer == null)
                {
                    localNetworkPlayer = TryGetNetworkPlayerFromViewId(LocalPlayerViewId);
                }

                return localNetworkPlayer;
            }
        }

        public VR_Controller RightController
        {
            get
            {
                if (LocalNetworkPlayer == null)
                {
                    return null;
                }

                return LocalNetworkPlayer.RightController;
            }
        }

        public VR_Controller LeftController
        {
            get
            {
                if (LocalNetworkPlayer == null)
                {
                    return null;
                }

                return LocalNetworkPlayer.LeftController;
            }
        }

        public virtual void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        public virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        private void Update()
        {
            if (timerIsRunning)
            {
                gameTime = PhotonNetwork.Time - startTime;
            }
        }

        public void NotifyNetWorkPlayer(VR_NetworkPlayer networkPlayer , int playerViewId)
        {
            if (!networkPlayersPool.ContainsKey(playerViewId))
            {
                networkPlayersPool[playerViewId] = networkPlayer;
                OnNotifyNetworkPlayer?.Invoke(networkPlayer, playerViewId);
            }

            CheckIfAllPlayersAreReady();
        }

        private void CheckIfAllPlayersAreReady()
        {
            if (!PhotonNetwork.IsMasterClient || allPlayersReadyTriggered)
            {
                return;
            }

            if (networkPlayersPool.Count >= PhotonNetwork.CurrentRoom.PlayerCount)
            {
                allPlayersReadyTriggered = true;
                photonView.RPC(kAllPlayersReadyRPC, RpcTarget.AllViaServer);
            }
        }

        [PunRPC]
        private void TriggerAllPlayersReady()
        {
            startTime = PhotonNetwork.Time;
            timerIsRunning = true;
            
            OnAllPlayersReady?.Invoke();
        }

        public VR_NetworkPlayer TryGetNetworkPlayerFromViewId(int viewId)
        {
            if (networkPlayersPool.TryGetValue(viewId, out VR_NetworkPlayer player))
            {
                return player;
            }
            
            Debug.LogError("[VR_NetworkManager.TryGetNetworkPlayerFromViewId] Player viewId " + viewId + " doesn't exits");
            return null;
        }

        private IEnumerator InstantiatePlayerPrefab()
        {
            VR_NetworkPlayer networkPlayer = PhotonNetworkUtil.Instantiate(NetworkSettings.Instance.PlayerPrefab);
            networkPlayer.SetBodyVisualState(false);
            
            networkPlayer.CharacterController.StopSimulation = true;
            DontDestroyOnLoad(networkPlayer);
            
            VR_Manager.instance.SetCurrentPlayer(networkPlayer.Player);
            PhotonNetwork.LoadLevel(NetworkSettings.Instance.MultiplayerSceneName);

            while (NetworkTeamManager.instance == null) yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(KSceneLoadLagDelay);

            StartCoroutine(RepositionRoutine(networkPlayer));
        }

        private IEnumerator RepositionRoutine(VR_NetworkPlayer networkPlayer)
        {
            //hacky solution for a strange problem, some VR rig dont stay in place
            //even after setting the position and rotation so fix it for now setting those values for several frames
            float timer = 0.0f;
            Transform spawnPoint = NetworkTeamManager.instance.RequestPlayerSpawnPoint();

            while (timer < kRePositionTime)
            {
                networkPlayer.transform.position = spawnPoint.position;
                networkPlayer.transform.rotation = spawnPoint.rotation;
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            networkPlayer.CharacterController.StopSimulation = false;
            networkPlayer.SetBodyVisualState(true);
            networkPlayer.SetBodyVisualViaRPC(true);
        }

        public void ExitRoom()
        {
            OnBeforeLeaveRoom?.Invoke();
            OnBeforeLeaveRoom = null;
            
            PhotonNetwork.LeaveRoom();
        }

        public void ExitRoomAndLoadLobby()
        {
            ExitRoom();
            shouldLoadLobbyOnLeftRoom = true;
        }

        public void OnJoinedLobby()
        {
          
        }

        public void OnLeftLobby()
        {
           
        }

        public void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            
        }

        public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            
        }

        public void StartGame()
        {
            if (!PhotonNetwork.LocalPlayer.HasRejoined)
            {
                OnGameStart?.Invoke();
                StartCoroutine( InstantiatePlayerPrefab() );
            }
        }

        public void OnCreatedRoom() { }
        public void OnCreateRoomFailed(short returnCode, string message) { }
        public void OnJoinRoomFailed(short returnCode, string message) { }
        public void OnJoinRandomFailed(short returnCode, string message) { }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            
            RemovePhotonView();
            TryDestroyPlayerAvatar();
            ResetOnLeaveRoom();

            if (shouldLoadLobbyOnLeftRoom)
            {
                SceneManager.LoadScene(NetworkSettings.Instance.LobbySceneName);
                shouldLoadLobbyOnLeftRoom = false;
            }
        }

        private void RemovePhotonView()
        {
            var view = GetComponent<PhotonView>();

            if (view != null)
            {
                Destroy(view);
            }
        }

        private void TryDestroyPlayerAvatar()
        {
            if (networkPlayersPool.TryGetValue(LocalPlayerViewId, out var localPlayer) && localPlayer != null)
            {
                Destroy(localPlayer.gameObject);
            }
        }

        private void ResetOnLeaveRoom()
        {
            OnAllPlayersReady = null;
            OnGameStart = null;
            OnNotifyNetworkPlayer = null;
            OnBeforeLeaveRoom = null;
            OnNotifyNetworkPlayer = null;
            networkPlayersPool = new Dictionary<int, VR_NetworkPlayer>();
            allPlayersReadyTriggered = false;
            timerIsRunning = false;
            gameTime = 0.0f;
        }

        public void OnOwnershipRequest(PhotonView targetView, Photon.Realtime.Player requestingPlayer)
        {
            targetView.TransferOwnership(requestingPlayer);
        }

        public void OnOwnershipTransfered(PhotonView targetView, Photon.Realtime.Player previousOwner)
        {
            
        }

        public void OnOwnershipTransferFailed(PhotonView targetView, Photon.Realtime.Player senderOfFailedRequest)
        {
            
        }

        public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            
        }

        public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            CheckIfAllPlayersAreReady();
        }

        public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            
        }

        public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
        {
            
        }

        public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
        {
            
        }
    }
}

