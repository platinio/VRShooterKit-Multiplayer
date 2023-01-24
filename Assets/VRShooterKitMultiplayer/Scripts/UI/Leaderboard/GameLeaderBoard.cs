using System.Collections;
using System.Linq;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public class GameLeaderBoard : MonoBehaviour
    {
        [SerializeField] private LeaderboardItem leaderBoardItemPrefab = null;
        [SerializeField] private GameObject exitButton = null;
        [SerializeField] private Transform container = null;
#if SDK_OCULUS
        [SerializeField] private GameObject player = null;
#endif

        private void Start()
        {
            StartCoroutine(InitializeOVRManager());
            ShowLeaderboard();
        }

        private IEnumerator InitializeOVRManager()
        {
            yield return null;
#if SDK_OCULUS
            if (player != null)
            {
                var OVRManager = player.GetComponent<OVRManager>();

                if (OVRManager == null)
                {
                    player.AddComponent<OVRManager>();
                }
            }
#endif
        }

        private void ShowLeaderboard()
        {
            var sortedLeaderboard = KillCounterTracker.instance.LeaderboardInfoList.OrderByDescending(item => item.KillCounterInfo.KillCounter).ToList();
            sortedLeaderboard[0].IsWinner = true;
            
            foreach (var leaderboardItem in sortedLeaderboard)
            {
                var item = Instantiate(leaderBoardItemPrefab, container);
                item.Show(leaderboardItem);
            }
            
            exitButton.transform.SetAsLastSibling();
        }
    }
}

