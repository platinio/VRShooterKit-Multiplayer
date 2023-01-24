using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Platinio;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace VRShooterKit.Multiplayer
{
    public class RoomController : Singleton<RoomController>, IMatchmakingCallbacks
    {
        public Room CurrentRoom = null;

        private const string kRoomTimeKey = "gameTime";
        private const string kStartTimeKey = "startTime";

        private bool checkForGameEnd = false;
        private int gameMaxTime = 0;

        /// <summary>
        /// The room game max time in minutes (directly from user input)
        /// </summary>
        public int GameMaxTime => gameMaxTime;
        
        public void CreateRoom(string roomName, int maxPlayers, int gameTime)
        {
            Hashtable roomProperties = new Hashtable();
            roomProperties.Add(kRoomTimeKey, gameTime);
            roomProperties.Add(kStartTimeKey, PhotonNetwork.Time);
            
            PhotonNetwork.CreateRoom(roomName, new RoomOptions()
            {
                MaxPlayers = (byte)maxPlayers, 
                CustomRoomProperties = roomProperties
            });
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
            if (!checkForGameEnd || !PhotonNetwork.IsMasterClient) return;

            int gameTimeInMinutes = (int)(VR_NetworkManager.instance.GameTime / 60);
            if (gameTimeInMinutes >= gameMaxTime)
            {
                checkForGameEnd = false;
                PhotonNetwork.LoadLevel(NetworkSettings.Instance.GameEndSceneName);
                VR_NetworkManager.instance.ExitRoom();
            }
        }

        private void OnGameStart()
        {
            checkForGameEnd = true;
            
            if (CurrentRoom != null)
            {
                CurrentRoom.IsOpen = false;
            }
        }

        public void OnFriendListUpdate(List<FriendInfo> friendList)
        {
            
        }

        public void OnCreatedRoom()
        {
            CurrentRoom = PhotonNetwork.CurrentRoom;
            
            if(CurrentRoom.CustomProperties.TryGetValue(kRoomTimeKey, out var time))
            {
                gameMaxTime = (int) time;
            }
        }

        public void OnCreateRoomFailed(short returnCode, string message)
        {
            
        }

        public void OnJoinedRoom()
        {
            CurrentRoom = PhotonNetwork.CurrentRoom;

            checkForGameEnd = false;
            //ensure there is just one listener
            VR_NetworkManager.instance.OnGameStart -= OnGameStart;
            VR_NetworkManager.instance.OnGameStart += OnGameStart;
        }

        public void OnJoinRoomFailed(short returnCode, string message)
        {
           
        }

        public void OnJoinRandomFailed(short returnCode, string message)
        {
          
        }

        public void OnLeftRoom()
        {
            
        }
    }
}

