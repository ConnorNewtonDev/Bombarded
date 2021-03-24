using System;
using System.Collections.Generic;
using System.Linq;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using Utilities;
using Random = UnityEngine.Random;

namespace Networking
{
    public class FightManager : FightManagerBehavior
    {
        public static FightManager instance;
     
        private bool localPlayerAlive = true;
        [SerializeField] private Transform playerContainer;
        [SerializeField] private GameObject playerUI;
        [SerializeField] private GameObject playerWin;
        [SerializeField] private GameObject bombardedWin;
        [SerializeField] private TMP_Text winnerLabel;
        public UnityEvent OnPlayerDied;
        public UnityEvent OnFightEnd;

        public int MaxDeaths = 3;
        public float GameTimer = 300f;
        public bool fightStarted = false;
        public List<ScoreData> scores = new List<ScoreData>();
        public Action<int, int> playerDeath;
        public UnityEvent<int> playerWon;
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else if(instance != this)
                Destroy(gameObject);
            
            DontDestroyOnLoad(gameObject);
        }

        protected override void NetworkStart()
        {
            base.NetworkStart();
            networkObject.onDestroy += Destroy;
            GenerateScores();
        }

        private void GenerateScores()
        {
            var players = LobbyManager.instance.players;
            for (var index = 0; index < players.Count; index++)
            {
                var playerData = players[index];
                scores.Add(new ScoreData(index, playerData.netID));
                Instantiate(playerUI, playerContainer);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (fightStarted)
            {
                if (GameTimer > 0)
                {
                    GameTimer -= Time.deltaTime;
                }
                else
                {
                    fightStarted = false;
                    OnFightEnd?.Invoke();
                }
            }
        }

        public void PlayerDied(uint playerID)
        {
            networkObject.SendRpc(RPC_PLAYER_DIED, Receivers.Server, playerID);
        }
        
        public override void PlayerDied(RpcArgs args)
        { 
            Debug.Log("PlayerDied Fired");
            if (!localPlayerAlive)
                return;
            var pID = args.GetNext<uint>();
            var target = args.Info.SendingPlayer;
            int currentDeaths = 0;
            var data = new ScoreData();
            
            if (NetworkManager.Instance.IsServer)
            {
                for (int i = 0; i < scores.Count; i++)
                {
                    if (scores[i].playerID == pID)
                    {
                        var score = scores[i];
                        Debug.Log($"Deaths Before{score.deaths}");
                        score.deaths++;
                        Debug.Log($"Deaths After{score.deaths}");
                        scores[i] = score;
                        SyncScores();
                        networkObject.SendRpc(RPC_PLAYER_DIED,Receivers.Others, pID);
                        return;
                    }
                }
                

            }
            else if (GameManager.instance.PlayerData.netID == pID)
            {
                localPlayerAlive = false;
                foreach (var score in scores.Where(s => pID == s.playerID))
                {
                    data = score;
                    break;
                }
                playerDeath?.Invoke(data.playerIndex, data.deaths);

                Debug.Log("LocalPlayer Died");
                if (data.deaths < MaxDeaths)
                {
                    RespawnPlayer();
                }
                else
                {
                    CheckEndState();
                }
            }
            else
            {
                foreach (var score in scores.Where(s => pID == s.playerID))
                {
                    data = score;
                    break;
                }
                
                playerDeath?.Invoke(data.playerIndex, data.deaths);
            }
            
            
        }

        private void CheckEndState()
        {
            var p = GameManager.instance.player;
            p.networkObject.Destroy();
            p.gameObject.SetActive(false);

            List<ScoreData> remaining = new List<ScoreData>();
            remaining.Clear();
            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i].deaths < MaxDeaths)
                    remaining.Add(scores[i]);
                else
                {
                    Debug.Log("MaxDeathHit");
                }
            }

            if (remaining.Count == 1)
            {
                EndGame(remaining[0].playerIndex);
            }
            else if(remaining.Count == 0)
            {
                EndGame(-1);
            }
        }

        private void EndGame(int winningPlayer)
        {
            networkObject.SendRpc(RPC_FIGHT_WON, Receivers.Server, winningPlayer);
        }

        public override void FightWon(RpcArgs args)
        {
            var winner = args.GetNext<int>();

            if (NetworkManager.Instance.IsServer)
            {
                networkObject.SendRpc(RPC_FIGHT_WON, Receivers.Others, winner);
                return;
            }
            
            Debug.Log($"Winner IS {winner}");
            if (winner == -1)
            {
                // All Players Died [BOMBARDED]
                bombardedWin.SetActive(true);
            }
            else
            {
                // Show Winning Player
                winnerLabel.text = $"Player {winner + 1}";
                playerWin.SetActive(true);
            }
        }

        public void SyncScores()
        {
            var json = JsonHelper.ToJson(scores.ToArray());
            networkObject.SendRpc(RPC_SYNC_SCORES, Receivers.Others, json);
        }

        public override void SyncScores(RpcArgs args)
        {
            if (NetworkManager.Instance.IsServer)
                return;

            var json = args.GetNext<string>();
            var data = JsonHelper.FromJson<ScoreData>(json);

            for (int i = 0; i < data.Length; i++)
            {
                scores[i] = data[i];
            }
        }

        private void RespawnPlayer()
        {
            var p = GameManager.instance.player;
            p.networkObject.Destroy();
            p.gameObject.SetActive(false);
            LeanTween.delayedCall(3f,() =>
            {
                var terrains = FindObjectsOfType<Terrain>();
                var pos = terrains[Random.Range(0, terrains.Length)].transform.position + (Vector3.up *2);
                p = NetworkManager.Instance.InstantiatePlayer(0, pos, quaternion.identity);
                GameManager.instance.player = p;
                p.networkObject.SnapInterpolations();
                localPlayerAlive = true;

            });
            
        }

        private void Destroy(NetWorker netWorker)
        {
            if (instance == this)
                instance = null;
            
            Destroy(gameObject);
        }
    }
    
    [System.Serializable]
    public struct ScoreData
    {
        public int playerIndex;
        public int kills;
        public int deaths;
        public uint playerID;

        public ScoreData(int index, uint playerID)
        {
            this.playerID = playerID;
            playerIndex = index;
            kills = 0;
            deaths = 0;
        }

        public ScoreData(ScoreData other)
        {
            playerIndex = other.playerIndex;
            kills = other.kills;
            deaths = other.deaths;
            playerID = other.playerID;
        }

        public bool IsAlive(int maxdeaths)
        {
            return maxdeaths == deaths;
        }

        public void AddDeath()
        {
            deaths++;
        }
        
    }
}
