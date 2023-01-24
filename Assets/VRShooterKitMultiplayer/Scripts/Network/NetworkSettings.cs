using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    [CreateAssetMenu(fileName = "NetworkSettings", menuName = "VRShooterKit/Multiplayer/Create Settings")]
    public class NetworkSettings : ScriptableObject
    {
        [SerializeField] private string multiplayerSceneName = null;
        [SerializeField] private string lobbySceneName = null;
        [SerializeField] private string gameEndSceneName = null;
        [SerializeField] private string gameVersion = null;
        [SerializeField] private VR_NetworkPlayer oculusPlayerPrefab = null;

        public static NetworkSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<NetworkSettings>("NetworkSettings");
                }

                return _instance;
            }
        }

        private static NetworkSettings _instance = null;

        public string GameVersion => gameVersion;
        public string MultiplayerSceneName => multiplayerSceneName;
        public string LobbySceneName => lobbySceneName;
        public string GameEndSceneName => gameEndSceneName;

        public VR_NetworkPlayer PlayerPrefab
        {
            get
            {
                #if SDK_OCULUS
                return oculusPlayerPrefab;
                #endif
                return null;
            }
        }
    }
}

