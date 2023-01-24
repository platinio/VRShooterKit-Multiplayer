using System;
using Photon.Pun;
using TMPro;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public class RoomTimeLabel : MonoBehaviourPun
    {
        [SerializeField] private TextMeshProUGUI label = null;

        private RoomController m_roomController = null;
        private VR_NetworkManager m_networkManager = null;
        
        private void Start()
        {
            if (label != null) label.text = string.Empty;

            m_roomController = RoomController.instance;
            m_networkManager = VR_NetworkManager.instance;

        }

        private void Update()
        {
            if (label == null || m_roomController == null || m_networkManager == null) return;
            
            float leftTime = (m_roomController.GameMaxTime * 60.0f) - ((float)m_networkManager.GameTime);
            TimeSpan timeSpan = new TimeSpan(0, 0, (int)leftTime);
            label.text = timeSpan.ToString(@"mm\:ss");
        }
    }

}

