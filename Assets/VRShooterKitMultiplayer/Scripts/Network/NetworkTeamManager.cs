using Photon.Pun;
using Platinio;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public class NetworkTeamManager : Singleton<NetworkTeamManager>
    {
        [SerializeField] private Transform[] spawnPoints = null;

        public Transform RequestPlayerSpawnPoint()
        {
            if (spawnPoints.Length < PhotonNetwork.LocalPlayer.ActorNumber)
            {
                return GetRandomSpawnPoint();
            }

            return spawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1];
        }

        public Transform GetRandomSpawnPoint()
        {
            return spawnPoints[Random.Range(0, spawnPoints.Length)];
        }

    }
}