using System;
using System.Collections.Generic;
using System.Linq;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Networking
{
    public class FightManager : FightManagerBehavior
    {
        public static FightManager instance;
        public UnityEvent OnPlayerDied;
        public UnityEvent OnFightEnd;
        public int MaxDeaths = 3;
        public float GameTimer = 300f;

        public bool fightStarted = false;
        // Track Kills Deaths
        public List<ScoreData> scores = new List<ScoreData>();
        private bool localPlayerAlive = true;

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
            GenerateScores();
        }

        private void GenerateScores()
        {
            var players = LobbyManager.instance.players;
            for (var index = 0; index < players.Count; index++)
            {
                var playerData = players[index];
                    scores.Add(new ScoreData(index, playerData.netID));
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
            if (!localPlayerAlive)
                return;
            var pID = args.GetNext<uint>();
            var target = args.Info.SendingPlayer;
            int currentDeaths = 0;
            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i].playerID == pID)
                {
                    var score = scores[i];
                    score.deaths++;
                    currentDeaths = score.deaths;
                    scores[i] = score;
                    break;
                }
            }
            
            if (NetworkManager.Instance.IsServer)
            {
                networkObject.SendRpc(RPC_PLAYER_DIED,Receivers.Others, pID);
            }
            else if (GameManager.instance.PlayerData.netID == pID)
            {
                localPlayerAlive = false;

                Debug.Log("LocalPlayer Died");
                if (currentDeaths < MaxDeaths)
                {
                    RespawnPlayer();           
                }
                else
                {
                    GameManager.instance.player.gameObject.SetActive(false);
                }
            }
        }

        private void RespawnPlayer()
        {
            var p = GameManager.instance.player;
            p.gameObject.SetActive(false);
            LeanTween.delayedCall(3f,() =>
            {
                var terrains = FindObjectsOfType<Terrain>();
                var pos = terrains[Random.Range(0, terrains.Length)].transform.position + (Vector3.up *2);
                p.gameObject.transform.position = pos;
                p.gameObject.SetActive(true);
                p.networkObject.SnapInterpolations();
                localPlayerAlive = true;

            });
            
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

        public void AddDeath()
        {
            deaths++;
        }
        
    }
}
