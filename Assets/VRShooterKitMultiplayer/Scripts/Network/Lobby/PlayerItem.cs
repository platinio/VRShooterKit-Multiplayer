using TMPro;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public class PlayerItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameLabel = null;
        [SerializeField] private TextMeshProUGUI deviceNameLabel = null;

        public void Initialize(Photon.Realtime.Player player)
        {
            if (playerNameLabel != null)
            {
                playerNameLabel.text = player.NickName;
            }

            if (deviceNameLabel != null)
            {
                deviceNameLabel.text = "Oculus Quest 2";
            }
        }

    }
}

