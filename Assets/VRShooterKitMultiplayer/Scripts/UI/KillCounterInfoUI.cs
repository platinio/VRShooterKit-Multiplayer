using Photon.Pun;
using TMPro;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public class KillCounterInfoUI : MonoBehaviourPun
    {
        [SerializeField] private Transform m_root = null;
        [SerializeField] private VR_NetworkPlayer networkPlayer = null;
        [SerializeField] private TextMeshProUGUI killCounter = null;
        [SerializeField] private TextMeshProUGUI deathCounter = null;

        private KillCounterInfo killCounterInfo = null;
        
        private void Start()
        {
            if (!photonView.IsMine && m_root != null)
            {
                Destroy(m_root.gameObject);
            }

            if (KillCounterTracker.instance == null)
            {
                KillCounterTracker.OnInstanceCreated += OnKillCounterTrackerCreated;
            }
            else
            {
                OnKillCounterTrackerCreated(KillCounterTracker.instance);
            }
        }

        private void OnKillCounterTrackerCreated(KillCounterTracker instance)
        {
            instance.OnKillCounterInfoCreated += OnKillCounterInfoCreated;

            foreach (var keyValuePair in instance.KillInfoDictionary)
            {
                OnKillCounterInfoCreated(keyValuePair.Key, keyValuePair.Value);
            }
        }

        private void OnDestroy()
        {
            if (KillCounterTracker.instance != null)
            {
                KillCounterTracker.instance.OnKillCounterInfoCreated -= OnKillCounterInfoCreated;
            }
        }

        private void Update()
        {
            if (killCounterInfo == null)
            {
                return;
            }

            if (killCounter != null && killCounter.gameObject.activeInHierarchy)
            {
                killCounter.text = killCounterInfo.KillCounter.ToString();
            }

            if (deathCounter != null && deathCounter.gameObject.activeInHierarchy)
            {
                deathCounter.text = killCounterInfo.DeathCounter.ToString();
            }
        }

        private void OnKillCounterInfoCreated(VR_NetworkPlayer player, KillCounterInfo info)
        {
            if (player == networkPlayer)
            {
                killCounterInfo = info;
            }
        }

    }
}

