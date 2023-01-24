using System;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace VRShooterKit.Multiplayer
{
    public class KillCounterTracker : Platinio.Singleton<KillCounterTracker>
    {
        private Dictionary<VR_NetworkPlayer, KillCounterInfo> killInfoDictionary = new Dictionary<VR_NetworkPlayer, KillCounterInfo>();
        private List<LeaderboardItemInfo> leaderboardInfoList = new List<LeaderboardItemInfo>();

        public Dictionary<VR_NetworkPlayer, KillCounterInfo> KillInfoDictionary { get => killInfoDictionary; }
        public event Action<VR_NetworkPlayer, KillCounterInfo> OnKillCounterInfoCreated = null;
        
        public List<LeaderboardItemInfo> LeaderboardInfoList => leaderboardInfoList;

        protected override void Start()
        {
            base.Start();
            VR_NetworkManager.instance.OnNotifyNetworkPlayer += OnNotifyNetworkPlayer;

            foreach (var player in VR_NetworkManager.instance.NetworkPlayersPool.Values)
            {
                AddNewPlayer(player);
            }
        }

        private void OnDestroy()
        {
            VR_NetworkManager.instance.OnNotifyNetworkPlayer -= OnNotifyNetworkPlayer;
        }

        private void OnNotifyNetworkPlayer(VR_NetworkPlayer player, int playerViewId)
        {
            if (!killInfoDictionary.ContainsKey(player))
            {
                AddNewPlayer(player);
            }
        }

        private void AddNewPlayer(VR_NetworkPlayer player)
        {
            killInfoDictionary[player] = new KillCounterInfo(player.ActorNumber);
            player.DamageableNetworkManager.OnDie.AddListener(delegate { OnPlayerDie(player); });
            OnKillCounterInfoCreated?.Invoke(player, killInfoDictionary[player]);
            
            //save leaderboard data in order to build the player list after the game ends
            leaderboardInfoList.Add(new LeaderboardItemInfo(player, killInfoDictionary[player]));
        }

        private void OnPlayerDie(VR_NetworkPlayer player)
        {
            if (killInfoDictionary.TryGetValue(player, out KillCounterInfo victimInfo))
            {
                victimInfo.DeathCounter++;

                var killerPlayer = TryGetLastAttacker(player);

                if (killerPlayer != null && killerPlayer != player && killInfoDictionary.TryGetValue(killerPlayer, out KillCounterInfo killerInfo))
                {
                    killerInfo.KillCounter++;
                }
            }
        }

        private VR_NetworkPlayer TryGetLastAttacker(VR_NetworkPlayer player)
        {
            if (player.DamageableNetworkManager.LastDamageInfo == null)
            {
                return null;
            }

            GameObject sender = player.DamageableNetworkManager.LastDamageInfo.sender;

            if (sender == null)
            {
                return null;
            }

            return sender.GetComponent<VR_NetworkPlayer>();
        }
    }

    public class KillCounterInfo
    {
        public int DeathCounter = 0;
        public int KillCounter = 0;
        public int ActorNumber = 0;

        public KillCounterInfo(int actorNumber)
        {
            ActorNumber = actorNumber;
        }
    }

    public class LeaderboardItemInfo
    {
        public KillCounterInfo KillCounterInfo;
        public string Nickname;
        public int ActorNumber;
        public bool IsMine = false;
        public bool IsWinner = false;

        public LeaderboardItemInfo(VR_NetworkPlayer player, KillCounterInfo info)
        {
            KillCounterInfo = info;
            Nickname = player.Nickname;
            ActorNumber = player.ActorNumber;
            IsMine = ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
        }
    }
}

