using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VRShooterKit.Multiplayer
{
    public class LeaderboardItem : MonoBehaviour
    {
        [SerializeField] private Image background = null;
        [SerializeField] private TextMeshProUGUI nicknameLabel = null;
        [SerializeField] private TextMeshProUGUI killCounterLabel = null;
        [SerializeField] private TextMeshProUGUI deathCounterLabel = null;
        [SerializeField] private Image winnerImage = null;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color mineColor;

        public void Show(LeaderboardItemInfo leaderboardItem)
        {
            if (nicknameLabel != null)
            {
                nicknameLabel.text = leaderboardItem.Nickname;
            }

            if (killCounterLabel != null)
            {
                killCounterLabel.text = leaderboardItem.KillCounterInfo.KillCounter.ToString();
            }

            if (deathCounterLabel != null)
            {
                deathCounterLabel.text = leaderboardItem.KillCounterInfo.DeathCounter.ToString();
            }

            if (winnerImage != null)
            {
                winnerImage.enabled = leaderboardItem.IsWinner;
            }

            if (background != null)
            {
                background.color = leaderboardItem.IsMine ? mineColor : normalColor;
            }
        }
    }

}

