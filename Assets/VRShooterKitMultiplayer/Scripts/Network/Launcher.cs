using System;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRShooterKit.Multiplayer
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        [SerializeField] private NetworkSettings networkSettings = null;
        [SerializeField] private VR_NetworkPlayer player = null;
        
        private IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);
            PhotonNetwork.AutomaticallySyncScene = false;

            PhotonNetwork.GameVersion = networkSettings.GameVersion;
            PhotonNetwork.ConnectUsingSettings();

            while (!PhotonNetwork.IsConnected) yield return new WaitForSeconds(Time.deltaTime);
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("connected to master");
            PhotonNetwork.JoinOrCreateRoom("NewRoom", new RoomOptions(), null);
        }

        public override void OnJoinedLobby()
        {
            PhotonNetwork.JoinRandomRoom();
        }
        
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Join room failed");
        }
    }
}

